//
//  MALVoteClient.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 3/2/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

#import "MALVoteClient.h"
#import "AFJSONRequestOperation.h"
#import "Constants.h"

// objects
#import "Poll.h"
#import "User.h"
#import "Comment.h"
#import "Response.h"
#import "PollResponse.h"
#import "Result.h"
#import "PollSummary.h"

@implementation MALVoteClient

+ (id)sharedInstance
{
    static MALVoteClient *sharedInstance;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[MALVoteClient alloc] initWithBaseURL:[NSURL URLWithString:kMALAPIBaseURLString]];
    });
    
    return sharedInstance;
}

- (id)initWithBaseURL:(NSURL *)url
{
    self = [super initWithBaseURL:url];
    
    if (self) {
        // custom setup        
        [self setParameterEncoding:AFJSONParameterEncoding];
        [self registerHTTPOperationClass:[AFJSONRequestOperation class]];
        [self setDefaultHeader:@"Accept" value:@"application/json"];
    }
    
    return self;
}

- (void)setAuthorizationHeaderWithToken:(NSString *)token
{
    [self setDefaultHeader:@"Authorization" value:[NSString stringWithFormat:@"Bearer %@", token]];
}

#pragma mark - User APIs

- (void)getUserByProfileID:(NSString *)profileID withCompletion:(void (^)(User *))completionBlock
{
    NSString *getUserPath = [NSString stringWithFormat:@"user?userProfileId=%@", profileID];
    [self getPath:getUserPath parameters:nil success:^(AFHTTPRequestOperation *op, id response) {
        // user returned
        if (response) {
            if ([response isKindOfClass:[NSDictionary class]]) {
                User *user = [[User alloc] initUserWithDictionary:response];
                completionBlock(user);
            } else {
                // invalid return type
                completionBlock(nil);
            }
        } else {
            completionBlock(nil);
        }
    } failure:^(AFHTTPRequestOperation *op, NSError *error) {
        // error retrieving user
        completionBlock(nil);
    }];
}

- (void)addUser:(NSDictionary *)user withCompletion:(void (^)(BOOL success))completionBlock
{
    // add user to DB
    [self putPath:@"user" parameters:user success:^(AFHTTPRequestOperation *op, id response) {
        // check status code?
        completionBlock(YES);
    } failure:^(AFHTTPRequestOperation *op, NSError *error) {
        // need to check for duplicates here - error 401 given?! that may be authentication error code
        completionBlock(NO);
    }];
}

- (void)editUser:(NSDictionary *)user withProfileID:(NSString *)profileID withCompletion:(void (^)(BOOL success))completionBlock
{
    // edit user in DB
    NSString *putPath = [NSString stringWithFormat:@"user?userProfileId=%@", profileID];
    [self putPath:putPath parameters:user success:^(AFHTTPRequestOperation *op, id response) {
        completionBlock(YES);
    } failure:^(AFHTTPRequestOperation *op, NSError *error) {
        completionBlock(NO);
    }];
}

#pragma mark - Poll APIs

- (void)getPollWithID:(NSInteger)pollID withCompletion:(void (^)(Poll *))completionBlock
{
    // setup get string
    NSString *getPathString = [NSString stringWithFormat:@"Poll/%d", pollID];
    [self getPath:getPathString parameters:nil success:^(AFHTTPRequestOperation *op, id response) {
        if (response) {
            // create single poll from response
            
        } else {
            // return nil
            completionBlock(nil);
        }
    } failure:^(AFHTTPRequestOperation *op, NSError *error) {
        // return nil
        completionBlock(nil);
    }];
}

- (void)getPollWithFilterType:(PollFilterType)filter withCompletion:(void (^)(NSArray *))completionBlock
{
    // get path
    NSString *getPath = @"";
    if (filter == PollFilterTypeAll) {
        getPath = @"poll";
    } else {
        getPath = [NSString stringWithFormat:@"poll?filterBy=%@", [self translatePollFilterType:filter]];
    }

    [self getPath:getPath parameters:nil success:^(AFHTTPRequestOperation *op, id response) {
        if (response) {
            // create polls from response
            if ([response isKindOfClass:[NSArray class]]) {
                NSArray *polls = [self parsePollsFromResponse:(NSDictionary *)response];
                
                // completion block
                completionBlock(polls);
            }
        } else {
            // return nil
            completionBlock(nil);
        }
    } failure:^(AFHTTPRequestOperation *op, NSError *error) {
        // return nil
        completionBlock(nil);
    }];
}

- (void)addPoll:(NSDictionary *)poll withCompletion:(void (^)(BOOL))completionBlock
{
    [self putPath:@"poll" parameters:poll success:^(AFHTTPRequestOperation *op, id response) {
        // check status code?
        completionBlock(YES);
    } failure:^(AFHTTPRequestOperation *op, NSError *error) {
        completionBlock(NO);
    }];
}

