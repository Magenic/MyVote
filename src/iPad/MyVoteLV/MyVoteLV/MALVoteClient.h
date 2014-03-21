//
//  MALVoteClient.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 3/2/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

#import "AFNetworking.h"

@class User;
@class Poll;
@class Response;
@class Result;
@class Comment;

// NS_ENUM for poll category type
typedef NS_ENUM(NSInteger, PollFilterType) {
    PollFilterTypeAll = 0,
    PollFilterTypeMostPopular,
    PollFilterTypeNewest
};

@interface MALVoteClient : AFHTTPClient

+ (id)sharedInstance;

#pragma mark - User APIs

// returns user with specified profile ID
- (void)getUserByProfileID:(NSString *)profileID withCompletion:(void (^)(User *user))completionBlock;

// adds User to DB
- (void)addUser:(NSDictionary *)user withCompletion:(void (^)(BOOL success))completionBlock;

// edit user
- (void)editUser:(NSDictionary *)user withProfileID:(NSString *)profileID withCompletion:(void (^)(BOOL success))completionBlock;

#pragma mark - Poll APIs

// returns a single poll with the given id
- (void)getPollWithID:(NSInteger)pollID withCompletion:(void (^)(Poll *poll))completionBlock;

// returns all polls by filter - all, most popular, newest, and ...
- (void)getPollWithFilterType:(PollFilterType)filter withCompletion:(void (^)(NSArray *polls))completionBlock;

// adds a new poll
- (void)addPoll:(NSDictionary *)poll withCompletion:(void (^)(BOOL success))completionBlock;

// deletes the poll with the given id
- (void)deletePoll:(NSInteger)pollID withCompletion:(void (^)(BOOL success))completionBlock;

#pragma mark - Respond APIs

// vote on poll
- (void)respondToPollWithResponse:(NSDictionary *)response withCompletion:(void (^)(BOOL success))completionBlock;

// get user response for a poll
- (void)getUserResponseForPollID:(NSInteger)pollID forUserID:(NSInteger)userID withCompletion:(void (^)(Response *response))completionBlock;

#pragma mark - Poll Result API

// get poll reults for a single poll
- (void)getPollResultsForPollID:(NSInteger)pollID withCompletion:(void (^)(Result *result))completionBlock;

#pragma mark - Comments API

// get comments for a poll with Poll ID
- (void)getCommentsForPollID:(NSInteger)pollID withCompletion:(void (^)(NSArray *comments))completionBlock;

// post comment
- (void)postComment:(Comment *)comment withCompletion:(void (^)(BOOL success))completionBlock;

@end
