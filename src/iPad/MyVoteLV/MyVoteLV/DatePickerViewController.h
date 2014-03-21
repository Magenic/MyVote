//
//  DatePickerViewController.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/11/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import <UIKit/UIKit.h>

@protocol DatePickerDelegate <NSObject>

- (void)datePickerStartDate:(NSDate *)startDate andEndDate:(NSDate *)endDate;

@end

@interface DatePickerViewController : UIViewController

@property (weak, nonatomic) id<DatePickerDelegate> delegate;
@property (strong, nonatomic) NSDate *startDate;
@property (strong, nonatomic) NSDate *endDate;

@end
