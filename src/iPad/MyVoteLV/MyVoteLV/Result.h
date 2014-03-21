//
//  Result.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/16/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

#import "CommonMethods.h"

@interface Result : NSObject

@property NSInteger pollID;
@property (strong, nonatomic) NSString *question;
@property (strong, nonatomic) NSArray *pollOptionResults;
@property (strong, nonatomic) NSArray *comments;

- (instancetype)initResultFromDictionary:(NSDictionary *)dictionary;

@end
