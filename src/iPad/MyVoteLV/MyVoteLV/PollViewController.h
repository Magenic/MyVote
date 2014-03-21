//
//  PollViewController.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/6/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "BackgroundViewController.h"

@class User;
@class PollSummary;

@interface PollViewController : BackgroundViewController

@property (strong, nonatomic) User *currentUser;
@property (strong, nonatomic) PollSummary *pollSummary;

@end
