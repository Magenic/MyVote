//
//  ProfileViewController.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/8/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "ProfileViewController.h"
#import "GenericSelectionPickerController.h"
#import "User.h"
#import "NSString+Convenience.h"
#import "PDKeychainBindings.h"
#import "MyVoteService.h"
#import "SVProgressHUD.h"

// sections
static NSUInteger const MALNumberOfSections = 2;
static NSUInteger const MALInformationSection = 0;
static NSUInteger const MALSubmitSection = 1;

// rows for information section
static NSUInteger const MALNumberOfInformationRows = 7;
static NSUInteger const MALFirstNameRow = 0;
static NSUInteger const MALLastNameRow = 1;
static NSUInteger const MALEmailRow = 2;
static NSUInteger const MALUsernameRow = 3;
static NSUInteger const MALDateOfBirthRow = 4;
static NSUInteger const MALGenderRow = 5;
static NSUInteger const MALZipCodeRow = 6;

// rows for submit sectioin
static NSUInteger const MALNumberOfSubmitRows = 1;
static NSUInteger const MALSubmitRow = 0;

// view tags
static NSUInteger const MALTextFieldTag = 99;
static NSUInteger const MALLabelTag = 100;

@interface ProfileViewController () <UITextFieldDelegate, UIPopoverControllerDelegate, GenericSelectionPickerDelegate>

@property (copy, nonatomic) NSArray *informationRowLabels;
@property BOOL creatingNewUser;

// textfields
@property (weak, nonatomic) UITextField *firstNameTextField;
@property (weak, nonatomic) UITextField *lastNameTextField;
@property (weak, nonatomic) UITextField *emailTextField;
@property (weak, nonatomic) UITextField *usernameTextField;
@property (weak, nonatomic) UITextField *zipCodeTextField;

// labels
@property (weak, nonatomic) UILabel *genderLabel;
@property (weak, nonatomic) UILabel *dateOfBirthLabel;

// date popover
@property (strong, nonatomic) UIPopoverController *thePopoverController;
@property (strong, nonatomic) NSDateFormatter *dateFormatter;
@property (strong, nonatomic) NSDate *selectedBirthDate;

// gender popover
@property (strong, nonatomic) GenericSelectionPickerController *genderPickerPopover;
@property (strong, nonatomic) UIPopoverController *genderPopoverController;
@property (strong, nonatomic) NSArray *genderChoices;
@property BOOL isGenderSelected;

@end

@implementation ProfileViewController

#pragma mark - View Lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    // setup labels
    self.informationRowLabels = @[NSLocalizedString(@"First Name", nil), NSLocalizedString(@"Last Name", nil), NSLocalizedString(@"Email", nil),
                                  NSLocalizedString(@"Username", nil), NSLocalizedString(@"Date of Birth", nil), NSLocalizedString(@"Gender", nil),
                                  NSLocalizedString(@"Zip Code", nil)];
    
    // gender choices
    self.isGenderSelected = NO;
    self.genderChoices = @[@"Male", @"Female"];
    
    // date formatter for birth date
    self.dateFormatter = [[NSDateFormatter alloc] init];
    [self.dateFormatter setDateStyle:NSDateFormatterLongStyle];
    
    // date selected if user is set
    if (self.user) {
        self.selectedBirthDate = self.user.dateOfBirth;
        self.creatingNewUser = NO;
        self.isGenderSelected = YES;
    } else {
        self.creatingNewUser = YES;
    }
}

#pragma mark - Date Picker Methods

- (void)dateSelected:(UIDatePicker *)datePicker
{
    // set date locally first
    self.selectedBirthDate = datePicker.date;
    
    // setup label
    self.dateOfBirthLabel.textColor = [UIColor blackColor];
    self.dateOfBirthLabel.alpha = 1.0;
    self.dateOfBirthLabel.text = [self.dateFormatter stringFromDate:datePicker.date];
}

- (void)displayDatePickerPopover:(UITableViewCell *)sender
{
    // setup date picker
    UIDatePicker *datePicker = [[UIDatePicker alloc] init];
    datePicker.datePickerMode = UIDatePickerModeDate;
    
    // set date
    if (self.selectedBirthDate) {
        datePicker.date = self.selectedBirthDate;
    } else {
        datePicker.date = [NSDate date];
    }
    
    // add target action for date picker
    [datePicker addTarget:self action:@selector(dateSelected:) forControlEvents:UIControlEventValueChanged];
    
    // setup popover content and controller
    UIViewController *popoverContent = [[UIViewController alloc] init];
    popoverContent.view = datePicker;
    self.thePopoverController = [[UIPopoverController alloc] initWithContentViewController:popoverContent];
    self.thePopoverController.delegate = self;
    [self.thePopoverController setPopoverContentSize:CGSizeMake(CGRectGetWidth(datePicker.frame), CGRectGetHeight(datePicker.frame)) animated:YES];
    [self.thePopoverController presentPopoverFromRect:sender.frame inView:self.tableView permittedArrowDirections:UIPopoverArrowDirectionUp animated:YES];
}

