//
//  RootPollTableViewViewController.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 3/14/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

// view controllers
#import "RootPollTableViewViewController.h"
#import "PollTypePopoverPickerController.h"
#import "PollCollectionViewCell.h"
#import "PollViewController.h"
#import "DetailPollViewController.h"
#import "AddNewPollViewController.h"
#import "ProfileViewController.h"
#import "ViewController.h"
#import "CategoryTableViewCell.h"

// data objects
#import "User.h"
#import "PollSummary.h"
#import "PollSectionHeaderView.h"

// helpers
#import "MyVoteService.h"
#import "PDKeychainBindings.h"
#import <QuartzCore/QuartzCore.h>
#import "SVProgressHUD.h"
#import "SDWebImage/UIImageView+WebCache.h"

NSString *const MALShowPollViewSegueIdentifier = @"Show Poll View";
NSString *const MALShowPollSegueIdentifier = @"Show Poll";
NSString *const MALAddNewPollSegueIdentifier = @"Add New Poll";
NSString *const MALShowProfileSegueIdentifier = @"Show Profile";
NSString *const MALShowLoginSegueIdentifier = @"Show Login";
const NSUInteger MALNoUserFoundTag = 99;
const NSUInteger MALCancelButton = 0;
const NSUInteger MALOKButton = 1;

@interface RootPollTableViewViewController () <PollTypePopoverDelegate, UICollectionViewDataSource, UICollectionViewDelegateFlowLayout, LoginDelegate, EditProfileDelegate, UIAlertViewDelegate>

// client service for authentication and user
@property (strong, nonatomic) MyVoteService *myVoteService;

// popover variables
@property (strong, nonatomic) NSArray *popoverItems;
@property (strong, nonatomic) PollTypePopoverPickerController *itemPicker;
@property (strong, nonatomic) UIPopoverController *itemPopoverController;

// category arrays
@property (strong, nonatomic) NSArray *allPollsArray;
@property (strong, nonatomic) NSMutableArray *funCategoryArray;
@property (strong, nonatomic) NSMutableArray *technologyCategoryArray;
@property (strong, nonatomic) NSMutableArray *entertainmentCategroyArray;
@property (strong, nonatomic) NSMutableArray *newsCategorArray;
@property (strong, nonatomic) NSMutableArray *sportsCategoryArray;
@property (strong, nonatomic) NSMutableArray *offTopicCategoryArray;

@property (strong, nonatomic) NSArray *arrayOfCategoryArrays;
@property (strong, nonatomic) NSMutableArray *itemsPerSectionArray;
@property (strong, nonatomic) NSMutableArray *sectionTitles;

// others
@property (strong, nonatomic) User *user;
@property PollFilterType selectedFilter;
@property BOOL isFetchingPoll;

@end

@implementation RootPollTableViewViewController


#pragma mark - Service Initialization

- (MyVoteService *)myVoteService
{
    if (!_myVoteService) {
        _myVoteService = [MyVoteService sharedInstance];
    }
    
    return _myVoteService;
}

#pragma mark - View Lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    // init
    self.isFetchingPoll = NO;
    
    // init arrays
    self.allPollsArray = [NSMutableArray array];
    self.funCategoryArray = [NSMutableArray array];
    self.technologyCategoryArray = [NSMutableArray array];
    self.entertainmentCategroyArray = [NSMutableArray array];
    self.newsCategorArray = [NSMutableArray array];
    self.sportsCategoryArray = [NSMutableArray array];
    self.offTopicCategoryArray = [NSMutableArray array];
    self.arrayOfCategoryArrays = @[self.funCategoryArray, self.technologyCategoryArray, self.entertainmentCategroyArray, self.newsCategorArray, self.sportsCategoryArray, self.offTopicCategoryArray];
    self.itemsPerSectionArray = [NSMutableArray array];
    self.sectionTitles = [NSMutableArray array];
        
    // add refresh control (Pull-to-Refresh)
    self.tableView.backgroundView.layer.zPosition -= 1; // http://stackoverflow.com/questions/18903076/uirefreshcontrol-and-uitableviews-backgroundview
    UIRefreshControl *refreshControl = [[UIRefreshControl alloc] init];
    [refreshControl setTintColor:[UIColor blackColor]];
    [refreshControl addTarget:self action:@selector(refresh:) forControlEvents:UIControlEventValueChanged];
    self.refreshControl = refreshControl;
    
    // hack #2 - http://stackoverflow.com/questions/19121276/uirefreshcontrol-incorrect-title-offset-during-first-run-and-sometimes-title-mis
    /**
    [refreshControl setAttributedTitle:[[NSAttributedString alloc] initWithString:@"Refreshing Polls..."]];
    dispatch_async(dispatch_get_main_queue(), ^{
        [self.refreshControl beginRefreshing];
        [self.refreshControl endRefreshing];
    });*/
    
    self.selectedFilter = PollFilterTypeAll;
    
    // load userID and profileID from keychain, if available
    NSString *profileID = [[PDKeychainBindings sharedKeychainBindings] objectForKey:@"profileID"];
    NSString *accessToken = [[PDKeychainBindings sharedKeychainBindings] objectForKey:@"accessToken"];
    
    if (profileID) {
        // init user with found ID and attach to Azure service
        MSUser *user = [[MSUser alloc] initWithUserId:profileID];
        user.mobileServiceAuthenticationToken = accessToken;
        self.myVoteService.client.currentUser = user;
        
        // set auth header for web API
        [[MALVoteClient sharedInstance] setAuthorizationHeaderWithToken:accessToken];
        
        // fetch user
        [self fetchUser];
    } else {
        // show login view
        [self performSelector:@selector(showLogin) withObject:self afterDelay:0.2];
    }
}

