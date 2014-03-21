//
//  ViewController.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/6/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

#import "ViewController.h"
#import "AppDelegate.h"
#import "MyVoteService.h"
#import "SVProgressHUD.h"
#import "User.h"
#import "PDKeychainBindings.h"
#import "LoginButton.h"

static const NSInteger MALTwitterTag = 67;
static const NSInteger MALFacebookTag = 68;
static const NSInteger MALMicrosoftTag = 69;
static const NSInteger MALGoogleTag = 70;

// segues
static NSString *const MALShowProfileSegue = @"Show Profile";

@interface ViewController ()

@property (weak, nonatomic) IBOutlet LoginButton *twitterButton;
@property (weak, nonatomic) IBOutlet LoginButton *facebookButton;
@property (weak, nonatomic) IBOutlet LoginButton *microsoftButton;
@property (weak, nonatomic) IBOutlet LoginButton *googleButton;

@end

@implementation ViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
	
    // hide nav bar
    [self.navigationController setNavigationBarHidden:YES];
    
    // setup background image
    UIImage *backgroundImage = nil;
    
    if (UIDeviceOrientationIsLandscape([[UIApplication sharedApplication] statusBarOrientation])) {
        // landscape
        backgroundImage = [UIImage imageNamed:@"background_landscape"];
    } else {
        // portrait
        backgroundImage = [UIImage imageNamed:@"background_portrait"];
    }
    
    // set image
    self.view.backgroundColor = [UIColor colorWithPatternImage:backgroundImage];
    
    // setup
    [self setupButtons];
}

- (void)willAnimateRotationToInterfaceOrientation:(UIInterfaceOrientation)toInterfaceOrientation duration:(NSTimeInterval)duration
{
    // setup background image
    UIImage *backgroundImage = nil;
    
    if (UIDeviceOrientationIsLandscape([[UIApplication sharedApplication] statusBarOrientation])) {
        // landscape
        backgroundImage = [UIImage imageNamed:@"background_landscape"];
    } else {
        // portrait
        backgroundImage = [UIImage imageNamed:@"background_portrait"];
    }
    
    // set image
    self.view.backgroundColor = [UIColor colorWithPatternImage:backgroundImage];
}

#pragma mark - Button Setup

- (void)setupButtons
{
    /** button background colors (RGB):
     Twitter = 1, 171, 233
     Facebook = 61, 80, 159
     Google = 220, 2, 30
     Microsoft = 255, 255, 255
     */
    
    // twitter
    [self.twitterButton setBackgroundColor:[UIColor colorWithRed:(1.0/255.0) green:(171.0/255.0) blue:(233.0/255.0) alpha:1]];
    [self.twitterButton setLoginImage:[UIImage imageNamed:@"icon_twitter_100x100"]];
    
    // facebook
    [self.facebookButton setBackgroundColor:[UIColor colorWithRed:(61.0/255.0) green:(80.0/255.0) blue:(159.0/255.0) alpha:1]];
    [self.facebookButton setLoginImage:[UIImage imageNamed:@"icon_facebook_100x100"]];
    
    // google
    [self.googleButton setBackgroundColor:[UIColor colorWithRed:(220.0/255.0) green:(2.0/255.0) blue:(30.0/255.0) alpha:1]];
    [self.googleButton setLoginImage:[UIImage imageNamed:@"icon_google_100x100"]];
    
    // microsoft
    [self.microsoftButton setBackgroundColor:[UIColor colorWithRed:1.0 green:1.0 blue:1.0 alpha:1]];
    [self.microsoftButton setLoginImage:[UIImage imageNamed:@"icon_microsoft_100x100"]];
}

#pragma mark - IBActions

- (IBAction)login:(UIButton *)sender
{
    // grab which service user is trying to login with
    NSString *provider = @"";
    if (sender.tag == MALTwitterTag) {
        provider = @"twitter";
    } else if (sender.tag == MALFacebookTag) {
        provider = @"facebook";
    } else if (sender.tag == MALMicrosoftTag){
        provider = @"microsoftaccount";
    } else {
        provider = @"google";
    }
    
    // get client and display login view controller
    MSClient *client = self.myVoteService.client;
    
    if (client.currentUser != nil) {
        return;
    }
    
    [client loginWithProvider:provider controller:self animated:YES completion:^(MSUser *user, NSError *error) {
        if (error) {
            if (error.code != -1503) {
                // error -1503 just means the user pressed cancel on the login view
                UIAlertView *alert = [[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Login Error", nil)
                                                                message:NSLocalizedString(@"There was a problem logging in. Please try again.", nil)
                                                               delegate:nil
                                                      cancelButtonTitle:NSLocalizedString(@"OK", nil)
                                                      otherButtonTitles:nil];
                [alert show];
            } else {
                NSLog(@"Error code: %d -> %@", error.code, error.description);
            }
        } else {
            // set access token
            [[PDKeychainBindings sharedKeychainBindings] setObject:user.mobileServiceAuthenticationToken forKey:@"accessToken"];
            
            // set authentication
            [[MALVoteClient sharedInstance] setAuthorizationHeaderWithToken:user.mobileServiceAuthenticationToken];
            [self doesUserAlreadyExistsWithProfileID:user.userId];
        }
    }];
}

- (void)doesUserAlreadyExistsWithProfileID:(NSString *)profileID
{
    // show HUD
    [SVProgressHUD show];
    
    // see if user with this profile ID already exists first then if, if not, create one
    [[MALVoteClient sharedInstance] getUserByProfileID:profileID withCompletion:^(User *user) {
        if (user) {
            // successfully retrieved user - one is already created with this profile ID
            [SVProgressHUD showSuccessWithStatus:NSLocalizedString(@"We Found Your User!", nil)];
            
            // set profile ID
            [[PDKeychainBindings sharedKeychainBindings] setObject:user.ProfileID forKey:@"profileID"];
            
            // delegate - set user
            [self.delegate foundExistingUser:user];
            
            // dismiss view to see polls
            [self.presentingViewController dismissViewControllerAnimated:YES completion:^{
                // delegate - set user
                //[self.delegate foundExistingUser:user];
            }];
        } else {
            // dismis HUD
            [SVProgressHUD dismiss];
            
            // no user found so create one
            [self performSegueWithIdentifier:MALShowProfileSegue sender:self];
        }
    }];
}

@end
