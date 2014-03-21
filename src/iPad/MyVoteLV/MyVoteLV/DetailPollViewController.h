//
//  DetailPollViewController.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/14/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import <UIKit/UIKit.h>

@class User;
@class PollSummary;

@interface DetailPollViewController : UIViewController

@property (strong, nonatomic) User *currentUser;
@property (strong, nonatomic) PollSummary *pollSummary;

@end