#pragma mark - Gender Picker

- (void)itemSelected:(NSInteger)index
{
    // set bool
    self.isGenderSelected = YES;
    
    // set gender label on cell
    self.genderLabel.textColor = [UIColor blackColor];
    self.genderLabel.alpha = 1.0;
    self.genderLabel.text = self.genderChoices[index];
    
    // deselect row
    NSIndexPath *ip = [self.tableView indexPathForSelectedRow];
    [self.tableView deselectRowAtIndexPath:ip animated:YES];
    
    //dismiss popover controller
    [self.thePopoverController dismissPopoverAnimated:YES];
}

- (void)displayGenderPickerPopover:(UITableViewCell *)sender
{
    // setup control and view containing date picker
    GenericSelectionPickerController *genderPickerPopover = [[GenericSelectionPickerController alloc] initWithStyle:UITableViewStylePlain];
    genderPickerPopover.delegate = self;
    genderPickerPopover.itemsArray = self.genderChoices;
    [genderPickerPopover.tableView setScrollEnabled:NO];
    self.thePopoverController = [[UIPopoverController alloc] initWithContentViewController:genderPickerPopover];
    self.thePopoverController.delegate = self;
    
    // display popover controller
    [self.thePopoverController presentPopoverFromRect:sender.frame inView:self.tableView permittedArrowDirections:UIPopoverArrowDirectionUp animated:YES];
}

#pragma mark - UIPopoverControllerDelegate

- (BOOL)popoverControllerShouldDismissPopover:(UIPopoverController *)popoverController
{
    NSIndexPath *ip = [self.tableView indexPathForSelectedRow];
    [self.tableView deselectRowAtIndexPath:ip animated:YES];
    
    return YES;
}

#pragma mark - Create/Update Profile

- (void)saveProfile
{
    // create a user to return
    User *userObject = [self createUser];
    
    if ([self isUserValid:userObject]) {
        
        // create response and send information
        MyVoteService *service = [MyVoteService sharedInstance];
        NSString *profileID = service.client.currentUser.userId;
        NSString *accessToken = service.client.currentUser.mobileServiceAuthenticationToken;
        
        NSString *gender = userObject.Gender;
        if ([gender isEqualToString:NSLocalizedString(@"Male", nil)]) {
            gender = @"M";
        } else {
            gender = @"F";
        }
        
        NSDictionary *user = @{@"ProfileID" : profileID,
                               @"ProfileAuthToken" : profileID,
                               @"UserName" : userObject.Username,
                               @"FirstName" : userObject.FirstName,
                               @"LastName" : userObject.LastName,
                               @"PostalCode" : userObject.PostalCode,
                               @"Gender" : gender,
                               @"EmailAddress" : userObject.EmailAddress,
                               @"BirthDate" : self.selectedBirthDate.description};
        
        // show HUD
        [SVProgressHUD show];
        
        // set authentication web API
        [[MALVoteClient sharedInstance] setAuthorizationHeaderWithToken:accessToken];
        
        // see if we are creating a user or updating one
        if (self.creatingNewUser) {
            [self addUser:userObject withUserDictionary:user];
        } else {
            [self updateUser:userObject withUserDictionary:user];
        }
    }
}

- (void)addUser:(User *)user withUserDictionary:(NSDictionary *)userDictionary
{
    MyVoteService *service = [MyVoteService sharedInstance];
    NSString *profileID = service.client.currentUser.userId;
    NSString *accessToken = service.client.currentUser.mobileServiceAuthenticationToken;
    
    // create user
    [[MALVoteClient sharedInstance] addUser:userDictionary withCompletion:^(BOOL success) {
        if (success) {
            // save user ID to keychain
            PDKeychainBindings *bindings = [PDKeychainBindings sharedKeychainBindings];
            [bindings setObject:profileID forKey:@"profileID"];
            [bindings setObject:accessToken forKey:@"accessToken"];
            
            // dismiss HUD
            [SVProgressHUD showSuccessWithStatus:@"New User Created!"];
            
            // call delegate
            [self.delegate updatedUser:user];
            
            // dismiss view controller
            [self dismissViewControllerAnimated:YES completion:nil];
        } else {
            // failed to create new user
            [SVProgressHUD showErrorWithStatus:@"Failed to create user!"];
        }
    }];
}

