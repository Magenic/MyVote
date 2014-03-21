//
//  BackgroundViewController.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/7/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "BackgroundViewController.h"

@implementation BackgroundViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
	
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

@end