- (void)viewDidAppear:(BOOL)animated
{
    [super viewDidAppear:animated];
    
    // get polls
    NSString *profileID = [[PDKeychainBindings sharedKeychainBindings] objectForKey:@"profileID"];
    
    if (profileID) {
        [self refreshAllPollsWithFilter:self.selectedFilter];
    }
}

#pragma mark - Private Methods

- (void)fetchUser
{
    NSString *profileID = [[PDKeychainBindings sharedKeychainBindings] objectForKey:@"profileID"];
    
    // fetch user
    [[MALVoteClient sharedInstance] getUserByProfileID:profileID withCompletion:^(User *user) {
        if (user) {
            // successfully retrieved user - one is already created with this profile ID
            self.user = user;
        } else {
            // error finding user that should exist
            // tap OK to try loading your user again
            // or try loggin out and logging back in
            UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@""
                                                            message:NSLocalizedString(@"We had trouble finding your user. Press OK to try again. Press Cancel to logout.", nil)
                                                           delegate:self
                                                  cancelButtonTitle:NSLocalizedString(@"Cancel", nil)
                                                  otherButtonTitles:NSLocalizedString(@"OK", nil), nil];
            
            alert.tag = MALNoUserFoundTag;
            [alert show];
        }
    }];
}

- (void)logout
{
    [[PDKeychainBindings sharedKeychainBindings] removeObjectForKey:@"userID"];
    [[PDKeychainBindings sharedKeychainBindings] removeObjectForKey:@"accessToken"];
    [[PDKeychainBindings sharedKeychainBindings] removeObjectForKey:@"profileID"];
    [[NSUserDefaults standardUserDefaults] synchronize];
    
    // remove user from self and client so they are allowed to login again
    self.user = nil;
    [self.myVoteService.client logout];
    
    // remove all polls and reload table view
    [self cleanAllArrays];
    [self.tableView reloadData];
    
    // show initial login screen
    [self performSegueWithIdentifier:MALShowLoginSegueIdentifier sender:self];
}

- (void)refresh:(id)sender
{
    // check it user is logged in first
    if (self.user) {
        [sender endRefreshing];
    
        // refresh polls after collection view refreshed
        [self refreshAllPollsWithFilter:self.selectedFilter];
    }
}

- (void)cleanAllArrays
{
    self.allPollsArray = nil;
    
    // remove objects fro mutable arrays
    [self.itemsPerSectionArray removeAllObjects];
    [self.sectionTitles removeAllObjects];
    [self.funCategoryArray removeAllObjects];
    [self.technologyCategoryArray removeAllObjects];
    [self.newsCategorArray removeAllObjects];
    [self.sportsCategoryArray removeAllObjects];
    [self.entertainmentCategroyArray removeAllObjects];
    [self.offTopicCategoryArray removeAllObjects];
}

