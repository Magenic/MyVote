//
//  NSDate+Convenience.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/1/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "NSDate+Convenience.h"

@implementation NSDate (Convenience)

+ (NSDateFormatter *)dateFormatter
{
    static NSDateFormatter *dateFormatter;
    if (!dateFormatter) {
        dateFormatter = [[NSDateFormatter alloc] init];
    }
    
    [dateFormatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ss.SSS"];
    NSTimeZone *gmt = [NSTimeZone timeZoneWithAbbreviation:@"GMT"];
    [dateFormatter setTimeZone:gmt];
    
    return dateFormatter;
}

+ (NSString *)stringFromDateString:(NSString *)dateString
{
    NSDateFormatter *dateFormatter = [NSDate dateFormatter];
    
    // get date from string
    NSDate *date = [dateFormatter dateFromString:dateString];
    
    // format date - 12:10PM 1/31/2014
    [dateFormatter setTimeZone:[NSTimeZone localTimeZone]];
    [dateFormatter setDateFormat:@"h:mma M/d/yyyy"];
    
    return [dateFormatter stringFromDate:date];
}

@end
