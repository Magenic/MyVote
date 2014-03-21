//
//  SubmittedResponse.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/29/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

@interface SubmittedResponse : NSObject

@property NSInteger pollID;
@property NSInteger userID;
@property (strong, nonatomic) NSString *comment;
@property (strong, nonatomic) NSArray *responseItems;

- (NSDictionary *)serializeObjectForJSON;

@end
