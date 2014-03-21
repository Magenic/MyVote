//
//  GenericSelectionPickerController.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/22/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

#import <UIKit/UIKit.h>

@protocol GenericSelectionPickerDelegate <NSObject>

- (void)itemSelected:(NSInteger)index;

@end

@interface GenericSelectionPickerController : UITableViewController

@property (nonatomic, strong) NSArray *itemsArray;
@property (nonatomic, weak) id<GenericSelectionPickerDelegate> delegate;

@end
