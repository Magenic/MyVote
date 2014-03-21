//
//  Poll.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/22/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

#import "Enums.h"
#import "CommonMethods.h"

@interface Poll : NSObject

// naming convention isn't traditional but it aligns with API keys
@property BOOL PollAdminRemovedFlag;
@property BOOL PollDeletedFlag;
@property NSInteger PollID;
@property NSInteger PollMaxAnswers;
@property NSInteger PollMinAnswers;
@property NSInteger UserID;
@property PollCategoryType pollCategory;
@property (strong, nonatomic) NSString *PollDateRemoved;
@property (strong, nonatomic) NSString *PollDeletedDate;
@property (strong, nonatomic) NSString *PollEndDate;
@property (strong, nonatomic) NSString *PollImageLink;
@property (strong, nonatomic) NSString *PollQuestion;
@property (strong, nonatomic) NSString *PollStartDate;
@property (strong, nonatomic) NSArray *PollOptions;

- (instancetype)initPollWithDictionary:(NSDictionary *)dict;

@end
