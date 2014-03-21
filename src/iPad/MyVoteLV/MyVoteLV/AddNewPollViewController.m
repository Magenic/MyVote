//
//  AddNewPollViewController.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/7/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "AddNewPollViewController.h"
#import "GenericSelectionPickerController.h"
#import "DatePickerViewController.h"

// helpers
#import "NSData+Base64.h"
#import "MyVoteService.h"
#import "SVProgressHUD.h"
#import <AssetsLibrary/AssetsLibrary.h>

// sections
static NSUInteger const MALNumberOfSections = 3;
static NSUInteger const MALQuestionAndAnswerSection = 0;
static NSUInteger const MALPollDetailSection = 1;
static NSUInteger const MALSubmitPollSection = 2;

// rows
static NSUInteger const MALQuestionAndAnswerRows = 7;
static NSUInteger const MALPollDetailRows = 3;
static NSUInteger const MALSubmitPollRows = 1;
static NSUInteger const MALSelectACategoryRow = 0;
static NSUInteger const MALSelectAnEndDateRow = 1;
static NSUInteger const MALAddAnImageRow = 2;
static NSUInteger const MALQuestionRow = 0;
static NSUInteger const MALAnswerOneRow = 1;
static NSUInteger const MALAnswerTwoRow = 2;
static NSUInteger const MALAnswerThreeRow = 3;
static NSUInteger const MALAnswerFourRow = 4;
static NSUInteger const MALAnswerFiveRow = 5;
static NSUInteger const MALMultiSelectionRow = 6;

// row height
static CGFloat const MALDefaultRowHeight = 60.0f;

// section height
static CGFloat const MALSectionHeightWithText = 50.0f;
static CGFloat const MALSectionHeightWithoutText = 20.0f;

// padding
static CGFloat const MALPadding = 15.0f;

// view tags
static NSUInteger const MALTextFieldTag = 99;
static NSUInteger const MALSwitchTag = 100;

@interface AddNewPollViewController () <UITextFieldDelegate, UINavigationBarDelegate, UIPopoverControllerDelegate, UIImagePickerControllerDelegate, UINavigationControllerDelegate, GenericSelectionPickerDelegate, DatePickerDelegate>

// textfields
@property (weak, nonatomic) UITextField *questionTextField;
@property (weak, nonatomic) UITextField *answerOneTextField;
@property (weak, nonatomic) UITextField *answerTwoTextField;
@property (weak, nonatomic) UITextField *answerThreeTextField;
@property (weak, nonatomic) UITextField *answerFourTextField;
@property (weak, nonatomic) UITextField *answerFiveTextField;

// dates
@property (strong, nonatomic) UIPopoverController *expirationDatePopover;
@property (strong, nonatomic) NSDate *pollStartDate;
@property (strong, nonatomic) NSDate *pollEndDate;
@property (strong, nonatomic) NSString *pollStartEndDateString;

@property BOOL allowMultiSelection;

// image
@property (strong, nonatomic) UIPopoverController *imagePickerPopover;
@property (strong, nonatomic) NSString *pollImageLink;
@property (strong, nonatomic) UIImage *selectedImage;

// category
@property (strong, nonatomic) UIPopoverController *categoryPickerPopover;
@property (strong, nonatomic) NSArray *categoryArray;
@property NSInteger selectedCategory;

@end

@implementation AddNewPollViewController

#pragma mark - View Lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    // init
    self.allowMultiSelection = NO;
    
    // category initiation
    self.categoryArray = @[NSLocalizedString(@"Fun", nil), NSLocalizedString(@"Technology", nil), NSLocalizedString(@"Entertainment", nil),
                           NSLocalizedString(@"News", nil), NSLocalizedString(@"Sports", nil), NSLocalizedString(@"Off-Topic", nil)];
    
    self.selectedCategory = -1;
    
    // init string
    self.pollImageLink = @"";
}

#pragma mark - Poll Creation

- (void)createPoll
{
    // verify and review poll then dismiss view controller
    if ([self isPollValid]) {
        [SVProgressHUD showWithStatus:@"Creating your poll!" maskType:SVProgressHUDMaskTypeBlack];
        if (self.selectedImage) {
            [self uploadImage:self.selectedImage];
        } else {
            [self setupAndSubmitPoll];
        }
    }
}

