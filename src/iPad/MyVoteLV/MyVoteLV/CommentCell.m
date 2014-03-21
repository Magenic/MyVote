//
//  CommentCell.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/25/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

#import "CommentCell.h"
#import "BDBAttributedButton.h"

const CGFloat kLabelPadding = 10.0f;
const CGFloat kSidePadding = 15.0f;
const CGFloat kLabelHeight = 20.0f;

@implementation CommentCell

- (void)awakeFromNib
{
    [super awakeFromNib];
    
    /**
    // setup reply button
    NSDictionary *buttonAttributes = @{BDBCornerRadiusAttributeName : @(10.0f),
                                       BDBBorderWidthAttributeName : @(1.0f),
                                       BDBBorderColorAttributeName : [UIColor blueColor],
                                       BDBFillColorAttributeName : [UIColor lightGrayColor]};
    
    NSDictionary *buttonHighlightedAttributes = @{BDBCornerRadiusAttributeName: @ (10.0f),
                                                  BDBBorderWidthAttributeName : @(1.0f),
                                                  NSForegroundColorAttributeName : [UIColor whiteColor],
                                                  BDBFillColorAttributeName : [UIColor blueColor]};
    
    [self.replyButton setStyleAttributes:buttonAttributes forControlState:UIControlStateNormal];
    [self.replyButton setStyleAttributes:buttonHighlightedAttributes forControlState:UIControlStateHighlighted];
     */
}

@end