- (void)deletePoll:(NSInteger)pollID withCompletion:(void (^)(BOOL))completionBlock
{
    NSString *deletePath = [NSString stringWithFormat:@"poll/%d", pollID];
    [self deletePath:deletePath parameters:nil success:^(AFHTTPRequestOperation *op, id response) {
        // check status code?
        completionBlock(YES);
    } failure:^(AFHTTPRequestOperation *op, NSError *error) {
        completionBlock(NO);
    }];
}

#pragma mark - Respond APIs

- (void)respondToPollWithResponse:(NSDictionary *)response withCompletion:(void (^)(BOOL))completionBlock
{
    [self putPath:@"respond" parameters:response success:^(AFHTTPRequestOperation *op, id response) {
        // check status code?
        completionBlock(YES);
    } failure:^(AFHTTPRequestOperation *op, NSError *error) {
        completionBlock(NO);
    }];
}

- (void)getUserResponseForPollID:(NSInteger)pollID forUserID:(NSInteger)userID withCompletion:(void (^)(Response *))completionBlock
{
    NSString *getPath = [NSString stringWithFormat:@"respond?pollID=%d&userID=%d", pollID, userID];
    [self getPath:getPath parameters:nil success:^(AFHTTPRequestOperation *op, id response) {
        if (response) {
            // create response
            if ([response isKindOfClass:[NSDictionary class]]) {
                Response *userResponse = [[Response alloc] initResponseWithDictionary:response];
                completionBlock(userResponse);
            }
        } else {
            // error
            completionBlock(nil);
        }
    } failure:^(AFHTTPRequestOperation *op, NSError *error) {
        // error
        completionBlock(nil);
    }];
}

#pragma mark - Poll Result API

- (void)getPollResultsForPollID:(NSInteger)pollID withCompletion:(void (^)(Result *))completionBlock
{
    NSString *getPath = [NSString stringWithFormat:@"PollResult?pollId=%d", pollID];
    [self getPath:getPath parameters:nil success:^(AFHTTPRequestOperation *op, id response) {
        if (response) {
            // create poll results from response
            if ([response isKindOfClass:[NSDictionary class]]) {
                // Poll Response
                Result *result = [[Result alloc] initResultFromDictionary:response];
                completionBlock(result);
            } else {
                // invalid return type
                completionBlock(nil);
            }
        } else {
            // return nil
            completionBlock(nil);
        }
    } failure:^(AFHTTPRequestOperation *op, NSError *error) {
        // return nil
        completionBlock(nil);
    }];
}

#pragma mark - Comments API

- (void)getCommentsForPollID:(NSInteger)pollID withCompletion:(void (^)(NSArray *))completionBlock
{
    //GET api/PollComment?pollID={pollID}
    NSString *getPath = [NSString stringWithFormat:@"PollComment?pollID=%d", pollID];
    [self getPath:getPath parameters:nil success:^(AFHTTPRequestOperation *op, id response) {
        if (response) {
            // return array of comments
            if ([response isKindOfClass:[NSArray class]]) {
                NSArray *comments = [self parseCommentsFromResponse:response];
                completionBlock(comments);
            } else {
                completionBlock(nil);
            }
        } else {
            // return nil
            completionBlock(nil);
        }
    } failure:^(AFHTTPRequestOperation *op, NSError *error) {
        // return nil
        completionBlock(nil);
    }];
}

- (void)postComment:(Comment *)comment withCompletion:(void (^)(BOOL))completionBlock
{
    // get dict from comment to pass
    NSDictionary *parameters = [comment serializeObjectForJSON];
    
    // post comment
    [self putPath:@"PollComment" parameters:parameters success:^(AFHTTPRequestOperation *op, id response) {
        // response should be a dict with PollCommentID
        completionBlock(YES);
    } failure:^(AFHTTPRequestOperation *op, NSError *error) {
        // error posting comment
        completionBlock(NO);
    }];
}

#pragma mark - Private Helper Methods

- (NSArray *)parsePollsFromResponse:(NSDictionary *)response
{
    NSMutableArray *pollsArray = [NSMutableArray array];
    
    // each dictionary = poll summary
    for (NSDictionary *pollSummary in response) {
        PollSummary *newPollSummary = [[PollSummary alloc] initPollSummaryWithDictionary:pollSummary];
        
        [pollsArray addObject:newPollSummary];
    }
    
    // return array of polls
    return [pollsArray copy];
}

- (NSArray *)parseCommentsFromResponse:(NSDictionary *)response
{
    NSMutableArray *commentsArray = [NSMutableArray array];
    
    // each dictionary = comment
    for (NSDictionary *comment in response) {
        Comment *newComment = [[Comment alloc] initCommentWithDictionary:comment];
        
        [commentsArray addObject:newComment];
    }
    
    // return array of comments
    return [commentsArray copy];
}

- (NSString *)translatePollFilterType:(PollFilterType)filterType
{
    if (filterType == PollFilterTypeMostPopular) {
        return @"mostpopular";
    } else if (filterType == PollFilterTypeNewest) {
        return @"newest";
    } else {
        // PollFilterTypeAll = @""
        return @"";
    }
}

@end