- (BOOL)isPollValid
{
    // first check if question is there
    if (self.questionTextField.text.length == 0) {
        // invalid question
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Invalid Question", nil)
                                                        message:NSLocalizedString(@"You must enter a question.", nil)
                                                       delegate:nil
                                              cancelButtonTitle:NSLocalizedString(@"OK", nil)
                                              otherButtonTitles:nil];
        [alert show];
        
        return NO;
    } else if (self.answerOneTextField.text.length == 0 || self.answerTwoTextField.text.length == 0) {
        // answer 1 and 2 both required
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Invalid Answers", nil)
                                                        message:NSLocalizedString(@"You must enter at least two answers.", nil)
                                                       delegate:nil
                                              cancelButtonTitle:NSLocalizedString(@"OK", nil)
                                              otherButtonTitles:nil];
        [alert show];
        
        return NO;
    } else if (self.selectedCategory == -1) {
        // category required
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Invalid Category", nil)
                                                        message:NSLocalizedString(@"You must select a category.", nil)
                                                       delegate:nil
                                              cancelButtonTitle:NSLocalizedString(@"OK", nil)
                                              otherButtonTitles:nil];
        [alert show];
        
        return NO;
    }

    return YES;
}

- (NSArray *)setupAnswersArray
{
    // need to create answers
    NSDictionary *answer1 = @{@"PollOptionID": @1,
                              @"PollID": @1,
                              @"OptionPosition": @1,
                              @"OptionText": self.answerOneTextField.text};
    
    NSDictionary *answer2 = @{@"PollOptionID": @1,
                              @"PollID": @1,
                              @"OptionPosition": @2,
                              @"OptionText": self.answerTwoTextField.text};
    
    NSMutableArray *answersArray = [[NSMutableArray alloc] initWithObjects:answer1,answer2, nil];
    
    // setup option answers
    
    // init answers textfields array
    NSArray *optionalAnswersTextFieldsArray = @[self.answerThreeTextField, self.answerFourTextField, self.answerFiveTextField];
    
    NSUInteger optionPosition = 3;
    for (UITextField *currentTextField in optionalAnswersTextFieldsArray) {
        if (currentTextField.text.length > 0) {
            // create dictionary
            NSDictionary *optionalAnswer = @{@"PollOptionID" : @1,
                                             @"PollID" : @1,
                                             @"OptionPosition" : [NSNumber numberWithInt:optionPosition++],
                                             @"OptionText" : currentTextField.text};
            [answersArray addObject:optionalAnswer];
        }
    }
    
    return [answersArray copy];
}


