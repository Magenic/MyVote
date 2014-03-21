//
//  Enums.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 1/16/14.
//  Copyright (c) 2014 Magenic. All rights reserved.
//

// NS_ENUM for poll category - fun, sports, technology, entertainment, news, off-topic
typedef NS_ENUM(NSInteger, PollCategoryType) {
    PollCategoryTypeInvalid,
    PollCategoryTypeEntertainment,
    PollCategoryTypeFun,
    PollCategoryTypeNews,
    PollCategoryTypeOffTopic,
    PollCategoryTypeSports,
    PollCategoryTypeTechnology
};

@interface Enums : NSObject

+ (PollCategoryType)translateStringToPollCategoryType:(NSString *)category;

@end
