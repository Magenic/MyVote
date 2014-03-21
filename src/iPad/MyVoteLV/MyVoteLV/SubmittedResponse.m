//
//  SubmittedResponse.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/29/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "SubmittedResponse.h"

@implementation SubmittedResponse

- (NSDictionary *)serializeObjectForJSON
{
    // setup dictionary for response
    return @{@"PollID" : [NSNumber numberWithInteger:self.pollID],
             @"UserID" : [NSNumber numberWithInteger:self.userID],
             @"Comment" : @"",
             @"ResponseItems" : self.responseItems};
}

@end
