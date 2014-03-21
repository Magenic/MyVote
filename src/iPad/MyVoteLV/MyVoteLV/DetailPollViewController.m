//
//  DetailPollViewController.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/14/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "DetailPollViewController.h"
#import "CommentReplyViewController.h"

// data objects
#import "PollSummary.h"
#import "PollOption.h"
#import "Response.h"
#import "User.h"
#import "Comment.h"
#import "Result.h"
#import "PollResponse.h"
#import "SubmittedResponse.h"
#import "Poll.h"

// service
#import "MyVoteService.h"

// charts
#import <IGChart/IGChart.h>

// views
#import "PollHeaderView.h"
#import "CommentCell.h"
#import "BDBAttributedButton.h"

// helper
#import "SVProgressHUD.h"
#import "NSData+Base64.h"
#import "NSString+Convenience.h"
#import "NSDate+Convenience.h"

// activity
#import "DeletePollActivity.h"

// view tags
static NSInteger const MALTextfieldTag = 99;

// section count
static NSInteger const MALSectionsForUnansweredPoll = 2;
static NSInteger const MALSectionsForAnsweredPoll = 2;

// sections
static NSInteger const MALResponseSection = 0;
static NSInteger const MALSubmitVoteSection = 1;
static NSInteger const MALStatSection = 0;
static NSInteger const MALCommentsSection = 1;

// row heights
static CGFloat const MALUnansweredRowHeight = 60.0f;
static CGFloat const MALStatsRowHeight = 300.0f;
static CGFloat const MALSubmitRowHeight = 60.0f;

// section height
static CGFloat const MALSectionHeight = 50.0f;

// padding
static CGFloat const MALPadding = 15.0f;

@interface DetailPollViewController () <UIPopoverControllerDelegate, IGChartViewDelegate, UITextFieldDelegate, UIAlertViewDelegate, ReplyCommentDelegate>

@property BOOL hasViewBeenSetup;
@property BOOL hasUserAnsweredPoll;
@property (strong, nonatomic) UIPopoverController *popover;

// answer(s) handling
@property (strong, nonatomic) NSMutableArray *answersArray;
@property BOOL allowsMultiSelection;
@property int lastSelectedRow; // use for single selection
@property (strong, nonatomic) NSMutableSet *selectedRows; // use for multi-selection

// data objects
@property (strong, nonatomic) Response *response;
@property (strong, nonatomic) Poll *poll;
@property (strong, nonatomic) IGChartView *chart;
@property (strong, nonatomic) NSMutableArray *commentsArray;

// outlets
@property (weak, nonatomic) IBOutlet UITableView *tableView;
@property (weak, nonatomic) IBOutlet PollHeaderView *pollHeaderView;
@property (weak, nonatomic) IBOutlet UIView *fixedCommentView;
@property (weak, nonatomic) IBOutlet UITextField *commentTextField;

@end

@implementation DetailPollViewController

#pragma mark - View Lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    // hide fixed comment view initially
    [self.fixedCommentView setHidden:YES];
    
    // init bools
    self.hasViewBeenSetup = NO;
    self.hasUserAnsweredPoll = NO;
    
    // determine if current user has already answered this poll
    [self checkViewSetup];
    
    // setup question and image
    self.pollHeaderView.pollQuestionLabel.text = self.pollSummary.question;
    if ([self.pollSummary.imageLink isEqualToString:@""]) {
        [self.pollHeaderView.pollImageView setImage:[UIImage imageNamed:@"NoImage"]];
    } else {
        NSURL *imageURL = [NSURL URLWithString:self.pollSummary.imageLink];
        [self.pollHeaderView.pollImageView setImageWithURL:imageURL placeholderImage:[UIImage imageNamed:@"NoImage"]];
    }
    
    // setup background image
    UIImageView *backgroundImageView = nil;
    
    if (UIDeviceOrientationIsLandscape([[UIApplication sharedApplication] statusBarOrientation])) {
        // landscape
        backgroundImageView = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"background_landscape"]];
    } else {
        // portrait
        backgroundImageView = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"background_portrait"]];
    }
    
    // set image
    [self.tableView setBackgroundView:backgroundImageView];
    
    // init empty footer view
    self.tableView.tableFooterView = [[UIView alloc] init];
    
    // add observer
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(deletePoll:) name:@"UIActivity" object:nil];
}

