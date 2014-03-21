//
//  DatePickerViewController.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/11/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "DatePickerViewController.h"

@interface DatePickerViewController () 

@property (weak, nonatomic) IBOutlet UIDatePicker *startDatePicker;
@property (weak, nonatomic) IBOutlet UIDatePicker *endDatePicker;

@end

@implementation DatePickerViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
	
    // init dates to today
    if (!self.startDate) {
        self.startDate = [NSDate date];
    }
    
    if (!self.endDate) {
        self.endDate = [NSDate date];
    }
    
    // set min date as today for both pickers
    [self.startDatePicker setMinimumDate:[NSDate date]];
    [self.endDatePicker setMinimumDate:[NSDate date]];
}

#pragma mark - IBActions

- (IBAction)cancelTapped:(UIBarButtonItem *)sender
{
    [self.delegate datePickerStartDate:nil andEndDate:nil];
}

- (IBAction)doneTapped:(UIBarButtonItem *)sender
{
    // verify dates are valid
    if ([self datesValid]) {
        // delegate
        [self.delegate datePickerStartDate:self.startDate andEndDate:self.endDate];
    }
}

#pragma mark - DatePickerDelegate

- (IBAction)dateChanged:(UIDatePicker *)datePicker
{
    if (datePicker == self.startDatePicker) {
        self.startDate = datePicker.date;
    } else if (datePicker == self.endDatePicker) {
        self.endDate = datePicker.date;
    }
}

#pragma mark - Private Methods

- (BOOL)datesValid
{
    // start date must be before end date
    if ([self.startDate compare:self.endDate] == NSOrderedDescending) {
        // start date is more recent than end date
        return NO;
    }
    
    return YES;
}

@end
