//
//  PollResponse.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/29/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "PollResponse.h"

@implementation PollResponse

- (instancetype)initPollResponseWithDictionary:(NSDictionary *)dictionary
{
    self = [super init];
    
    if (self) {
        NSNumber *pollOptionID = [dictionary objectForKey:@"PollOptionID"];
        self.pollOptionID = pollOptionID.integerValue;
        
        NSNumber *responseCount = [dictionary objectForKey:@"ResponseCount"];
        self.responseCount = responseCount.integerValue;
        
        self.optionText = [dictionary objectForKey:@"OptionText"];
    }
    
    return self;
}

@end
