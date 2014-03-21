//
//  PollSummary.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/16/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "Enums.h"
#import "CommonMethods.h"

@interface PollSummary : NSObject

// category, id, imageLink, question, submission count
@property NSInteger pollID;
@property NSInteger submissionCount;
@property (strong, nonatomic) NSString *imageLink;
@property (strong, nonatomic) NSString *question;
@property PollCategoryType categoryType;

- (instancetype)initPollSummaryWithDictionary:(NSDictionary *)dictionary;

@end
