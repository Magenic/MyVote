//
//  CommentReplyViewController.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/12/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import <UIKit/UIKit.h>

@class Comment;

@protocol ReplyCommentDelegate <NSObject>

- (void)replyToComment:(Comment *)comment withCommentText:(NSString *)newCommentText;

@end

@interface CommentReplyViewController : UIViewController

@property (weak, nonatomic) id<ReplyCommentDelegate> delegate;
@property (strong, nonatomic) Comment *comment;

@end
