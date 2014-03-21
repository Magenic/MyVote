//
//  ViewController.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/6/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

@class MyVoteService;
@class User;

@protocol LoginDelegate <NSObject>

- (void)foundExistingUser:(User *)user;

@end

@interface ViewController : UIViewController

@property (weak, nonatomic) id<LoginDelegate> delegate;
@property (strong, nonatomic) MyVoteService *myVoteService;

@end
