//
//  PollListViewController.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/12/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

#import "PollListViewController.h"
#import "PollViewController.h"
#import "NewPollViewController.h"
#import "PollTypePopoverPickerController.h"
#import "PollCollectionViewCell.h"
#import "PollStatsViewController.h"
#import "Poll.h"
#import "User.h"
#import "PollSectionHeaderView.h"
#import "MyVoteService.h"
#import "PDKeychainBindings.h"
#import "ViewController.h"
#import <QuartzCore/QuartzCore.h>
#import "SVProgressHUD.h"

@interface PollListViewController () <PollTypePopoverDelegate, UICollectionViewDataSource, UICollectionViewDelegateFlowLayout>

@property (strong, nonatomic) NSMutableArray *allPollsArray;

// client service for authentication and user
@property (strong, nonatomic) MyVoteService *myVoteService;
@property (strong, nonatomic) User *user;
@property (strong, nonatomic) NSNumber *userID;

// collection view
@property (weak, nonatomic) IBOutlet UICollectionView *collectionView;

// popover variables
@property (strong, nonatomic) NSArray *popoverItems;
@property (strong, nonatomic) PollTypePopoverPickerController *itemPicker;
@property (strong, nonatomic) UIPopoverController *itemPopoverController;

// category arrays
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
@property (strong, nonatomic) NSNumber *pollID;

@end

@implementation PollListViewController

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
    
    // set nav bar
    UIImage *navBarBackground = [[UIImage imageNamed:@"NavigationBarYellow"] resizableImageWithCapInsets:UIEdgeInsetsMake(0, 5, 0, 5)];
    [self.navigationController.navigationBar setBackgroundImage:navBarBackground forBarMetrics:UIBarMetricsDefault];
    
    // set view background
    [self.view setBackgroundColor:[UIColor colorWithPatternImage:[UIImage imageNamed:@"LandscapePageBackgroundDark"]]];
    
    // set bar button items
    UIImage *barButtonItemImage = [[UIImage imageNamed:@"barbuttonitem"] resizableImageWithCapInsets:UIEdgeInsetsMake(0, 5, 0, 5)];
    [self.navigationItem.rightBarButtonItem setBackgroundImage:barButtonItemImage forState:UIControlStateNormal barMetrics:UIBarMetricsDefault];
    
    UIButton *leftButton = [[UIButton alloc] initWithFrame:CGRectMake(0, 0, 100, 30)];
    [leftButton setBackgroundImage:barButtonItemImage forState:UIControlStateNormal];

    [leftButton setImage:[UIImage imageNamed:@"IconBrowse"] forState:UIControlStateNormal];
    [leftButton setTitle:NSLocalizedString(@"Browse", nil) forState:UIControlStateNormal];
    [leftButton.titleLabel setFont:[UIFont boldSystemFontOfSize:13]];
    
    // set title and image insets
    [leftButton setTitleEdgeInsets:UIEdgeInsetsMake(0, 0, 0, -10)];
    [leftButton setImageEdgeInsets:UIEdgeInsetsMake(0, -10, 0, 0)];
    
    // add action to button
    [leftButton addTarget:self action:@selector(popoverItemTapped:) forControlEvents:UIControlEventTouchUpInside];
    
    // set left bar button item to our custom button
    self.navigationItem.leftBarButtonItem = [[UIBarButtonItem alloc] initWithCustomView:leftButton];
    
    // init arrays
    self.allPollsArray = [NSMutableArray array];
    self.funCategoryArray = [NSMutableArray array];
    self.technologyCategoryArray = [NSMutableArray array];
    self.entertainmentCategroyArray = [NSMutableArray array];
    self.newsCategorArray = [NSMutableArray array];
    self.sportsCategoryArray = [NSMutableArray array];
    self.offTopicCategoryArray = [NSMutableArray array];
    self.arrayOfCategoryArrays = @[self.funCategoryArray, self.technologyCategoryArray, self.entertainmentCategroyArray, self.newsCategorArray,
                                   self.sportsCategoryArray, self.offTopicCategoryArray];
    self.itemsPerSectionArray = [NSMutableArray array];
    self.sectionTitles = [NSMutableArray array];
    
    // add image to navigation bar
    UIImage *logo = [UIImage imageNamed:@"ScreenLogo"];
    UIImageView *logoImageView = [[UIImageView alloc] initWithImage:logo];
    self.navigationItem.titleView = logoImageView;
    
    // register collection view cell class
    [self.collectionView registerClass:[PollCollectionViewCell class] forCellWithReuseIdentifier:@"PollCell"];
    
    // add refresh control (Pull-to-Refresh)
    UIRefreshControl *refreshControl = [[UIRefreshControl alloc] init];;
    [refreshControl addTarget:self action:@selector(refresh:) forControlEvents:UIControlEventValueChanged];
    [self.collectionView addSubview:refreshControl];

    // load whether this is first launch or not
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    BOOL hasBeenLaunched = [defaults boolForKey:@"hasBeenLaunched"];
    if (!hasBeenLaunched) {
        
        // this is null so it must be first launch - show overlay
        UIImageView *instructions = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"LandscapeHelpScreen"]];
        
        // need to change y origin since cannot get entire screen covered
        CGRect frame = instructions.frame;
        frame.origin.y -= 35;
        instructions.frame = frame;
        
        // add gesture recognizer to dismiss view
        UITapGestureRecognizer *tapGesture = [[UITapGestureRecognizer alloc] initWithTarget:self action:@selector(instructionsTapped:)];
        [instructions addGestureRecognizer:tapGesture];
        instructions.userInteractionEnabled = YES;
        [self.view addSubview:instructions];
    }
    
    // load userID and profileID from keychain, if available
    NSString *profileID = [[PDKeychainBindings sharedKeychainBindings] objectForKey:@"profileID"];
    NSString *accessToken = [[PDKeychainBindings sharedKeychainBindings] objectForKey:@"accessToken"];
    
    // remove all bindings in keychain  - TESTING ONLY!!!
    //[[PDKeychainBindings sharedKeychainBindings] removeObjectForKey:@"userID"];
    //[[PDKeychainBindings sharedKeychainBindings] removeObjectForKey:@"accessToken"];
    //[[PDKeychainBindings sharedKeychainBindings] removeObjectForKey:@"profileID"];
    //[[NSUserDefaults standardUserDefaults] removeObjectForKey:@"hasBeenLaunched"];
    //[[NSUserDefaults standardUserDefaults] synchronize];
    
    if (profileID) {
        // init user with found ID and attach to Azure service
        MSUser *user = [[MSUser alloc] initWithUserId:profileID];
        user.mobileServiceAuthenticationToken = accessToken;
        self.myVoteService.client.currentUser = user;
    } else {
        [self performSelector:@selector(showLogin) withObject:self afterDelay:0.1];
    }
}

