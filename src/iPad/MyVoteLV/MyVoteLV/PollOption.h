//
//  PollOption.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/16/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

/**
 * This represents an option (or possible answer) for a poll
 */

#import "CommonMethods.h"

@interface PollOption : NSObject

@property NSInteger pollOptionID;
@property BOOL isOptionSelected;
@property NSInteger optionPosition;
@property NSInteger pollResponseID;
@property (strong, nonatomic) NSString *optionText;

- (instancetype)initPollOptionWithDictionary:(NSDictionary *)dictionary;

@end