- (void)setupAndSubmitPoll
{
    /**
     {
     "PollID": 1,
     "UserID": 2,
     "PollCategoryID": 3,
     "PollQuestion": "sample string 4",
     "PollImageLink": "sample string 5",
     "PollMaxAnswers": 6,
     "PollMinAnswers": 7,
     "PollStartDate": "2013-03-06T13:28:08.7367545+00:00",
     "PollEndDate": "2013-03-06T13:28:08.7367545+00:00",
     "PollAdminRemovedFlag": true,
     "PollDateRemoved": "2013-03-06T13:28:08.7367545+00:00",
     "PollDeletedFlag": true,
     "PollDeletedDate": "2013-03-06T13:28:08.7367545+00:00",
     "PollOptions": [
     {
     "PollOptionID": 1,
     "PollID": 1,
     "OptionPosition": 1,
     "OptionText": "sample string 1"
     },
     {
     "PollOptionID": 1,
     "PollID": 1,
     "OptionPosition": 1,
     "OptionText": "sample string 1"
     },
     {
     "PollOptionID": 1,
     "PollID": 1,
     "OptionPosition": 1,
     "OptionText": "sample string 1"
     }
     ]
     }
     */
    
    // setup dates
    NSDate *startDate = [NSDate date];
    NSDate *endDate = [NSDate date];
    NSDateComponents *dateComponents = [[NSDateComponents alloc] init];
    [dateComponents setMonth:6];
    NSCalendar *calendar = [NSCalendar currentCalendar];
    NSDate *newDate = [calendar dateByAddingComponents:dateComponents toDate:endDate options:0];
    
    // check if user supplied start and end date
    if (self.pollStartDate && self.pollEndDate) {
        startDate = self.pollStartDate;
        endDate = self.pollEndDate;
    }
    
    // format dates
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
    [dateFormatter setDateFormat:@"MMM d, yyyy, h:mm a"];
    NSString *startDateString = [dateFormatter stringFromDate:startDate];
    NSString *endDateString = [dateFormatter stringFromDate:newDate];
    
    NSArray *answers = [self setupAnswersArray];
    
    NSNumber *maxAnswers = @1;
    if (self.allowMultiSelection) {
        maxAnswers = [NSNumber numberWithInteger:answers.count];
    }
    
    NSDictionary *pollDictionary = @{@"PollID" : @"",
                                     @"UserID" : @(self.userID),
                                     @"PollCategoryID" : @(self.selectedCategory),
                                     @"PollQuestion": self.questionTextField.text,
                                     @"PollImageLink": self.pollImageLink,
                                     @"PollMaxAnswers": maxAnswers,
                                     @"PollMinAnswers": @1,
                                     @"PollStartDate": startDateString,
                                     @"PollEndDate": endDateString,
                                     @"PollOptions": answers};
    
    // submit poll via API
    [[MALVoteClient sharedInstance] addPoll:pollDictionary withCompletion:^(BOOL success) {
        // check success
        if (success) {
            // dismiss HUD
            [SVProgressHUD showSuccessWithStatus:NSLocalizedString(@"Your poll was successfully created!", nil)];
            
            // close the view
            [self.navigationController popViewControllerAnimated:YES];
        } else {
            // dismiss HUD
            [SVProgressHUD showErrorWithStatus:@"Oops! An error occurred when creating your poll!"];
            
            /**
            // display alert
            UIAlertView *alert = [[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Failed to Add Poll!", nil)
                                                            message:NSLocalizedString(@"An error occurred when adding your poll. Please try again.", nil)
                                                           delegate:nil
                                                  cancelButtonTitle:NSLocalizedString(@"OK", nil)
                                                  otherButtonTitles:nil];
            [alert show];
             */
        }
    }];
}

#pragma mark - Image Handling

- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info
{
    self.selectedImage = [info valueForKey:UIImagePickerControllerOriginalImage];
    [self.imagePickerPopover dismissPopoverAnimated:YES];
}

- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker
{
    [self.imagePickerPopover dismissPopoverAnimated:YES];
}

- (void)showImagePicker:(UIImagePickerControllerSourceType)sourceType withSender:(UITableViewCell *)sender
{
    // hack for iOS 7 crash when user selects ok to allow photo access for the first time crashes app
    // http://stackoverflow.com/questions/18939537/uiimagepickercontroller-crash-only-on-ios-7-ipad
    if ([UIImagePickerController isSourceTypeAvailable:sourceType]) {
        void(^blk)() =  ^() {
            UIImagePickerController *imagePicker = [[UIImagePickerController alloc] init];
            imagePicker.delegate = self;
            imagePicker.sourceType = sourceType;
            if (self.imagePickerPopover == nil) {
                self.imagePickerPopover = [[UIPopoverController alloc] initWithContentViewController:imagePicker];
                self.imagePickerPopover.delegate = self;
            }
            
            [self.imagePickerPopover presentPopoverFromRect:sender.frame inView:sender.superview permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
        };
        
        // Make sure we have permission, otherwise request it first
        ALAssetsLibrary *assetsLibrary = [[ALAssetsLibrary alloc] init];
        ALAuthorizationStatus authStatus = [ALAssetsLibrary authorizationStatus];

        
        if (authStatus == ALAuthorizationStatusAuthorized) {
            blk();
        } else if (authStatus == ALAuthorizationStatusDenied || authStatus == ALAuthorizationStatusRestricted) {
            // show alert view needing access to photos
            [self showAlertForAccessToPhotos];

        } else if (authStatus == ALAuthorizationStatusNotDetermined) {
            [assetsLibrary enumerateGroupsWithTypes:ALAssetsGroupAll usingBlock:^(ALAssetsGroup *group, BOOL *stop) {
                // Catch the final iteration, ignore the rest
                if (group == nil)
                    dispatch_async(dispatch_get_main_queue(), ^{
                        blk();
                    });
                *stop = YES;
            } failureBlock:^(NSError *error) {
                // failure :(
                dispatch_async(dispatch_get_main_queue(), ^{
                    // show alert view needing access to photos
                    [self showAlertForAccessToPhotos];
                });
            }];
        }
    }
}

- (void)showAlertForAccessToPhotos
{
    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Photo Access Denied", nil)
                                                    message:NSLocalizedString(@"To change this: Settings > Privacy > Photos", nil)
                                                   delegate:nil
                                          cancelButtonTitle:NSLocalizedString(@"OK", nil)
                                          otherButtonTitles:nil];
    
    [alert show];
}