- (void)putPollIntoCategory:(PollSummary *)pollSummary
{
    // filter polls by category and place in proper category array
    if (pollSummary.categoryType == PollCategoryTypeFun) {
        [self.funCategoryArray addObject:pollSummary];
    } else if (pollSummary.categoryType == PollCategoryTypeTechnology) {
        [self.technologyCategoryArray addObject:pollSummary];
    } else if (pollSummary.categoryType == PollCategoryTypeEntertainment) {
        [self.entertainmentCategroyArray addObject:pollSummary];
    } else if (pollSummary.categoryType == PollCategoryTypeNews) {
        [self.newsCategorArray addObject:pollSummary];
    } else if (pollSummary.categoryType == PollCategoryTypeSports) {
        [self.sportsCategoryArray addObject:pollSummary];
    } else if (pollSummary.categoryType == PollCategoryTypeOffTopic) {
        [self.offTopicCategoryArray addObject:pollSummary];
    }
}

// specifier options:
// Home is default
// if Most Popular, return popular polls
// if Recently Added, return newest polls
// if My Polls, return polls of user
// if My Active Polls, return active polls of user
- (void)refreshAllPollsWithFilter:(PollFilterType)filterType
{
    // show HUD
    [SVProgressHUD showWithStatus:NSLocalizedString(@"Loading Polls...", nil) maskType:SVProgressHUDMaskTypeClear];
    
    // filter polls
    [[MALVoteClient sharedInstance] getPollWithFilterType:filterType withCompletion:^(NSArray *polls) {
        // dismiss HUD
        [SVProgressHUD dismiss];
            
        // check polls array
        if (polls && polls.count > 0) {
            // remove polls from all arrays
            [self cleanAllArrays];
                
            // sort polls
            for (PollSummary *currentPollSummary in polls) {
                // filter polls by category and place in proper category array
                [self putPollIntoCategory:currentPollSummary];
            }
                
            // set all polls
            self.allPollsArray = polls;
                
            // refresh table view
            [self.tableView reloadData];
        } else {
            // no polls
            UIAlertView *alert = [[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error", nil)
                                                            message:NSLocalizedString(@"Failed to retrieve polls. Please make sure you are connected to the Internet. If so, please retry.", nil)
                                                            delegate:nil
                                                    cancelButtonTitle:@"OK"
                                                    otherButtonTitles:nil];
                
            // show alert
            [alert show];
        }
    }];
}

- (void)showLogin
{
    [self performSegueWithIdentifier:MALShowLoginSegueIdentifier sender:self];
}

#pragma mark - IBActions

- (IBAction)browseButtonTapped:(UIBarButtonItem *)sender
{
    if (!self.itemPicker) {
        self.itemPicker = [[PollTypePopoverPickerController alloc] init];
        self.itemPicker.delegate = self;
        self.itemPopoverController = [[UIPopoverController alloc] initWithContentViewController:self.itemPicker];
    }
    
    if (self.itemPopoverController.isPopoverVisible) {
        [self.itemPopoverController dismissPopoverAnimated:YES];
    } else {
        // display popover
        [self.itemPopoverController presentPopoverFromBarButtonItem:self.navigationItem.leftBarButtonItem
                                           permittedArrowDirections:UIPopoverArrowDirectionAny
                                                           animated:YES];
    }
}

#pragma mark - LoginDelegate

- (void)foundExistingUser:(User *)user
{
    self.user = user;
    
    // fetch polls
    [self refreshAllPollsWithFilter:self.selectedFilter];
}

#pragma mark - EditProfileDelegate

- (void)updatedUser:(User *)user
{
    self.user = user;
}

#pragma mark - PollTypePopoverDelegate

- (void)itemSelected:(NSString *)item
{
    // need to handle - Home, Most Popular, Recently Added, My Polls, My Active Polls, Reported Polls
    if ([item isEqualToString:NSLocalizedString(@"Home", nil)]) {
        self.selectedFilter = PollFilterTypeAll;
        [self refreshAllPollsWithFilter:self.selectedFilter];
    } else if ([item isEqualToString:NSLocalizedString(@"Most Popular", nil)]) {
        self.selectedFilter = PollFilterTypeMostPopular;
        [self refreshAllPollsWithFilter:self.selectedFilter];
    } else if ([item isEqualToString:NSLocalizedString(@"Recently Added", nil)]) {
        self.selectedFilter = PollFilterTypeNewest;
        [self refreshAllPollsWithFilter:self.selectedFilter];
    } else if ([item isEqualToString:NSLocalizedString(@"My Polls", nil)]) {
        
    } else if ([item isEqualToString:NSLocalizedString(@"My Active Polls", nil)]) {
        
    } else if ([item isEqualToString:NSLocalizedString(@"Edit Profile", nil)]) {
        [self performSegueWithIdentifier:MALShowProfileSegueIdentifier sender:self];
    } else if ([item isEqualToString:NSLocalizedString(@"Logout", nil)]) {
        [self logout];
    }
    
    [self.itemPopoverController dismissPopoverAnimated:YES];
}

#pragma mark - Segue

- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
    if (self.itemPopoverController.isPopoverVisible) {
        [self.itemPopoverController dismissPopoverAnimated:YES];
    }
    
    // pass data to next view controller
    if ([segue.identifier isEqualToString:MALShowLoginSegueIdentifier]) {
        UINavigationController *navController = segue.destinationViewController;
        ViewController *loginView = [navController.viewControllers objectAtIndex:0];
        loginView.delegate = self;
        loginView.myVoteService = self.myVoteService;
    } else if ([segue.identifier isEqualToString:MALShowProfileSegueIdentifier]) {
        ProfileViewController *profileVC = segue.destinationViewController;
        profileVC.delegate = self;
        profileVC.user = self.user;
    } else if ([segue.identifier isEqualToString:MALShowPollSegueIdentifier]) {
        PollViewController *pollVC = segue.destinationViewController;
        pollVC.currentUser = self.user;
        pollVC.pollSummary = sender;
    } else if ([segue.identifier isEqualToString:MALShowPollViewSegueIdentifier]) {
        DetailPollViewController *pollVC = segue.destinationViewController;
        pollVC.currentUser = self.user;
        pollVC.pollSummary = sender;
    } else if ([segue.identifier isEqualToString:MALAddNewPollSegueIdentifier]) {
        AddNewPollViewController *addVC = segue.destinationViewController;
        addVC.userID = self.user.UserId;
    }
}

