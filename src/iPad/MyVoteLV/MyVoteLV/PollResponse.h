//
//  PollResponse.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/29/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface PollResponse : NSObject

@property (strong, nonatomic) NSString *optionText;
@property NSInteger pollOptionID;
@property NSInteger responseCount;

- (instancetype)initPollResponseWithDictionary:(NSDictionary *)dictionary;

@end