- (void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    
    // get polls
    NSString *profileID = [[PDKeychainBindings sharedKeychainBindings] objectForKey:@"profileID"];
    
    if (profileID) {
        [self refreshAllPolls];
    }
    
    // load user
    NSString *userID = [[PDKeychainBindings sharedKeychainBindings] objectForKey:@"userID"]; // unique int
    self.userID = [NSNumber numberWithInteger:userID.integerValue];
    
    if ([self.userID isEqualToNumber:@0]) {
        [[MALVoteClient sharedInstance] getUserByProfileId:profileID withSuccess:^(AFHTTPRequestOperation *op, id response) {
            if (response) {
                self.user = [[User alloc] initUserWithDictionary:response];
                [[PDKeychainBindings sharedKeychainBindings] setObject:self.user.UserId.stringValue forKey:@"userID"];
            } else {
                NSLog(@"ERROR FETCHING USER!!");
            }
        } andFailure:^(AFHTTPRequestOperation *op, NSError *error) {
            NSLog(@"FAILED TO GET USER!!");
        }];
    }
}

#pragma mark - Private Methods

- (void)refresh:(id)sender
{
    [sender endRefreshing];
    
    // refresh polls after collection view refreshed
    [self refreshAllPolls];
}

- (void)cleanAllArrays
{
    [self.allPollsArray removeAllObjects];
    [self.funCategoryArray removeAllObjects];
    [self.technologyCategoryArray removeAllObjects];
    [self.newsCategorArray removeAllObjects];
    [self.sportsCategoryArray removeAllObjects];
    [self.entertainmentCategroyArray removeAllObjects];
    [self.offTopicCategoryArray removeAllObjects];
}

- (void)putPollIntoCategory:(NSDictionary *)poll
{
    // filter polls by category and place in proper category array
    NSString  *category = [poll objectForKey:@"Category"];
    
    if ([category isEqualToString:@"Fun"]) {
        [self.funCategoryArray addObject:poll];
    } else if ([category isEqualToString:@"Technology"]) {
        [self.technologyCategoryArray addObject:poll];
    } else if ([category isEqualToString:@"Entertainment"]) {
        [self.entertainmentCategroyArray addObject:poll];
    } else if ([category isEqualToString:@"News"]) {
        [self.newsCategorArray addObject:poll];
    } else if ([category isEqualToString:@"Sports"]) {
        [self.sportsCategoryArray addObject:poll];
    } else if ([category isEqualToString:@"Off-Topic"]) {
        [self.offTopicCategoryArray addObject:poll];
    }
}