- (void)viewWillDisappear:(BOOL)animated
{
    [super viewWillDisappear:animated];
    
    // dismiss popover if it's showing
    if ([self.popover isPopoverVisible]) {
        [self.popover dismissPopoverAnimated:YES];
    }
}

- (void)willAnimateRotationToInterfaceOrientation:(UIInterfaceOrientation)toInterfaceOrientation duration:(NSTimeInterval)duration
{
    // setup background image
    UIImageView *backgroundImageView = nil;
    
    if (UIDeviceOrientationIsLandscape(toInterfaceOrientation)) {
        // landscape
        backgroundImageView = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"background_landscape"]];
    } else {
        // portait
        backgroundImageView = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"background_portrait"]];
    }
    
    // set image
    [self.tableView setBackgroundView:backgroundImageView];
}

#pragma mark - View Setup

- (void)checkViewSetup
{
    // show progress HUD
    [SVProgressHUD show];
    
    // get response
    [[MALVoteClient sharedInstance] getUserResponseForPollID:self.pollSummary.pollID forUserID:self.currentUser.UserId withCompletion:^(Response *response) {
        // check response is valid
        if (response) {
            // view now setup
            self.hasViewBeenSetup = YES;
            
            // cycle through all options
            for (PollOption *option in response.pollOptions) {
                // see if any options are picked
                if (option.isOptionSelected) {
                    // user already answered poll
                    self.hasUserAnsweredPoll = YES;
                    
                    // get result since user has already answered
                    [self setupAnsweredView];
                    break;
                }
            }
            
            // user has yet to vote
            if (!self.hasUserAnsweredPoll) {
                [self setupUnansweredViewWithResponse:response];
            }
        } else {
            // updated API should never get here unless error
            [SVProgressHUD showErrorWithStatus:NSLocalizedString(@"Error fetching poll response!", nil)];
        }
    }];
}