- (void)uploadImage:(UIImage *)image
{
    // setup image dictionary to upload to Azure
    NSData *imageData = UIImagePNGRepresentation(image);
    NSString *dataString = nil;
    dataString = [imageData base64EncodedString];
    NSDictionary *activeUserDictionary = @{@"containerName" : @"pollpictures",
                                           @"resourceName" : dataString};
    
    [[MyVoteService sharedInstance] addImage:activeUserDictionary completion:^(NSInteger index) {
        // add image
        self.pollImageLink = [NSString stringWithFormat:@"%d", index];
        
        // save link to image here and save poll
        [self setupAndSubmitPoll];
    }];
}

- (void)switchFlipped:(UISwitch *)sender
{
    self.allowMultiSelection = sender.isOn;
}

#pragma mark - Poll Expiration

- (void)displayDateExpirationPopover:(UITableViewCell *)cell
{
    // setup control and view containing date picker
    if (!self.expirationDatePopover) {
        DatePickerViewController *picker = [self.storyboard instantiateViewControllerWithIdentifier:@"Add Poll Date Picker"];
        picker.delegate = self;
        self.expirationDatePopover = [[UIPopoverController alloc] initWithContentViewController:picker];
        self.expirationDatePopover.delegate = self;
    }
    
    // display popover controller
    [self.expirationDatePopover presentPopoverFromRect:cell.frame inView:cell.superview permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
}

- (void)datePickerStartDate:(NSDate *)startDate andEndDate:(NSDate *)endDate
{
    self.pollStartDate = startDate;
    self.pollEndDate = endDate;
    
    // set something on cell that dates have been selected
    NSIndexPath *indexPath = [NSIndexPath indexPathForRow:MALSelectAnEndDateRow inSection:MALPollDetailSection];
    [self.tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    if (self.pollStartDate && self.pollEndDate) {
        // set label based on dates
        NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
        [dateFormatter setDateStyle:NSDateFormatterShortStyle];
        [dateFormatter setTimeStyle:NSDateFormatterNoStyle];
        
        // string dates
        NSString *start = [dateFormatter stringFromDate:self.pollStartDate];
        NSString *end = [dateFormatter stringFromDate:self.pollEndDate];
        
        self.pollStartEndDateString = [NSString stringWithFormat:@"%@ - %@", start, end];
    } else {
        self.pollStartEndDateString = nil;
    }
    
    // update date row only
    [self.tableView reloadRowsAtIndexPaths:@[indexPath] withRowAnimation:UITableViewRowAnimationAutomatic];
    
    // dismiss popover
    [self.expirationDatePopover dismissPopoverAnimated:YES];
}

#pragma mark - Category Selection

- (void)displayCategoryPickerPopover:(UITableViewCell *)cell
{
    // setup control and view containing date picker
    if (!self.categoryPickerPopover) {
        GenericSelectionPickerController *picker = [[GenericSelectionPickerController alloc] initWithStyle:UITableViewStylePlain];
        picker.delegate = self;
        picker.itemsArray = self.categoryArray;
        [picker.tableView setScrollEnabled:NO];
        self.categoryPickerPopover = [[UIPopoverController alloc] initWithContentViewController:picker];
        self.categoryPickerPopover.delegate = self;
    }
    
    // display popover controller
    [self.categoryPickerPopover presentPopoverFromRect:cell.frame inView:cell.superview permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
}

#pragma mark - GenericSelectionDelegate

- (void)itemSelected:(NSInteger)index
{
    self.selectedCategory = index + 1;
    
    [self.categoryPickerPopover dismissPopoverAnimated:YES];

    // deselect row
    NSIndexPath *indexPath = [NSIndexPath indexPathForRow:MALSelectACategoryRow inSection:MALPollDetailSection];
    [self.tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    // update category row only
    [self.tableView reloadRowsAtIndexPaths:@[indexPath] withRowAnimation:UITableViewRowAnimationAutomatic];
}

#pragma mark - UITextFieldDelegate

- (BOOL)textFieldShouldReturn:(UITextField *)textField
{
    if (textField == self.questionTextField) {
        [self.answerOneTextField becomeFirstResponder];
    } else if (textField == self.answerOneTextField) {
        [self.answerTwoTextField becomeFirstResponder];
    } else if (textField == self.answerTwoTextField) {
        [self.answerThreeTextField becomeFirstResponder];
    } else if (textField == self.answerThreeTextField ) {
        [self.answerFourTextField becomeFirstResponder];
    } else if (textField == self.answerFourTextField) {
        [self.answerFiveTextField becomeFirstResponder];
    } else if (textField == self.answerFiveTextField) {
        // dismiss keyboard
        [self.answerFiveTextField resignFirstResponder];
    }
    
    return YES;
}

- (void)dismissKeyboard
{
    // resign keyboards
    [self.questionTextField resignFirstResponder];
    [self.answerOneTextField resignFirstResponder];
    [self.answerTwoTextField resignFirstResponder];
    [self.answerThreeTextField resignFirstResponder];
    [self.answerFourTextField resignFirstResponder];
    [self.answerFiveTextField resignFirstResponder];
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    // Return the number of sections.
    return MALNumberOfSections;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if (section == MALQuestionAndAnswerSection) {
        return MALQuestionAndAnswerRows;
    } else if (section == MALPollDetailSection) {
        return MALPollDetailRows;
    } else if (section == MALSubmitPollSection) {
        return MALSubmitPollRows;
    }
    
    return 0;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString *TextFieldCellIdentifier = @"TextField Cell";
    static NSString *CellIdentifier = @"Cell";
    static NSString *SubmitCellIdentifier = @"Submit Cell";
    static NSString *SwitchCellIdentifiter = @"Switch Cell";
    
    // configure cell
    if (indexPath.section == MALQuestionAndAnswerSection) {
        if (indexPath.row == MALMultiSelectionRow) {
            UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:SwitchCellIdentifiter forIndexPath:indexPath];
            UISwitch *multiSelectionSwitch = (UISwitch *)[cell viewWithTag:MALSwitchTag];
            [multiSelectionSwitch addTarget:self action:@selector(switchFlipped:) forControlEvents:UIControlEventValueChanged];
            
            return cell;
        } else {
            UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:TextFieldCellIdentifier forIndexPath:indexPath];
            
            // set placeholder for textfield
            UITextField *textField = (UITextField *)[cell viewWithTag:MALTextFieldTag];
            
            if (indexPath.row == MALQuestionRow) {
                self.questionTextField = textField;
                textField.placeholder = NSLocalizedString(@"Ask your question here*", nil);
            } else if (indexPath.row == MALAnswerOneRow) {
                self.answerOneTextField = textField;
                textField.placeholder = NSLocalizedString(@"Answer #1*", nil);
            } else if (indexPath.row == MALAnswerTwoRow) {
                self.answerTwoTextField = textField;
                textField.placeholder = NSLocalizedString(@"Answer #2*", nil);
            } else if (indexPath.row == MALAnswerThreeRow) {
                self.answerThreeTextField = textField;
                textField.placeholder = NSLocalizedString(@"Answer #3", nil);
            } else if (indexPath.row == MALAnswerFourRow) {
                self.answerFourTextField = textField;
                textField.placeholder = NSLocalizedString(@"Answer #4", nil);
            } else if (indexPath.row == MALAnswerFiveRow) {
                self.answerFiveTextField = textField;
                textField.placeholder = NSLocalizedString(@"Answer #5", nil);
            }
            
            return cell;
        }
    } else if (indexPath.section == MALPollDetailSection) {
        UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier forIndexPath:indexPath];
        cell.detailTextLabel.text = @"";
        
        if (indexPath.row == MALSelectACategoryRow) {
            cell.textLabel.text = NSLocalizedString(@"Select a Category*", nil);
            if (self.selectedCategory != -1) {
                cell.detailTextLabel.text = self.categoryArray[self.selectedCategory - 1];
            }
        } else if (indexPath.row == MALSelectAnEndDateRow) {
            cell.textLabel.text = NSLocalizedString(@"Select a Start and End Date?", nil);
            if (self.pollStartEndDateString) {
                cell.detailTextLabel.text = self.pollStartEndDateString;
            } else {
                cell.detailTextLabel.text = @"";
            }
        } else if (indexPath.row == MALAddAnImageRow) {
            cell.textLabel.text = NSLocalizedString(@"Add an Image?", nil);
        }
        
        return cell;
    } else {
        // submit row
        UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:SubmitCellIdentifier forIndexPath:indexPath];
        
        return cell;
    }
}

- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    return MALDefaultRowHeight;
}

- (CGFloat)tableView:(UITableView *)tableView heightForHeaderInSection:(NSInteger)section
{
    if (section == MALQuestionAndAnswerSection) {
        return MALSectionHeightWithText;
    } else {
        return MALSectionHeightWithoutText;
    }
}

- (UIView *)tableView:(UITableView *)tableView viewForHeaderInSection:(NSInteger)section
{
    if (section == MALQuestionAndAnswerSection) {
        // simple view with a label but need to increase label size
        UIView *sectionHeaderView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, CGRectGetWidth(tableView.frame), MALSectionHeightWithText)];
        UILabel *label = [[UILabel alloc] initWithFrame:CGRectMake(MALPadding, 5, CGRectGetWidth(tableView.frame) - (2 * MALPadding), MALSectionHeightWithText)];
        label.font = [UIFont boldSystemFontOfSize:20.0f];
        label.textColor = [UIColor blackColor];
        label.text = NSLocalizedString(@"Create a New Poll", nil);
        
        // add label to view
        [sectionHeaderView addSubview:label];
        
        return sectionHeaderView;
    } else if (section == MALSubmitPollSection) {
        // simple view with a label
        UIView *sectionHeaderView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, CGRectGetWidth(tableView.frame), MALSectionHeightWithoutText)];
        UILabel *label = [[UILabel alloc] initWithFrame:CGRectMake(MALPadding, 0, CGRectGetWidth(tableView.frame) - (2 * MALPadding), 12)];
        label.autoresizingMask = UIViewAutoresizingFlexibleWidth;
        label.font = [UIFont systemFontOfSize:13.0f];
        label.textAlignment = NSTextAlignmentCenter;
        label.textColor = [UIColor darkGrayColor];
        label.text = NSLocalizedString(@"*Required Fields", nil);
        
        // add label to view
        [sectionHeaderView addSubview:label];
        
        return sectionHeaderView;
    } else {
        return [[UIView alloc] init];
    }
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    // resign keyboard if it's showing
    [self dismissKeyboard];
    
    if (indexPath.section == MALPollDetailSection) {
        // get selected cell
        UITableViewCell *selectedCell = [tableView cellForRowAtIndexPath:indexPath];
        
        if (indexPath.row == MALAddAnImageRow) {
            // display popover of saved images
            if ([[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPad) {
                // must display in popover
                [self showImagePicker:UIImagePickerControllerSourceTypeSavedPhotosAlbum withSender:selectedCell];
            } else {
                // display as modal view?
            }
        } else if (indexPath.row == MALSelectACategoryRow) {
            [self displayCategoryPickerPopover:selectedCell];
        } else if (indexPath.row == MALSelectAnEndDateRow) {
            [self displayDateExpirationPopover:selectedCell];
        }
    } else if (indexPath.section == MALSubmitPollSection) {
        // create poll
        [self createPoll];
    }
    
    // deselect
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}

@end
