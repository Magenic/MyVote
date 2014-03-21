//
//  PollCollectionViewCell.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/21/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

#import "PollCollectionViewCell.h"

const CGFloat CellLabelPadding = 5.0f;

@implementation PollCollectionViewCell

- (instancetype)initWithCoder:(NSCoder *)aDecoder
{
    self = [super initWithCoder:aDecoder];
    if (self) {
        // add imageView
        self.pollImageView = [[UIImageView alloc] initWithFrame:CGRectMake(self.contentView.frame.origin.x,
                                                                           self.contentView.frame.origin.y,
                                                                           self.contentView.frame.size.width,
                                                                           self.contentView.frame.size.height)];
        
        // set image scale type
        self.pollImageView.contentMode = UIViewContentModeScaleAspectFill;
        
        // round the corners on the image view
        self.pollImageView.layer.cornerRadius = 90.0;
        self.pollImageView.clipsToBounds = YES;
        
        // add overlay view
        UIView *overlayView = [[UIView alloc] initWithFrame:CGRectMake(0,
                                                                       (self.pollImageView.frame.size.height / 3) * 2,
                                                                       self.pollImageView.frame.size.width,
                                                                       self.pollImageView.frame.size.height / 3)];
        [overlayView setBackgroundColor:[UIColor whiteColor]];
        [overlayView setAlpha:0.70f];
        
        // setup title label on overlay view
        self.questionLabel = [[UILabel alloc] initWithFrame:CGRectMake(CellLabelPadding, CellLabelPadding,
                                                                       CGRectGetWidth(overlayView.frame) - (CellLabelPadding * 2),
                                                                       CGRectGetHeight(overlayView.frame) - (CellLabelPadding * 2))];
        self.questionLabel.numberOfLines = 2;
        [self.questionLabel setTextColor:[UIColor blackColor]];
        [self.questionLabel setBackgroundColor:[UIColor clearColor]];
        self.questionLabel.font = [UIFont preferredFontForTextStyle:UIFontTextStyleBody];
        
        // add label to overlay view
        [overlayView addSubview:self.questionLabel];
        
        // add views to content view
        [self.contentView addSubview:self.pollImageView];
        [self.contentView addSubview:overlayView];

    }
    
    return self;
}

- (void)prepareForReuse
{
    [super prepareForReuse];
    
    // set some outlets to nil
    self.pollImageView.image = nil;
    self.questionLabel.text = @"";
}


@end