#pragma mark - UIAlertViewDelegate

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
    if (buttonIndex == MALCancelButton) {
        // log user out
        [self logout];
    } else if (buttonIndex == MALOKButton) {
        // retry searching for user
        [self fetchUser];
    }
}

#pragma mark - UITableViewDataSource Methods

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    // only # of categories with > 0 items
    int items = 0;
    for (NSArray *currentArray in self.arrayOfCategoryArrays) {
        if (currentArray.count > 0) {
            items++;
            [self.itemsPerSectionArray addObject:[NSNumber numberWithInteger:currentArray.count]];
            
            // determine which array has count > 0 and store titel
            if (currentArray == self.funCategoryArray) {
                [self.sectionTitles addObject:NSLocalizedString(@"Fun", nil)];
            } else if (currentArray == self.technologyCategoryArray) {
                [self.sectionTitles addObject:NSLocalizedString(@"Technology", nil)];
            } else if (currentArray == self.entertainmentCategroyArray) {
                [self.sectionTitles addObject:NSLocalizedString(@"Entertainment", nil)];
            } else if (currentArray == self.newsCategorArray) {
                [self.sectionTitles addObject:NSLocalizedString(@"News", nil)];
            } else if (currentArray == self.sportsCategoryArray) {
                [self.sectionTitles addObject:NSLocalizedString(@"Sports", nil)];
            } else if (currentArray == self.offTopicCategoryArray) {
                [self.sectionTitles addObject:NSLocalizedString(@"Off-Topic", nil)];
            }
        }
    }
    
    return items;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString *CellIdentifier = @"CategoryCell";
    
    CategoryTableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier forIndexPath:indexPath];
    cell.backgroundColor = [UIColor clearColor];
    
    // setup the cell
    if (self.itemsPerSectionArray.count > 0 && self.sectionTitles.count > 0) {
        NSNumber *items = [self.itemsPerSectionArray objectAtIndex:indexPath.row];
        NSString *pollNumberString = @"";
        
        // determine text for number of polls in category
        if (items.integerValue > 1) {
            pollNumberString = [NSString stringWithFormat:@"%d Polls", items.integerValue];
        } else {
            pollNumberString = [NSString stringWithFormat:@"%d Poll", items.integerValue];
        }
        
        // set category label
        NSString *categoryText = [self.sectionTitles objectAtIndex:indexPath.row];
        cell.categoryTitleLabel.text = [NSString stringWithFormat:@"%@ (%@)", categoryText, pollNumberString];
    }
    
    return cell;
}

- (void)tableView:(UITableView *)tableView willDisplayCell:(CategoryTableViewCell *)cell forRowAtIndexPath:(NSIndexPath *)indexPath
{
    [cell setCollectionViewDataSourceAndDelegate:self index:indexPath.row];
}

