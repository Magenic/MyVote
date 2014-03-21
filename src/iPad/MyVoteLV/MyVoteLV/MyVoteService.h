//
//  MyVoteService.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 2/11/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

#import <WindowsAzureMobileServices/WindowsAzureMobileServices.h>

typedef void (^completionWithIndexBlock) (NSInteger blobID);
typedef void (^imageCompletionBlock) (NSString *imageDataString);

@interface MyVoteService : NSObject

@property (strong, nonatomic) MSClient *client;

- (void)addImage:(NSDictionary *)itemDict completion:(completionWithIndexBlock)completion;

- (void)getImageWithID:(NSNumber *)blobID completion:(imageCompletionBlock)completion;

+ (MyVoteService *)sharedInstance;

@end
