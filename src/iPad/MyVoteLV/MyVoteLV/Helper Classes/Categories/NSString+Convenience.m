//
//  NSString+Convenience.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/6/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "NSString+Convenience.h"

@implementation NSString (Convenience)

- (NSString *)stringByTrimmingLeadingWhitespace
{
    NSInteger i = 0;
    
    while ((i < [self length]) && [[NSCharacterSet whitespaceCharacterSet] characterIsMember:[self characterAtIndex:i]]) {
        i++;
    }
    
    return [self substringFromIndex:i];
}

@end
