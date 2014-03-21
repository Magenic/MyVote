//
//  Response.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/16/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

/** 
 * This represents a user's response to a poll
 */

@interface Response : NSObject

@property NSInteger pollSubmissionID;
@property NSInteger pollID;
@property NSInteger userID;
@property NSInteger minAnswers;
@property NSInteger maxAnswers;
@property (strong, nonatomic) NSString *pollDescription;
@property (strong, nonatomic) NSString *pollQuestion;
@property (strong, nonatomic) NSString *comment;
@property (strong, nonatomic) NSString *submissionDate;
@property (strong, nonatomic) NSArray *pollOptions;

- (instancetype)initResponseWithDictionary:(NSDictionary *)dictionary;

@end
