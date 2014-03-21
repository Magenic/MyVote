//
//  PollTypePopoverPickerController.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/20/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

@protocol PollTypePopoverDelegate <NSObject>

- (void)itemSelected:(NSString *)item;

@end

@interface PollTypePopoverPickerController : UITableViewController

@property (nonatomic, assign) id<PollTypePopoverDelegate> delegate;

@end
