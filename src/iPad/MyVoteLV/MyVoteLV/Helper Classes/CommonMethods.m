//
//  CommonMethods.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/18/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "CommonMethods.h"

@implementation CommonMethods

+ (NSInteger)validValueForJSONNumber:(NSNumber *)number
{
    if ([number isEqual:[NSNull null]]) {
        return -1;
    } else {
        return number.integerValue;
    }
}

@end