- (void)refreshAllPolls
{
    // show HUD
    [SVProgressHUD show];
    
    [[MALVoteClient sharedInstance] getPollWithCategoryFilter:@"newest" withSuccess:^(AFHTTPRequestOperation *op, id response) {
        if (response) {
            // remove polls from all arrays
            [self cleanAllArrays];
            
            // Category - nsstring
            // Id = nsnumber
            // ImageLink = nsstring
            // Question = nsstring
            for (id poll in response) {
                [self.allPollsArray addObject:poll];
                
                // filter polls by category and place in proper category array
                [self putPollIntoCategory:poll];
            }
            
            // refresh collection view
            [self.collectionView reloadData];
            
        } else {
            NSLog(@"response is nil.");
        }
        
        // dismiss HUD
        [SVProgressHUD dismiss];
        
    } andFailure:^(AFHTTPRequestOperation *op, NSError *error) {        
        // dismiss HUD
        [SVProgressHUD dismiss];
        
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Failed to retrieve polls. Please make sure you are connected to the Internet. If so, please retry." delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alert show];
    }];
}

- (void)instructionsTapped:(UITapGestureRecognizer *)sender
{
    // set has been launched to true and save defaults
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    [defaults setBool:YES forKey:@"hasBeenLaunched"];
    [defaults synchronize];
    
    [sender.view removeFromSuperview];
    
    // enable buttons
}

- (void)showLogin
{
    [self performSegueWithIdentifier:@"ShowLogin" sender:self];
}

- (void)popoverItemTapped:(id)sender
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
        [self.itemPopoverController presentPopoverFromBarButtonItem:self.navigationItem.leftBarButtonItem permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
    }
}

#pragma mark - PollTypePopoverDelegate

- (void)itemSelected:(NSString *)item
{    
    // need to handle - Home, Most Popular, Recently Added, My Polls, My Active Polls, Reported Polls
    if ([item isEqualToString:NSLocalizedString(@"Home", nil)]) {
        [self refreshAllPolls];
    }
    
    [self.itemPopoverController dismissPopoverAnimated:YES];
}

- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
    if (self.itemPopoverController.isPopoverVisible) {
        [self.itemPopoverController dismissPopoverAnimated:YES];
    }
    
    if ([segue.identifier isEqualToString:@"ShowLogin"]) {
        UINavigationController *navController = segue.destinationViewController;
        ViewController *loginView = [navController.viewControllers objectAtIndex:0];
        loginView.myVoteService = self.myVoteService;
    } else if ([segue.identifier isEqualToString:@"Show Poll"]) {
        PollViewController *pollVC = segue.destinationViewController;
        pollVC.userID = self.userID;
        pollVC.pollID = self.pollID.integerValue;
    } else if ([segue.identifier isEqualToString:@"ShowNewPoll"]) {
        NewPollViewController *newPollVC = segue.destinationViewController;
        newPollVC.UserId = self.userID;
    } else if ([segue.identifier isEqualToString:@"Show Stats"]) {
        PollStatsViewController *statsVC = segue.destinationViewController;
        statsVC.pollID = self.pollID.integerValue;
    }
}

#pragma mark - UICollectionView Datasource

- (NSInteger)collectionView:(UICollectionView *)collectionView numberOfItemsInSection:(NSInteger)section
{
    // number of items returned when we receive the polls back from the web service call
    NSNumber *items = @0;
    if (self.itemsPerSectionArray.count > 0) {
        items = [self.itemsPerSectionArray objectAtIndex:section];
    }
    
    return items.integerValue;
}

- (NSInteger)numberOfSectionsInCollectionView:(UICollectionView *)collectionView
{
    // MAX = 6 AND MINIMUM = 0
    // need to determine how many arrays have counts > 0
    int sections = 0;
    [self.itemsPerSectionArray removeAllObjects];
    [self.sectionTitles removeAllObjects];
    for (NSArray *currentArray in self.arrayOfCategoryArrays) {
        if (currentArray.count > 0) {
            sections++;
            [self.itemsPerSectionArray addObject:[NSNumber numberWithInteger:currentArray.count]];
            
            // determine which array has count > 0 and store titel
            if (currentArray == self.funCategoryArray) {
                [self.sectionTitles addObject:@"Fun"];
            } else if (currentArray == self.technologyCategoryArray) {
                [self.sectionTitles addObject:@"Technology"];
            } else if (currentArray == self.entertainmentCategroyArray) {
                [self.sectionTitles addObject:@"Entertainment"];
            } else if (currentArray == self.newsCategorArray) {
                [self.sectionTitles addObject:@"News"];
            } else if (currentArray == self.sportsCategoryArray) {
                [self.sectionTitles addObject:@"Sports"];
            } else if (currentArray == self.offTopicCategoryArray) {
                [self.sectionTitles addObject:@"Off-Topic"];
            }
        }
    }
    
    return sections;
}

