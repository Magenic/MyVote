//
//  MyVoteService.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/11/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

#import "MyVoteService.h"
#import "Constants.h"

@implementation MyVoteService

- (MyVoteService *)init
{
    self = [super init];
    
    if (self) {
        // create client for this service
        NSURL *url = [NSURL URLWithString:kURL];
        self.client = [MSClient clientWithApplicationURL:url applicationKey:kAppKey];
    }
    
    return self;
}

- (void)addImage:(NSDictionary *)itemDict completion:(completionWithIndexBlock)completion
{
    MSTable *activeUsersTable = [self.client tableWithName:@"ActiveUsers"];
    
    [activeUsersTable insert:itemDict completion:^(NSDictionary *result, NSError *error) {
        NSNumber *blobID = [result objectForKey:@"id"];
        completion(blobID.integerValue);
    }];
}

- (void)getImageWithID:(NSNumber *)blobID completion:(imageCompletionBlock)completion
{
    MSTable *activeUsersTable = [self.client tableWithName:@"ActiveUsers"];
    [activeUsersTable readWithId:blobID completion:^(NSDictionary *item, NSError *error) {
        completion([item objectForKey:@"resourceName"]);
    }];
}

#pragma mark - Class Method(s)

+ (MyVoteService *)sharedInstance
{
    static MyVoteService *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[self alloc] init];
    });
    
    return sharedInstance;
}

@end