- (void)setupAnsweredView
{
    // get poll results and comments
    [[MALVoteClient sharedInstance] getPollResultsForPollID:self.pollSummary.pollID withCompletion:^(Result *result) {
        // check if results is valid
        if (result) {
            // dismiss HUD
            [SVProgressHUD dismiss];
            
            // process results
            [self setupChartViewWithResult:result];
            
            // comments
            self.commentsArray = [NSMutableArray array];
            for (Comment *parentComment in result.comments) {
                // add parrent comment
                [self.commentsArray addObject:parentComment];
                
                // grab child comments
                for (Comment *childComment in parentComment.childComments) {
                    // add child comment
                    [self.commentsArray addObject:childComment];
                }
            }
            
            // sort comments - oldest first
            NSSortDescriptor *sortDescriptor = [[NSSortDescriptor alloc] initWithKey:@"commentCreationDate" ascending:YES];
            [self.commentsArray sortUsingDescriptors:@[sortDescriptor]];
            
            // reload table view to show dta
            [self.tableView reloadData];
            
            // show comments view
            [self.fixedCommentView setHidden:NO];
        } else {
            // dismiss HUD
            [SVProgressHUD dismiss];
            
            // show error
            UIAlertView *alert = [[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Failed to get poll results!", nil)
                                                            message:NSLocalizedString(@"There was an error retrieving the results. Please try again to select this poll from the main poll view.", nil)
                                                           delegate:nil
                                                  cancelButtonTitle:NSLocalizedString(@"OK", nil)
                                                  otherButtonTitles:nil];
            [alert show];
            
            // reload table view too
            [self.tableView reloadData];
        }
    }];
}

- (void)setupUnansweredViewWithResponse:(Response *)response
{
    // dismiss HUD
    [SVProgressHUD dismiss];
    
    // init some variables tied to this setup
    self.lastSelectedRow = -1;
    
    // initialize set
    self.selectedRows = [NSMutableSet set];
    
    // response contains everything we need for this view
    self.response = response;
    
    // reload table view
    [self.tableView reloadData];
}

- (void)setupChartViewWithResult:(Result *)result
{
    NSArray *arrayOfResults = result.pollOptionResults;
    
    // init variables before setting up chart
    NSUInteger answers = 0;
    NSMutableArray *responseCount = [NSMutableArray array];
    NSMutableArray *chartData = [NSMutableArray array];
    NSMutableArray *answersArray = [NSMutableArray array];
    NSInteger totalResponses = 0;
    
    // cycle through answers to collect information
    for (PollResponse *currentPollResponse in arrayOfResults) {
        answers++;
        totalResponses += currentPollResponse.responseCount;
        [responseCount addObject:@(currentPollResponse.responseCount)];
        [answersArray addObject:currentPollResponse.optionText];
    }
    
    // now calculate percentages for each answer
    for (NSNumber *responses in responseCount) {
        CGFloat value = (responses.integerValue / (CGFloat)totalResponses) * 100;
        NSNumber *percentage = [NSNumber numberWithDouble:value];
        [chartData addObject:percentage];
    }
    
    // configure chart and such here
    IGCategorySeriesDataSourceHelper *source = [[IGCategorySeriesDataSourceHelper alloc] init];
    source.values = chartData;
    source.labels = answersArray;
    
    self.chart = [[IGChartView alloc] initWithFrame:CGRectMake(15, 15, 600, 0)];
    self.chart.delegate = self;
    [self.chart setAutoresizingMask:UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight];
    [self.chart setTheme:[IGChartDefaultThemes IGTheme]];
    
    // x axis
    IGNumericXAxis *xAxis = [[IGNumericXAxis alloc] initWithKey:@"xAxis"];
    xAxis.minimum = 0;
    xAxis.maximum = 100;
    xAxis.interval = 10;
    
    // y axis
    IGCategoryYAxis *yAxis = [[IGCategoryYAxis alloc] initWithKey:@"yAxis"];
    
    [self.chart addAxis:xAxis];
    [self.chart addAxis:yAxis];
    
    IGBarSeries *barSeries = [[IGBarSeries alloc] initWithKey:@"barSeries"];
    barSeries.xAxis = xAxis;
    barSeries.yAxis = yAxis;
    barSeries.dataSource = source;
    
    [self.chart addSeries:barSeries];
}

#pragma mark - Poll Deletion

- (void)fetchThisPoll
{
    // need poll to check if user can delete it, only if they own it
    [[MALVoteClient sharedInstance] getPollWithID:self.pollSummary.pollID withCompletion:^(Poll *poll) {
        if (poll) {
            self.poll = poll;
        } else {
            // don't really need to do anything here...
            
        }
    }];
}

#pragma mark - Submit Vote

- (void)submitAnswer
{
    if (self.selectedRows.count > 0) {
        // setup response
        SubmittedResponse *response = [[SubmittedResponse alloc] init];
        response.pollID = self.response.pollID;
        response.userID = self.response.userID;
        response.comment = @"";
        
        // cycle through options
        NSInteger row = 0;
        NSMutableArray *responseArray = [NSMutableArray arrayWithCapacity:self.response.pollOptions.count];
        for (PollOption *currentOption in self.response.pollOptions) {
            // set up response, set if selected
            NSIndexPath *currentIndexPath = [NSIndexPath indexPathForRow:row++ inSection:0]; // only 1 section
            NSDictionary *response = nil;
            if ([self.selectedRows containsObject:currentIndexPath]) {
                // selected answer
                response = @{@"PollOptionID" : [NSNumber numberWithInteger:currentOption.pollOptionID],
                             @"IsOptionSelected" : @YES};
            } else {
                response = @{@"PollOptionID" : [NSNumber numberWithInteger:currentOption.pollOptionID],
                             @"IsOptionSelected" : @NO};
            }
            
            // add response dictionary to array
            [responseArray addObject:response];
        }
        
        // answers
        response.responseItems = [responseArray copy];
        
        // display HUD
        [SVProgressHUD showWithStatus:NSLocalizedString(@"Submitting Answer...", nil) maskType:SVProgressHUDMaskTypeClear];
        
        // submit response
        NSDictionary *responseJSON = [response serializeObjectForJSON];
        [[MALVoteClient sharedInstance] respondToPollWithResponse:responseJSON withCompletion:^(BOOL success) {
            // dismiss HUD
            [SVProgressHUD dismiss];
            
            // check success
            if (success) {
                // yes - now fetch results and display those
                self.hasUserAnsweredPoll = YES;
                [self setupAnsweredView];
            } else {
                // boo!
                UIAlertView *alert = [[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Error Submitting Response", nil)
                                                                message:NSLocalizedString(@"We were unable to process your response. Please try again.", nil)
                                                               delegate:nil
                                                      cancelButtonTitle:NSLocalizedString(@"OK", nil)
                                                      otherButtonTitles:nil];
                [alert show];
            }
        }];
    }
}

#pragma mark - Post Comment

- (Comment *)createNewCommentForThisPoll
{
    Comment *comment = [[Comment alloc] init];
    comment.userID = self.currentUser.UserId;
    comment.username = self.currentUser.Username;
    comment.commentText = self.commentTextField.text;
    comment.pollID = self.pollSummary.pollID;
    comment.parentCommentID = -1;
    
    return comment;
}

- (void)postComment:(Comment *)comment
{
    // post
    [[MALVoteClient sharedInstance] postComment:comment withCompletion:^(BOOL success) {
        if (success) {
            // success - post notification so cell handler can do its work
            // set date on comment since server will set that when we add it but won't be set locally
            NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
            [dateFormatter setDateFormat:@"h:mma M/d/yyyy"];
            
            comment.commentDate = [dateFormatter stringFromDate:[NSDate date]];
            
            // add comment to local array of comments update table
            [self.commentsArray addObject:comment];
            
            // clear textfield
            self.commentTextField.text = nil;
            
            // reload table view
            [self.tableView reloadData];
            
            // scroll to bottom
            NSIndexPath *indexPath = [NSIndexPath indexPathForRow:self.commentsArray.count - 1 inSection:MALCommentsSection];
            [self.tableView scrollToRowAtIndexPath:indexPath atScrollPosition:UITableViewScrollPositionBottom animated:YES];
        } else {
            // comment did not successfully post
            UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@""
                                                            message:NSLocalizedString(@"There was an error posting your comment. Please try again.", nil)
                                                           delegate:nil
                                                  cancelButtonTitle:NSLocalizedString(@"OK", nil)
                                                  otherButtonTitles:nil];
            [alert show];
        }
    }];
}

- (void)replyToComment:(UIButton *)sender
{
    // need to determine which reply button was tapped in which cell
    CGPoint buttonPosition = [sender convertPoint:CGPointZero toView:self.tableView];
    NSIndexPath *indexPath = [self.tableView indexPathForRowAtPoint:buttonPosition];
    
    if (indexPath) {
        // grab comment user is replying to
        //Comment *postedComment = self.commentsArray[indexPath.row];
    }
}

#pragma mark - Sharing

- (IBAction)sharePollTapped:(UIBarButtonItem *)sender
{
    // dismiss popover, if it's already showing
    if ([self.popover isPopoverVisible]) {
        [self.popover dismissPopoverAnimated:YES];
    } else {
        NSURL *shareURL = [NSURL URLWithString:[NSString stringWithFormat:@"http://myvote.azurewebsites.net/#/pollResult/%d", self.pollSummary.pollID]];
        NSString *shareText = NSLocalizedString(@"Check out the results to this poll!", nil);
        NSArray *itemsToShare = @[shareText, shareURL];
        
        /**
         // see if this user owns this poll
         UIActivityViewController *activityVC = nil;
         if (self.poll.UserID == self.currentUser.UserId) {
         DeletePollActivity *deleteActivity = [[DeletePollActivity alloc] init];
         activityVC = [[UIActivityViewController alloc] initWithActivityItems:itemsToShare applicationActivities:@[deleteActivity]];
         } else {
         activityVC = [[UIActivityViewController alloc] initWithActivityItems:itemsToShare applicationActivities:nil];
         }*/
        
        // sharing services, not needed
        UIActivityViewController *activityVC = [[UIActivityViewController alloc] initWithActivityItems:itemsToShare applicationActivities:nil];
        activityVC.excludedActivityTypes = @[UIActivityTypeAddToReadingList,
                                             UIActivityTypeAirDrop,
                                             UIActivityTypeAssignToContact,
                                             UIActivityTypePostToFlickr,
                                             UIActivityTypePostToTencentWeibo,
                                             UIActivityTypePrint,
                                             UIActivityTypeSaveToCameraRoll,
                                             UIActivityTypeMail,
                                             UIActivityTypeMessage];
        
        // determine which idiom - iPad or iPhone to determine how we display the UIActivityViewController
        if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad) {
            // iPad
            if (!self.popover) {
                self.popover = [[UIPopoverController alloc] initWithContentViewController:activityVC];
                self.popover.delegate = self;
            }
            
            // present popover
            [self.popover presentPopoverFromBarButtonItem:sender permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
        } else {
            // iPhone
            [self presentViewController:activityVC animated:YES completion:nil];
        }
    }
}

- (IBAction)deletePoll:(id)sender
{
    // alert view asking to delete poll
    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Delete Poll", nil)
                                                    message:NSLocalizedString(@"Are you sure want to delete this poll?", nil)
                                                   delegate:self
                                          cancelButtonTitle:NSLocalizedString(@"Cancel", nil)
                                          otherButtonTitles:NSLocalizedString(@"Yes", nil), nil];
    
    [alert show];
}

