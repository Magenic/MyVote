//
//  Response.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/16/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "Response.h"
#import "PollOption.h"

@implementation Response

- (instancetype)initResponseWithDictionary:(NSDictionary *)dictionary
{
    self = [super init];
    
    if (self) {
        // create response
        NSNumber *holder = [dictionary objectForKey:@"PollSubmissionID"];
        self.pollSubmissionID = [CommonMethods validValueForJSONNumber:holder];
        
        holder = [dictionary objectForKey:@"PollID"];
        self.pollID = holder.integerValue;
        
        holder = [dictionary objectForKey:@"MinAnswers"];
        self.minAnswers = [CommonMethods validValueForJSONNumber:holder];
        
        holder = [dictionary objectForKey:@"MaxAnswers"];
        self.maxAnswers = [CommonMethods validValueForJSONNumber:holder];
        
        holder = [dictionary objectForKey:@"UserID"];
        self.userID = [CommonMethods validValueForJSONNumber:holder];
        
        self.pollDescription = [dictionary objectForKey:@"PollDescription"];
        self.pollQuestion = [dictionary objectForKey:@"PollQuestion"];
        self.comment = [dictionary objectForKey:@"Comment"];
        self.submissionDate = [dictionary objectForKey:@"SubmissionDate"];
        
        // get poll options
        NSArray *options = [dictionary objectForKey:@"PollOptions"];
        NSMutableArray *pollOptions = [NSMutableArray arrayWithCapacity:options.count];
        
        for (NSDictionary *currentOption in options) {
            PollOption *newPollOption = [[PollOption alloc] initPollOptionWithDictionary:currentOption];
            [pollOptions addObject:newPollOption];
        }
        
        // copy immutable array
        self.pollOptions = [pollOptions copy];
    }
    
    return self;
}

@end
