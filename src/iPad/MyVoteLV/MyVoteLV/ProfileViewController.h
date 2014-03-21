//
//  ProfileViewController.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/8/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "BackgroundViewController.h"

@class User;

@protocol EditProfileDelegate <NSObject>

- (void)updatedUser:(User *)user;

@end

@interface ProfileViewController : BackgroundViewController

@property (weak, nonatomic) id<EditProfileDelegate> delegate;
@property (strong, nonatomic) User *user;

@end