#pragma mark - ReplyCommentDelegate

- (void)replyToComment:(Comment *)comment withCommentText:(NSString *)newCommentText
{
    // now post new comment
    if (newCommentText) {
        // create new comment
        Comment *newComment = [[Comment alloc] init];
        newComment.userID = self.currentUser.UserId;
        newComment.username = self.currentUser.Username;
        newComment.commentText = newCommentText;
        newComment.pollID = self.pollSummary.pollID;
        newComment.parentCommentID = comment.parentCommentID;
        newComment.usernameRepliedTo = comment.username;
        
        // post comment
        [self postComment:newComment];
    }
}

#pragma mark - UIAlertViewDelegate

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
    if (buttonIndex == 0) {
        NSLog(@"cancel");
    } else {
        NSLog(@"delete");
    }
}

#pragma mark - UITextFieldDelegate

- (BOOL)textField:(UITextField *)textField shouldChangeCharactersInRange:(NSRange)range replacementString:(NSString *)string
{
    // need to setup string based on if you're replying or not
    return YES;
}

- (BOOL)textFieldShouldReturn:(UITextField *)textField
{
    // return pressed on keyboard so post comment
    if ([textField.text stringByTrimmingLeadingWhitespace].length > 0) {
        [self postComment:[self createNewCommentForThisPoll]];
    }
    
    return YES;
}

