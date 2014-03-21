//
//  GenericSelectionPickerController.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/22/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

#import "GenericSelectionPickerController.h"

@interface GenericSelectionPickerController ()

@end

@implementation GenericSelectionPickerController

- (void)viewDidLoad
{
    [super viewDidLoad];

    // custom setup
    self.clearsSelectionOnViewWillAppear = NO;
    
    // get size by number of items in array
    CGFloat height = 44.0 * self.itemsArray.count;
    self.preferredContentSize = CGSizeMake(180.0, height);
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    // Return the number of sections.
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    // Return the number of rows in the section.
    return self.itemsArray.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    UITableViewCell *cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:@"Cell"];
    
    // Configure the cell...
    cell.textLabel.text = [self.itemsArray objectAtIndex:indexPath.row];
    cell.selectionStyle = UITableViewCellSelectionStyleBlue;

    return cell;
}

#pragma mark - Table view delegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    if (self.delegate) {
        [self.delegate itemSelected:indexPath.row];
    }
    
    // deselect selected item
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}

@end
