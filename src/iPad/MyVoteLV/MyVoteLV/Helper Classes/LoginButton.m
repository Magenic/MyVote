//
//  LoginButton.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/8/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "LoginButton.h"

@interface LoginButton ()

@property (strong, nonatomic) UIImageView *loginImageView;

@end

@implementation LoginButton

- (id)initWithCoder:(NSCoder *)aDecoder
{
    self = [super initWithCoder:aDecoder];
    if (self) {
        // init image view in button
        self.loginImageView = [[UIImageView alloc] initWithFrame:CGRectMake(0, 0, 50, 50)];
        [self addSubview:self.loginImageView];
    }
    
    return self;
}

- (void)setLoginImage:(UIImage *)loginImage
{
    self.loginImageView.image = loginImage;
}

@end