- (UICollectionViewCell *)collectionView:(UICollectionView *)collectionView cellForItemAtIndexPath:(NSIndexPath *)indexPath
{
    PollCollectionViewCell *cell = [collectionView dequeueReusableCellWithReuseIdentifier:@"PollCell" forIndexPath:indexPath];
    
    // need to grab right poll here -- tricky!
    NSDictionary *pollDictionary = nil;
    
    if (self.sectionTitles.count > 0) {
        NSString *category = [self.sectionTitles objectAtIndex:indexPath.section];
        
        // select poll from right section
        if ([category isEqualToString:@"Fun"]) {
            pollDictionary = [self.funCategoryArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:@"Technology"]) {
            pollDictionary = [self.technologyCategoryArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:@"Entertainment"]) {
            pollDictionary = [self.entertainmentCategroyArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:@"News"]) {
            pollDictionary = [self.newsCategorArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:@"Sports"]) {
            pollDictionary = [self.sportsCategoryArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:@"Off-Topic"]) {
            pollDictionary = [self.offTopicCategoryArray objectAtIndex:indexPath.row];
        }
    }
                                  
    [cell setupCellWithDictionary:pollDictionary];
    
    return cell;
}


#pragma mark - UICollectionViewDelegate

- (void)collectionView:(UICollectionView *)collectionView didSelectItemAtIndexPath:(NSIndexPath *)indexPath
{
    // get right poll
    if (self.sectionTitles.count > 0) {
        NSString *category = [self.sectionTitles objectAtIndex:indexPath.section];
        NSDictionary *pollDictionary = nil;
        
        // select poll from right section
        if ([category isEqualToString:@"Fun"]) {
            pollDictionary = [self.funCategoryArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:@"Technology"]) {
            pollDictionary = [self.technologyCategoryArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:@"Entertainment"]) {
            pollDictionary = [self.entertainmentCategroyArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:@"News"]) {
            pollDictionary = [self.newsCategorArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:@"Sports"]) {
            pollDictionary = [self.sportsCategoryArray objectAtIndex:indexPath.row];
        } else if ([category isEqualToString:@"Off-Topic"]) {
            pollDictionary = [self.offTopicCategoryArray objectAtIndex:indexPath.row];
        }
        
        // set poll ID to pass to next view
        self.pollID = [pollDictionary objectForKey:@"Id"];

        // if user has already voted then we should display stats view controller
        [[MALVoteClient sharedInstance] getUserResponseForPollID:self.pollID.integerValue forUserID:self.userID.integerValue withSuccess:^(AFHTTPRequestOperation *op, id response) {
            if (response) {
                NSArray *responseOptions = [response objectForKey:@"PollOptions"];
                // cycle through all options
                for (id option in responseOptions) {
                    NSNumber *isOptionSelected = [option objectForKey:@"IsOptionSelected"];
                    if ([isOptionSelected isEqualToNumber:@1]) {
                        // user already answered poll
                        [self performSegueWithIdentifier:@"Show Stats" sender:self];
                        break;
                    }
                }
                // user has yet to vote
                [self performSegueWithIdentifier:@"Show Poll" sender:self];
            } else {
                NSLog(@"Response is nil");
            }
        } andFailure:^(AFHTTPRequestOperation *op, NSError *error) {
            //NSLog(@"ERROR - %@", error.description);
            // user already answered poll
            [self performSegueWithIdentifier:@"Show Stats" sender:self];
        }];
    }
}

- (UICollectionReusableView *)collectionView:(UICollectionView *)collectionView viewForSupplementaryElementOfKind:(NSString *)kind atIndexPath:(NSIndexPath *)indexPath
{
    PollSectionHeaderView *headerView = [collectionView dequeueReusableSupplementaryViewOfKind:UICollectionElementKindSectionHeader withReuseIdentifier:@"PollSectionHeaderView" forIndexPath:indexPath];
    
    [headerView.sectionHeaderLabel setFont:[UIFont fontWithName:@"CreteRound-Regular" size:20]];
    [headerView.sectionPollTotalLabel setFont:[UIFont fontWithName:@"CreteRound-Regular" size:11]];
    
    NSString *pollNumberString = @"";
    
    if (self.itemsPerSectionArray.count > 0 && self.sectionTitles.count > 0) {
        NSNumber *items = [self.itemsPerSectionArray objectAtIndex:indexPath.section];
        if (items.integerValue > 1) {
            pollNumberString = [NSString stringWithFormat:@"%d Polls", items.integerValue];
        } else {
            pollNumberString = [NSString stringWithFormat:@"%d Poll", items.integerValue];
        }
        
        // setup header view
        headerView.sectionHeaderLabel.text = [self.sectionTitles objectAtIndex:indexPath.section];
        headerView.sectionPollTotalLabel.text = pollNumberString;
    }
    
    return headerView;
}

@end
