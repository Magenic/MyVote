//
//  User.h
//  MyVoteLV
//
//  Created by Kasey Schindler on 3/5/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

@interface User : NSObject

// naming conventions aren't ideal but they align with API
@property NSInteger UserId;
@property (strong, nonatomic) NSString *BirthDate;
@property (strong, nonatomic) NSDate *dateOfBirth;
@property (strong, nonatomic) NSString *EmailAddress;
@property (strong, nonatomic) NSString *FirstName;
@property (strong, nonatomic) NSString *LastName;
@property (strong, nonatomic) NSString *Gender;
@property (strong, nonatomic) NSString *PostalCode;
@property (strong, nonatomic) NSString *ProfileID;
@property (strong, nonatomic) NSString *Username;

- (instancetype)initUserWithDictionary:(NSDictionary *)dict;
- (NSString *)getDisplayName;
- (NSString *)readableGender;

@end
