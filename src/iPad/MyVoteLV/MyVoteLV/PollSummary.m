//
//  PollSummary.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/16/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "PollSummary.h"

@implementation PollSummary

- (instancetype)initPollSummaryWithDictionary:(NSDictionary *)dictionary
{
    self = [super init];
    
    if (self) {
        NSNumber *numberHolder = [dictionary objectForKey:@"Id"];
        self.pollID = [CommonMethods validValueForJSONNumber:numberHolder];
        
        self.imageLink = [dictionary objectForKey:@"ImageLink"];
        self.question = [dictionary objectForKey:@"Question"];
        numberHolder = [dictionary objectForKey:@"SubmissionCount"];
        self.submissionCount = [CommonMethods validValueForJSONNumber:numberHolder];
        
        self.categoryType = [Enums translateStringToPollCategoryType:[dictionary objectForKey:@"Category"]];
    }
    
    return self;
}

@end