- (void)updateUser:(User *)user withUserDictionary:(NSDictionary *)userDictionary
{
    // save updated info
    MyVoteService *service = [MyVoteService sharedInstance];
    NSString *profileID = service.client.currentUser.userId;
    
    [[MALVoteClient sharedInstance] editUser:userDictionary withProfileID:profileID withCompletion:^(BOOL success) {
        if (success) {
            // call delegate with updated user
            [self.delegate updatedUser:user];
            
            // dismiss HUD
            [SVProgressHUD dismiss];
            
            // success
            [self.navigationController popViewControllerAnimated:YES];
        } else {
            // dismiss HUD
            [SVProgressHUD dismiss];
            
            // show error alert
            UIAlertView *alert = [[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Update User Profile Error!", nil)
                                                            message:NSLocalizedString(@"There was an error while updating your user profile. Please try again.", nil)
                                                           delegate:nil
                                                  cancelButtonTitle:NSLocalizedString(@"OK", nil)
                                                  otherButtonTitles:nil];
            [alert show];
        }
        
    }];
}

- (BOOL)isUserValid:(User *)user
{
    // all fields are mandatory - trim leading white space
    NSString *errorMessage = nil;
    
    // first name
    if (user.FirstName.length == 0) {
        errorMessage = NSLocalizedString(@"First name is required.", nil);
    }else if (user.LastName.length == 0) {
        errorMessage = NSLocalizedString(@"Last name is required.", nil);
    } else if (user.EmailAddress.length == 0) {
        errorMessage = NSLocalizedString(@"Email is required.", nil);
    } else if (user.Username.length == 0) {
        errorMessage = NSLocalizedString(@"Username is required.", nil);
    } else if (user.BirthDate.length == 0 || !self.selectedBirthDate) {
        errorMessage = NSLocalizedString(@"Date of birth is required.", nil);
    } else if (user.Gender.length == 0 || !self.isGenderSelected) {
        errorMessage = NSLocalizedString(@"Gender is required.", nil);
    } else if (user.PostalCode.length == 0) {
        errorMessage = NSLocalizedString(@"Zip code is required.", nil);
    }
    
    // show error message
    if (errorMessage) {
        // post erro message
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Invalid User"
                                                        message:errorMessage
                                                       delegate:nil
                                              cancelButtonTitle:@"OK"
                                              otherButtonTitles:nil];
        
        // show alert
        [alert show];
        
        // user is not valid
        return NO;
    }
    
    return YES;
}

- (User *)createUser
{
    User *user = [[User alloc] init];
    user.FirstName = [self.firstNameTextField.text stringByTrimmingLeadingWhitespace];
    user.LastName = [self.lastNameTextField.text stringByTrimmingLeadingWhitespace];
    user.EmailAddress = [self.emailTextField.text stringByTrimmingLeadingWhitespace];
    user.Username = [self.usernameTextField.text stringByTrimmingLeadingWhitespace];
    user.BirthDate = [self.dateOfBirthLabel.text stringByTrimmingLeadingWhitespace];
    user.dateOfBirth = self.selectedBirthDate;
    user.Gender = [self.genderLabel.text stringByTrimmingLeadingWhitespace];
    user.PostalCode = [self.zipCodeTextField.text stringByTrimmingLeadingWhitespace];
    
    return user;
}

#pragma mark - UITextFieldDelegate