#pragma mark - UIPopoverControllerDelegate

- (void)popoverControllerDidDismissPopover:(UIPopoverController *)popoverController
{
    self.popover = nil;
}

#pragma mark - IGChartViewDelegate

- (NSString *)chartView:(IGChartView *)chartView labelForAxis:(IGAxis *)axis withItem:(NSObject *)item
{
    // only want to modify x axis labels
    if ([axis isKindOfClass:[IGNumericXAxis class]]) {
        NSString *label = [NSString stringWithFormat:@"%@ %%", item.description];
        
        return label;
    } else {
        IGDataPoint *dataPoint = (IGDataPoint *)item;
        return dataPoint.label;
    }
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    // Return the number of sections.
    if (self.hasViewBeenSetup) {
        if (self.hasUserAnsweredPoll) {
            // user has answered this poll
            return MALSectionsForAnsweredPoll;
        } else {
            // user has not answered this poll
            return MALSectionsForUnansweredPoll;
        }
    } else {
        // have not yet decided what view to display
        return 0;
    }
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    // Return the number of rows in the section.
    if (self.hasViewBeenSetup) {
        if (self.hasUserAnsweredPoll) {
            // user has answered this poll
            if (section == MALStatSection) {
                return 1;
            } else if (section == MALCommentsSection) {
                return self.commentsArray.count;
            }
        } else {
            // user has not answered this poll
            // poll options + submit button
            if (section == MALResponseSection) {
                return self.response.pollOptions.count;
            } else {
                return 1; // 1 for submit button
            }
        }
    }
    
    return 0;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    // determine which cells to configure
    UITableViewCell *cell = nil;
    if (self.hasUserAnsweredPoll) {
        cell = [self configureAnsweredCellsWithTableView:tableView forIndexPath:indexPath];
    } else {
        cell = [self configureUnansweredCellsWithTableView:tableView forIndexPath:indexPath];
    }
    
    // return configured cell
    return cell;
}

- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    // row & section
    NSUInteger row = indexPath.row;
    NSUInteger section = indexPath.section;
    
    if (self.hasUserAnsweredPoll) {
        // stats row is fixed
        if (section == MALStatSection) {
            return MALStatsRowHeight;
        } else if (section == MALCommentsSection) {
            // comment cell - set based on the length of the comment text
            Comment *comment = self.commentsArray[row];
            CGFloat cellWidth = CGRectGetWidth(self.tableView.frame);
            CGRect boundingRect = [comment.commentText boundingRectWithSize:CGSizeMake(cellWidth- 2 * kSidePadding, CGFLOAT_MAX)
                                                                    options:NSStringDrawingUsesLineFragmentOrigin
                                                                 attributes:@{NSFontAttributeName : [UIFont systemFontOfSize:15.0]}
                                                                    context:nil];
            
            // height of username + comment + date
            CGFloat cellHeight = boundingRect.size.height + (kLabelPadding * 2) + (kSidePadding * 2);
            
            return cellHeight;
        }
    } else {
        // not answered this poll - all rows are the same size
        return MALUnansweredRowHeight;
    }
    
    return 0.0f;
}

- (CGFloat)tableView:(UITableView *)tableView heightForHeaderInSection:(NSInteger)section
{
    if (self.hasUserAnsweredPoll) {
        if (section == MALStatSection) {
            return MALSectionHeight;
        } else if (section == MALCommentsSection) {
            return MALSectionHeight;
        }
    } else {
        if (section == MALResponseSection) {
            return MALSectionHeight;
        }
    }
    
    return 15.0f;
}

