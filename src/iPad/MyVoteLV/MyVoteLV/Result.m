//
//  Result.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/16/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "Result.h"
#import "PollResponse.h"
#import "Comment.h"

@implementation Result

- (instancetype)initResultFromDictionary:(NSDictionary *)dictionary
{
    self = [super init];
    
    if (self) {
        // create result
        NSNumber *holder = [dictionary objectForKey:@"PollID"];
        self.pollID = [CommonMethods validValueForJSONNumber:holder];
        self.question = [dictionary objectForKey:@"Question"];
        
        // grab options
        NSArray *results = [dictionary objectForKey:@"Results"];
        NSMutableArray *options = [NSMutableArray arrayWithCapacity:results.count];
        
        for (NSDictionary *pollResponse in results) {
            PollResponse *newPollResponse = [[PollResponse alloc] initPollResponseWithDictionary:pollResponse];
            [options addObject:newPollResponse];
        }
        
        self.pollOptionResults = [options copy];
        
        // grab comments
        NSArray *comments = [dictionary objectForKey:@"Comments"];
        NSMutableArray *commentObjects = [NSMutableArray arrayWithCapacity:comments.count];
        
        for (NSDictionary *commentDict in comments) {
            Comment *newComment = [[Comment alloc] initCommentWithDictionary:commentDict];
            [commentObjects addObject:newComment];
        }
        
        self.comments = [commentObjects copy];
    }
    
    return self;
}

@end
