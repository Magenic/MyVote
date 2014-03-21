//
//  Comment.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/16/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "CommonMethods.h"

@interface Comment : NSObject

@property NSInteger userID;
@property NSInteger pollID;
@property NSInteger pollCommentID;
@property NSInteger parentCommentID;
@property (copy, nonatomic) NSArray *childComments;
@property (copy, nonatomic) NSString *commentDate;
@property (copy, nonatomic) NSString *commentText;
@property (copy, nonatomic) NSString *username;
@property (copy, nonatomic) NSString *usernameRepliedTo;
@property (strong, nonatomic) NSDate *commentCreationDate;
@property (strong, nonatomic) NSDate *pollCommentDeletedDate;
@property BOOL pollCommentDeletedFlag;

- (instancetype)initCommentWithDictionary:(NSDictionary *)dictionary;
- (NSDictionary *)serializeObjectForJSON;
- (NSAttributedString *)attributedStringForCommentText;

@end
