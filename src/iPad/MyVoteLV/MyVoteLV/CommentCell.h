//
//  CommentCell.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/25/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

FOUNDATION_EXPORT UIFont * const kCommentUsernameLabelFont;
FOUNDATION_EXPORT UIFont * const kCommentLabelFont;
FOUNDATION_EXPORT UIFont * const kCommentDateFont;
FOUNDATION_EXPORT const CGFloat kLabelPadding;
FOUNDATION_EXPORT const CGFloat kSidePadding;
FOUNDATION_EXPORT const CGFloat kLabelHeight;

@interface CommentCell : UITableViewCell

@property (weak, nonatomic) IBOutlet UILabel *commentLabel;
@property (weak, nonatomic) IBOutlet UILabel *dateLabel;
@property (weak, nonatomic) IBOutlet UIButton *replyButton;

@end