#pragma mark - UICollectionViewDataSource Methods

- (NSInteger)collectionView:(IndexedCollectionView *)collectionView numberOfItemsInSection:(NSInteger)section
{
    // number of items returned when we receive the polls back from the web service call
    NSNumber *items = @0;
    if (self.itemsPerSectionArray.count > 0) {
        items = [self.itemsPerSectionArray objectAtIndex:collectionView.index];
    }

    return items.integerValue;
}

- (UICollectionViewCell *)collectionView:(IndexedCollectionView *)collectionView cellForItemAtIndexPath:(NSIndexPath *)indexPath
{
    PollCollectionViewCell *cell = [collectionView dequeueReusableCellWithReuseIdentifier:@"CollectionViewCellIdentifier" forIndexPath:indexPath];
    
    // need to grab right poll here -- tricky!
    PollSummary *pollSummary = nil;
    
    if (self.sectionTitles.count > 0) {
        NSString *category = [self.sectionTitles objectAtIndex:collectionView.index];

        // select poll from right section
        if ([category isEqualToString:NSLocalizedString(@"Fun", nil)]) {
            pollSummary = [self.funCategoryArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:NSLocalizedString(@"Technology", nil)]) {
            pollSummary = [self.technologyCategoryArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:NSLocalizedString(@"Entertainment", nil)]) {
            pollSummary = [self.entertainmentCategroyArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:NSLocalizedString(@"News", nil)]) {
            pollSummary = [self.newsCategorArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:NSLocalizedString(@"Sports", nil)]) {
            pollSummary = [self.sportsCategoryArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:NSLocalizedString(@"Off-Topic", nil)]) {
            pollSummary = [self.offTopicCategoryArray objectAtIndex:indexPath.row];
        }
    }
    
    // set question on cell
    cell.questionLabel.text = pollSummary.question;
    //cell.questionLabel.backgroundColor = [UIColor yellowColor];
    [cell.questionLabel sizeToFit];
    
    // set image view on cell
    NSString *imageLinkString = pollSummary.imageLink;
    UIImage *placeholderImage = [UIImage imageNamed:@"NoImage"];
    if ([imageLinkString isEqualToString:@""]) {
        [cell.pollImageView setImage:placeholderImage];
    } else {
        NSURL *imageURL = [NSURL URLWithString:imageLinkString];
        [cell.pollImageView setImageWithURL:imageURL placeholderImage:placeholderImage];
    }
    
    return cell;
}

- (void)collectionView:(IndexedCollectionView *)collectionView didSelectItemAtIndexPath:(NSIndexPath *)indexPath
{
    // check that we have a valid user first
    if (self.user) {
        // get right poll summary
        if (self.sectionTitles.count > 0) {
            NSString *category = [self.sectionTitles objectAtIndex:collectionView.index];
            PollSummary *pollSummary = nil;
            
            // select poll from right section
            if ([category isEqualToString:NSLocalizedString(@"Fun", nil)]) {
                pollSummary = [self.funCategoryArray objectAtIndex:indexPath.row];
            } else if ([category isEqualToString:NSLocalizedString(@"Technology", nil)]) {
                pollSummary = [self.technologyCategoryArray objectAtIndex:indexPath.row];
            } else if ([category isEqualToString:NSLocalizedString(@"Entertainment", nil)]) {
                pollSummary = [self.entertainmentCategroyArray objectAtIndex:indexPath.row];
            } else if ([category isEqualToString:NSLocalizedString(@"News", nil)]) {
                pollSummary = [self.newsCategorArray objectAtIndex:indexPath.row];
            } else if ([category isEqualToString:NSLocalizedString(@"Sports", nil)]) {
                pollSummary = [self.sportsCategoryArray objectAtIndex:indexPath.row];
            } else if ([category isEqualToString:NSLocalizedString(@"Off-Topic", nil)]) {
                pollSummary = [self.offTopicCategoryArray objectAtIndex:indexPath.row];
            }

            // show poll view then decide to show results or the possible responses
            [self performSegueWithIdentifier:MALShowPollSegueIdentifier sender:pollSummary];
            //[self performSegueWithIdentifier:MALShowPollViewSegueIdentifier sender:pollSummary];
        }
    } else {
        // no active user - fetch again
        NSLog(@"No active user!");
    }
}

@end
