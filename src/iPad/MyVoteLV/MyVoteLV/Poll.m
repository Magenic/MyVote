//
//  Poll.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/22/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

#import "Poll.h"

@implementation Poll

- (instancetype)initPollWithDictionary:(NSDictionary *)dict
{
    self = [super init];
    
    if (self) {
        // custom setup here
        NSNumber *holder = [dict objectForKey:@"PollAdminRemovedFlag"];
        self.PollAdminRemovedFlag = holder.boolValue;
        
        holder = [dict objectForKey:@"PollDeleteFlag"];
        self.PollDeletedFlag = holder.boolValue;
        
        holder = [dict objectForKey:@"PollID"];
        self.PollID = [CommonMethods validValueForJSONNumber:holder];
        
        holder = [dict objectForKey:@"PollMaxAnswers"];
        self.PollMaxAnswers = [CommonMethods validValueForJSONNumber:holder];
        
        holder = [dict objectForKey:@"PollMinAnswers"];
        self.PollMinAnswers = [CommonMethods validValueForJSONNumber:holder];
        
        holder = [dict objectForKey:@"UserID"];
        self.UserID = [CommonMethods validValueForJSONNumber:holder];

        self.pollCategory = [Enums translateStringToPollCategoryType:[dict objectForKey:@"PollCategoryID"]];
        self.PollDateRemoved = [dict objectForKey:@"PollDateRemoved"];
        self.PollDeletedDate = [dict objectForKey:@"PollDeletedDate"];
        self.PollEndDate = [dict objectForKey:@"PollEndDate"];
        
        self.PollImageLink = [dict objectForKey:@"PollImageLink"];
        self.PollOptions = [dict objectForKey:@"PollOptions"];
        self.PollQuestion = [dict objectForKey:@"PollQuestion"];
        self.PollStartDate = [dict objectForKey:@"PollStartDate"];
    }
    
    return self;
}

@end
