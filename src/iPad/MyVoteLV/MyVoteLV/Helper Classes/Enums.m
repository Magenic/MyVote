//
//  Enums.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/16/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "Enums.h"

@implementation Enums

+ (PollCategoryType)translateStringToPollCategoryType:(NSString *)category
{
    // fun, sports, technology, entertainment, news, off-topic
    if ([category isEqualToString:@"Fun"]) {
        return PollCategoryTypeFun;
    } else if ([category isEqualToString:@"Technology"]) {
        return PollCategoryTypeTechnology;
    } else if ([category isEqualToString:@"Entertainment"]) {
        return PollCategoryTypeEntertainment;
    } else if ([category isEqualToString:@"News"]) {
        return PollCategoryTypeNews;
    } else if ([category isEqualToString:@"Sports"]) {
        return PollCategoryTypeSports;
    } else if ([category isEqualToString:@"Off-Topic"]) {
        return PollCategoryTypeOffTopic;
    }
    
    return PollCategoryTypeInvalid;
}

@end
