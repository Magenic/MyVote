//
//  PollOption.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/16/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "PollOption.h"

@implementation PollOption

- (instancetype)initPollOptionWithDictionary:(NSDictionary *)dictionary
{
    self = [super init];
    
    if (self) {
        // create poll option
        NSNumber *holder = [dictionary objectForKey:@"PollOptionID"];
        self.pollOptionID = [CommonMethods validValueForJSONNumber:holder];
        
        // option text
        self.optionText = [dictionary objectForKey:@"OptionText"];
        
        // option position
        holder = [dictionary objectForKey:@"OptionPosition"];
        self.optionPosition = [CommonMethods validValueForJSONNumber:holder];
        
        // poll response id
        holder = [dictionary objectForKey:@"PollResponseID"];
        self.pollResponseID = [CommonMethods validValueForJSONNumber:holder];
        
        // is option selected
        holder = [dictionary objectForKey:@"IsOptionSelected"];
        self.isOptionSelected = holder.boolValue;
    }
    
    return self;
}

@end
