//
//  CommetReplyViewController.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/12/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "CommentReplyViewController.h"
#import "Comment.h"

static const CGFloat kLabelPadding = 10.0;
static const CGFloat kViewHeight = 300.0;
static const CGFloat kViewWidth = 400.0;

@interface CommentReplyViewController () <UITextViewDelegate>

@end

@implementation CommentReplyViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
	// Do any additional setup after loading the view.
    
    // setup view
    [self setupView];
}

- (void)setupView
{
    // set label text
    UILabel *commentLabel = [[UILabel alloc] initWithFrame:CGRectMake(5, kLabelPadding, kViewWidth - kLabelPadding, 0)];
    commentLabel.numberOfLines = 0;
    commentLabel.text = [NSString stringWithFormat:@"%@: %@", self.comment.username, self.comment.commentText];
    commentLabel.textColor = [UIColor lightGrayColor];
    
    // set based on the length of the comment text
    CGRect boundingRect = [commentLabel.text boundingRectWithSize:CGSizeMake(CGRectGetWidth(commentLabel.frame), CGFLOAT_MAX)
                                                            options:NSStringDrawingUsesLineFragmentOrigin
                                                         attributes:@{NSFontAttributeName : [UIFont systemFontOfSize:15.0]}
                                                            context:nil];
    
    // height of username + comment + date
    CGFloat labelHeight = boundingRect.size.height + kLabelPadding;
    CGRect frame = commentLabel.frame;
    frame.size.height = labelHeight;
    commentLabel.frame = frame;
    [self.view addSubview:commentLabel];

    // add one pixel high separator
    UIView *separatorView = [[UIView alloc] initWithFrame:CGRectMake(0, CGRectGetMaxY(commentLabel.frame) + kLabelPadding,
                                                                     kViewWidth, 1)];
    separatorView.backgroundColor = [UIColor lightGrayColor];
    [self.view addSubview:separatorView];

    // now configure textview
    UITextView *textView = [[UITextView alloc] initWithFrame:CGRectMake(0, 0, kViewWidth, 0)];
    textView.delegate = self;
    textView.font = [UIFont systemFontOfSize:15.0];
    textView.returnKeyType = UIReturnKeySend;
    frame = textView.frame;
    frame.origin.y = CGRectGetMaxY(separatorView.frame) + CGRectGetHeight(separatorView.frame) + kLabelPadding;
    frame.size.height = kViewHeight - frame.origin.y;
    textView.frame = frame;
    
    [self.view addSubview:textView];
    
    //
    [textView becomeFirstResponder];
}

#pragma mark - UITextViewDelegate

- (BOOL)textView:(UITextView *)textView shouldChangeTextInRange:(NSRange)range replacementText:(NSString *)text
{
    // check if /n character has been sent since this means return key tapped
    if ([text isEqualToString:@"\n"]) {
        // submit comment
        [self.delegate replyToComment:self.comment withCommentText:textView.text];
        
        // dismiss textview
        [textView resignFirstResponder];
        
        // don't allow character to be entered
        return NO;
    }
    
    return YES;
}

- (void)textViewDidBeginEditing:(UITextView *)textView
{
    if ([textView.text isEqualToString:NSLocalizedString(@"Reply to comment...", nil)]) {
        textView.text = @"";
        textView.textColor = [UIColor blackColor];
    }
    
    [textView becomeFirstResponder];
}

- (void)textViewDidEndEditing:(UITextView *)textView
{
    if ([textView.text isEqualToString:@""]) {
        textView.text = NSLocalizedString(@"Reply to comment...", nil);
        textView.textColor = [UIColor lightGrayColor]; //optional
    }
    
    [textView resignFirstResponder];
}

@end
