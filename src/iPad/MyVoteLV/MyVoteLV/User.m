//
//  User.m
//  MyVoteLV
//
//  Created by Kasey Schindler on 3/5/13.
//  Copyright (c) 2013 Magenic. All rights reserved.
//

#import "User.h"
#import "CommonMethods.h"

@implementation User

- (instancetype)initUserWithDictionary:(NSDictionary *)dict
{
    self = [super init];
    
    if (self) {
        // setup formatter to handle returned date
        NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
        [dateFormatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ss"];
        NSDate *date = [dateFormatter dateFromString:[dict objectForKey:@"BirthDate"]];
        self.dateOfBirth = date;
        
        // change formatter
        [dateFormatter setDateStyle:NSDateFormatterLongStyle];
        self.BirthDate = [dateFormatter stringFromDate:date];
        
        // set other fields
        self.EmailAddress = [dict objectForKey:@"EmailAddress"];
        self.FirstName = [dict objectForKey:@"FirstName"];
        self.LastName = [dict objectForKey:@"LastName"];
        self.Gender = [dict objectForKey:@"Gender"];
        self.PostalCode = [dict objectForKey:@"PostalCode"];
        self.ProfileID = [dict objectForKey:@"ProfileID"];
        self.Username = [dict objectForKey:@"UserName"];
        
        // User ID
        NSNumber *holder = [dict objectForKey:@"UserID"];
        self.UserId = [CommonMethods validValueForJSONNumber:holder];
    }
    
    return self;
}

- (NSString *)getDisplayName
{
    return [NSString stringWithFormat:@"%@ %@", self.FirstName, self.LastName];
}

- (NSString *)readableGender
{
    if ([self.Gender isEqualToString:@"M"] || [self.Gender isEqualToString:NSLocalizedString(@"Male", nil)]) {
        return NSLocalizedString(@"Male", nil);
    } else {
        return NSLocalizedString(@"Female", nil);
    }
}

- (NSString *)description
{
    return [NSString stringWithFormat:@"%@, %@, %@, %@, %d, %@, %@", [self getDisplayName], self.EmailAddress, self.Gender, self.Username, self.UserId, self.ProfileID, self.BirthDate];
}


@end
