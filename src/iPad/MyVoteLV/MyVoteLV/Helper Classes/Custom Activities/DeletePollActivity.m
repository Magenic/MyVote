//
//  DeletePollActivity.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/10/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "DeletePollActivity.h"

@implementation DeletePollActivity

+ (UIActivityCategory)activityCategory
{
    return UIActivityCategoryAction;
}

- (NSString *)activityType
{
    return @"myvote.Delete.Poll";
}

- (NSString *)activityTitle
{
    return NSLocalizedString(@"Delete Poll", nil);
}

- (BOOL)canPerformWithActivityItems:(NSArray *)activityItems
{
    //NSLog(@"%s", __FUNCTION__);
    return YES;
}

- (void)performActivity
{
    [[NSNotificationCenter defaultCenter] postNotificationName:@"UIActivity" object:nil];
    
    [self activityDidFinish:YES];
}

@end
