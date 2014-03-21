//
//  PollTypePopoverPickerController.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/20/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

#import "PollTypePopoverPickerController.h"

// number of sections
static const NSInteger kNumberOfSections = 3;

// section tags
const NSUInteger kHomeSection = 0;
const NSUInteger kPollTypesSection = 1;
const NSUInteger kAdminSection = 2;

// row counts
const NSUInteger kHomeRows = 1;
const NSUInteger kPollTypesRows = 2;
const NSUInteger kAdminRows = 2;

// constants
const CGFloat kHeaderHeight = 23.0f;
const CGFloat kCellRowHeight = 44.0f;
const CGFloat kTableViewWidth = 200.0f;

@interface PollTypePopoverPickerController ()

@property (copy, nonatomic) NSArray *itemsArray;

@end

@implementation PollTypePopoverPickerController

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    // init items array
    self.itemsArray = @[NSLocalizedString(@"Home", nil),
                        NSLocalizedString(@"Most Popular", nil),
                        NSLocalizedString(@"Recently Added", nil),
                        NSLocalizedString(@"Edit Profile", nil),
                        NSLocalizedString(@"Logout", nil)];
    
    // set size of popover - 88 height for two rows  (each defaults to 44)
    CGFloat height = (kCellRowHeight * self.itemsArray.count);
    
    // account for section headers too
    height += kHeaderHeight * (kNumberOfSections - 1);
    
    self.preferredContentSize = CGSizeMake(kTableViewWidth, height);
    
    self.clearsSelectionOnViewWillAppear = NO;
    [self.tableView setScrollEnabled:NO];
    
    // no separators
    self.tableView.separatorStyle = UITableViewCellSeparatorStyleNone;
}

#pragma mark - Table view data source

- (NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section
{
    if (section == kPollTypesSection) {
        return NSLocalizedString(@"Poll Types", nil);
    } else if (section == kAdminSection) {
        return NSLocalizedString(@"Admin", nil);
    } else {
        return @"";
    }
}

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    return kNumberOfSections;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    // Return the number of rows in the section.
    if (section == kHomeSection) {
        return kHomeRows;
    } else if (section == kPollTypesSection) {
        return kPollTypesRows;
    } else if (section == kAdminSection) {
        return kAdminRows;
    } else {
        return 0;
    }
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    UITableViewCell *cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:@"Cell"];
    
    static int indexCount = 0;
    
    // Configure the cell...
    cell.textLabel.text = [self.itemsArray objectAtIndex:indexCount++];
    cell.selectionStyle = UITableViewCellSelectionStyleBlue;
    
    return cell;
}

#pragma mark - Table view delegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    if (self.delegate != nil) {
        if (indexPath.section < 2) {
            int selectedIndex = indexPath.section + indexPath.row;
            [self.delegate itemSelected:[self.itemsArray objectAtIndex:selectedIndex]];
        } else {
            int selectedIndex = 0;
            if (indexPath.row == 0) {
                selectedIndex = 3;
            } else {
                selectedIndex = 4;
            }
            
            [self.delegate itemSelected:[self.itemsArray objectAtIndex:selectedIndex]];
        }
    }
    
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}

@end