- (UIView *)tableView:(UITableView *)tableView viewForHeaderInSection:(NSInteger)section
{
    // simple view with a label but need to increase label size
    UIView *sectionHeaderView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, CGRectGetWidth(tableView.frame), MALSectionHeight)];
    UILabel *label = [[UILabel alloc] initWithFrame:CGRectMake(MALPadding, 5, CGRectGetWidth(tableView.frame) - (2 * MALPadding), MALSectionHeight)];
    label.font = [UIFont boldSystemFontOfSize:20.0f];
    label.textColor = [UIColor blackColor];
    label.text = @"";
    
    if (self.hasUserAnsweredPoll) {
        if (section == MALStatSection) {
            label.text = NSLocalizedString(@"Thank you for voting. Here are the current results:", nil);
        } else if (section == MALCommentsSection) {
            label.text = NSLocalizedString(@"Comments", nil);
        }
    } else {
        if (section == MALResponseSection) {
            label.text = NSLocalizedString(@"Vote:", nil);
        } else {
            label.text = @"";
        }
    }
    
    // add label to view
    [sectionHeaderView addSubview:label];
    
    return sectionHeaderView;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    // determine which type of view we are dealing with
    if (!self.hasUserAnsweredPoll) {
        // user has not answered poll
        if (indexPath.section == MALResponseSection) {
            // show checkmark on selected cell
            if (self.allowsMultiSelection) {
                UITableViewCell *cell = nil;
                // use mutable set to keep track of rows selected
                if ([self.selectedRows containsObject:indexPath]) {
                    // row is selected so remove checkmark
                    cell = [tableView cellForRowAtIndexPath:indexPath];
                    cell.accessoryType = UITableViewCellAccessoryNone;
                    [self.selectedRows removeObject:indexPath];
                } else {
                    // add checkmark to this row
                    cell = [tableView cellForRowAtIndexPath:indexPath];
                    cell.accessoryType = UITableViewCellAccessoryCheckmark;
                    [self.selectedRows addObject:indexPath];
                }
                
            } else if (self.lastSelectedRow != indexPath.row) {
                UITableViewCell *cell = nil;
                if (self.lastSelectedRow > -1) {
                    // remove checkmark on
                    NSIndexPath *ip = [NSIndexPath indexPathForRow:self.lastSelectedRow inSection:0];
                    cell = [tableView cellForRowAtIndexPath:ip];
                    cell.accessoryType = UITableViewCellAccessoryNone;
                    
                    // remove index for set
                    [self.selectedRows removeAllObjects];
                }
                
                cell = [tableView cellForRowAtIndexPath:indexPath];
                cell.accessoryType = UITableViewCellAccessoryCheckmark;
                self.lastSelectedRow = indexPath.row;
                
                // add index for set
                [self.selectedRows addObject:indexPath];
            } else {
                // remove checkmark from already selected cell
                UITableViewCell *cell = [tableView cellForRowAtIndexPath:indexPath];
                cell.accessoryType = UITableViewCellAccessoryNone;
                self.lastSelectedRow = -1;
                
                // remove index for set
                [self.selectedRows removeAllObjects];
            }
        } else {
            // submit button has been tapped - check set of answers
            if (self.selectedRows.count > 0) {
                // answers have been selected so submit
                [self submitAnswer];
            }
        }
    }
    
    // deselect
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}

#pragma mark - Cell Configuration

- (UITableViewCell *)configureAnsweredCellsWithTableView:(UITableView *)tableView forIndexPath:(NSIndexPath *)indexPath
{
    static NSString *ChartCellIdentifier = @"Chart Cell";
    static NSString *CommentCellIdentifier = @"Comment Cell";
    
    // section
    if (indexPath.section == MALStatSection) {
        UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:ChartCellIdentifier forIndexPath:indexPath];
        [cell.contentView addSubview:self.chart];
        
        return cell;
    } else {
        // comments section
        CommentCell *cell = [tableView dequeueReusableCellWithIdentifier:CommentCellIdentifier forIndexPath:indexPath];
        
        // current comment
        Comment *comment = self.commentsArray[indexPath.row];
        cell.dateLabel.text = [NSString stringWithFormat:@"%@   ●", comment.commentDate];
        
        // comment text
        cell.commentLabel.attributedText =  [comment attributedStringForCommentText];
        
        // reply action
        [cell.replyButton addTarget:self action:@selector(replyToComment:) forControlEvents:UIControlEventTouchUpInside];
        
        return cell;
    }
}

- (UITableViewCell *)configureUnansweredCellsWithTableView:(UITableView *)tableView forIndexPath:(NSIndexPath *)indexPath
{
    static NSString *ResponseCellIdentifier = @"Response Cell";
    static NSString *SubmitCellIdentifier = @"Submit Cell";
    
    // section
    if (indexPath.section == MALResponseSection) {
        // poll options
        PollOption *currentOption = self.response.pollOptions[indexPath.row];
        
        // setup cell
        UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:ResponseCellIdentifier forIndexPath:indexPath];
        cell.textLabel.text = currentOption.optionText;
        
        return cell;
    } else {
        // submit section
        UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:SubmitCellIdentifier forIndexPath:indexPath];
        cell.textLabel.text = NSLocalizedString(@"Submit Vote", nil);
        
        return cell;
    }
}

@end