- (BOOL)textFieldShouldReturn:(UITextField *)textField
{
    if (textField == self.firstNameTextField) {
        [self.lastNameTextField becomeFirstResponder];
    } else if (textField == self.lastNameTextField) {
        [self.emailTextField becomeFirstResponder];
    } else if (textField == self.emailTextField) {
        // username is not editable so if user already exists, skip textfield
        if (self.user) {
            [self.zipCodeTextField becomeFirstResponder];
        } else {
            [self.usernameTextField becomeFirstResponder];
        }
    } else if (textField == self.usernameTextField) {
        [self.zipCodeTextField becomeFirstResponder];
    } else if (textField == self.zipCodeTextField) {
        // submit vote
        [self saveProfile];
    }
    
    return YES;
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    // Return the number of sections.
    return MALNumberOfSections;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    // Return the number of rows in the section.
    if (section == MALInformationSection) {
        return MALNumberOfInformationRows;
    } else if (section == MALSubmitSection) {
        return MALNumberOfSubmitRows;
    }
    
    return 0;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString *TextFieldCellIdentifier = @"TextField Cell";
    static NSString *DropdownCellIdentifiter = @"Dropdown Cell";
    static NSString *SubmitCellIdentifier = @"Submit Cell";
    
    // configure cells
    if (indexPath.section == MALInformationSection) {
        if (indexPath.row == MALDateOfBirthRow || indexPath.row == MALGenderRow) {
            UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:DropdownCellIdentifiter forIndexPath:indexPath];
            cell.textLabel.text = self.informationRowLabels[indexPath.row];
            UILabel *detailLabel = (UILabel *)[cell viewWithTag:MALLabelTag];
            
            if (indexPath.row == MALDateOfBirthRow) {
                self.dateOfBirthLabel = detailLabel;
                if (self.user) {
                    detailLabel.textColor = [UIColor blackColor];
                    detailLabel.alpha = 1.0;
                    detailLabel.text = self.user.BirthDate;
                } else {
                    detailLabel.textColor = [UIColor lightGrayColor];
                    detailLabel.text = NSLocalizedString(@"Enter your birthday", nil);
                }
            } else {
                // gender label
                self.genderLabel = detailLabel;
                if (self.user) {
                    detailLabel.textColor = [UIColor blackColor];
                    detailLabel.alpha = 1.0;
                    detailLabel.text = [self.user readableGender];
                } else {
                    detailLabel.textColor = [UIColor lightGrayColor];
                    detailLabel.text = NSLocalizedString(@"Select your gender", nil);
                }
            }
            return cell;
        } else {
            UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:TextFieldCellIdentifier forIndexPath:indexPath];
            cell.selectionStyle = UITableViewCellSelectionStyleNone;
            UITextField *cellTextField = (UITextField *)[cell viewWithTag:MALTextFieldTag];
            UILabel *cellLabel = (UILabel *)[cell viewWithTag:MALLabelTag];
            cellLabel.text = self.informationRowLabels[indexPath.row];
            
            // grab textfield and save based on row, also set placeholder text
            switch (indexPath.row) {
                case MALFirstNameRow:
                    self.firstNameTextField = cellTextField;
                    if (self.user) {
                        self.firstNameTextField.text = self.user.FirstName;
                    } else {
                        self.firstNameTextField.placeholder = NSLocalizedString(@"First Name", nil);
                    }
                    break;
                case MALLastNameRow:
                    self.lastNameTextField = cellTextField;
                    if (self.user) {
                        self.lastNameTextField.text = self.user.LastName;
                    } else {
                        self.lastNameTextField.placeholder = NSLocalizedString(@"Last Name", nil);
                    }
                    break;
                case MALEmailRow:
                    self.emailTextField = cellTextField;
                    if (self.user) {
                        self.emailTextField.text = self.user.EmailAddress;
                    } else {
                        self.emailTextField.placeholder = NSLocalizedString(@"Email Address", nil);
                    }
                    break;
                case MALUsernameRow:
                    self.usernameTextField = cellTextField;
                    if (self.user) {
                        self.usernameTextField.text = self.user.Username;
                        
                        // disable cell since username is locked at this point
                        cellTextField.textColor = [UIColor lightGrayColor];
                        cellTextField.alpha = 0.7;
                        [cellTextField setEnabled:NO];
                    } else {
                        self.usernameTextField.placeholder = NSLocalizedString(@"Username", nil);
                    }
                    cellTextField.autocapitalizationType = UITextAutocapitalizationTypeNone;
                    break;
                case MALZipCodeRow:
                    self.zipCodeTextField = cellTextField;
                    if (self.user) {
                        self.zipCodeTextField.text = self.user.PostalCode;
                    } else {
                        self.zipCodeTextField.placeholder = NSLocalizedString(@"Zip Code", nil);
                    }
                    cellTextField.returnKeyType = UIReturnKeyDone;
                    break;
                default:
                    break;
            }
            
            return cell;
        }
    } else {
        // submit section
        UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:SubmitCellIdentifier forIndexPath:indexPath];
        
        // if user is nil then they are creating one
        if (self.user) {
            cell.textLabel.text = NSLocalizedString(@"Update Profile", nil);
        } else {
            cell.textLabel.text = NSLocalizedString(@"Let's Get Started", nil);
        }
        
        return cell;
    }
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    if (indexPath.section == MALInformationSection) {
        // gender and date cells require dropdown
        UITableViewCell *cell = [self.tableView cellForRowAtIndexPath:indexPath];
        
        if (indexPath.row == MALGenderRow) {
            [self displayGenderPickerPopover:cell];
        } else if(indexPath.row == MALDateOfBirthRow) {
            [self displayDatePickerPopover:cell];
        }
    } else if (indexPath.section == MALSubmitSection) {
        // see if user is valid
        [self saveProfile];
    }
    
    // deselect row
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}

@end
