//
//  Comment.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/16/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "Comment.h"
#import "NSDate+Convenience.h"

@implementation Comment

- (instancetype)initCommentWithDictionary:(NSDictionary *)dictionary
{
    self = [super init];
    
    if (self) {
        // create comment
        NSNumber *value = [dictionary objectForKey:@"PollID"];
        self.pollID = [CommonMethods validValueForJSONNumber:value];
        
        value = [dictionary objectForKey:@"PollCommentID"];
        self.pollCommentID = [CommonMethods validValueForJSONNumber:value];
        
        value = [dictionary objectForKey:@"ParentCommentID"];
        self.parentCommentID = [CommonMethods validValueForJSONNumber:value];
        
        self.commentDate = [NSDate stringFromDateString:[dictionary objectForKey:@"CommentDate"]];
        
        self.commentText = [dictionary objectForKey:@"CommentText"];
        
        self.username = [dictionary objectForKey:@"UserName"];
        
        value = [dictionary objectForKey:@"UserID"];
        self.userID = [CommonMethods validValueForJSONNumber:value];
        
        // child comments
        NSArray *childCommentsArray = dictionary[@"Comments"];
        NSMutableArray *comments = [NSMutableArray arrayWithCapacity:childCommentsArray.count];
        
        for (NSDictionary *dict in childCommentsArray) {
            Comment *childComment = [[Comment alloc] initCommentWithDictionary:dict];
            childComment.usernameRepliedTo = self.username;
            [comments addObject:childComment];
        }
        
        self.childComments = [comments copy];
        
        // store NSDate of comment
        NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
        [dateFormatter setDateFormat:@"h:mma M/d/yyyy"];
        self.commentCreationDate = [dateFormatter dateFromString:self.commentDate];
    }
    
    return self;
}

- (NSDictionary *)serializeObjectForJSON
{
    return @{@"UserID" : @(self.userID),
             @"PollID" : @(self.pollID),
             @"CommentText" : self.commentText,
             @"UserName" : self.username,
             @"ParentCommentID" : self.parentCommentID == -1 ? [NSNull null] : @(self.parentCommentID)};
}

- (NSAttributedString *)attributedStringForCommentText
{
    // comment fro text
    NSMutableAttributedString *commentText = nil;
    
    // if parent comment, username says:
    if (self.parentCommentID == -1) {
        commentText = [[NSMutableAttributedString alloc] initWithString:[NSString stringWithFormat:@"%@ says: %@", self.username, self.commentText]];
    } else {
        // else, username replied to username:
        commentText = [[NSMutableAttributedString alloc] initWithString:[NSString stringWithFormat:@"%@ replied to %@: %@", self.username, self.usernameRepliedTo, self.commentText]];
    }
    
    // add attributes for text color - username should be light gray, text black
    NSUInteger lengthOfEntireString = [commentText length];
    NSUInteger lengthOfCommentOnly = [self.commentText length];
    NSUInteger lengthOfUsernameTextOnly = lengthOfEntireString - lengthOfCommentOnly;
    [commentText addAttribute:NSForegroundColorAttributeName value:[UIColor lightGrayColor] range:NSMakeRange(0, lengthOfUsernameTextOnly)];
    [commentText addAttribute:NSForegroundColorAttributeName value:[UIColor blackColor] range:NSMakeRange(lengthOfUsernameTextOnly, lengthOfCommentOnly)];
    
    return [commentText copy];
}

@end
