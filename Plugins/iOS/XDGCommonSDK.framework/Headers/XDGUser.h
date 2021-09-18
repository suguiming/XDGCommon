
#import <Foundation/Foundation.h>
#import <XDGCommonSDK/XDGAccessToken.h>
#import <XDGCommonSDK/XDGEntryType.h>

NS_ASSUME_NONNULL_BEGIN

@interface XDGUser : NSObject <NSCoding>
/**
The user's user ID.
*/
@property (nonatomic,copy,readonly) NSString *userId;

/**
The user's user name.
*/
@property (nonatomic,copy,readonly) NSString *name;
/**
The user's current loginType.
*/
@property (nonatomic,copy,readonly) LoginEntryType loginType;
/**
The user's bound accounts. eg.@[@"TAPTAP",@"GOOGLE",@"FACEBOOK"]
*/
@property (nonatomic,copy,readonly) NSArray<NSString *> *boundAccounts;
/**
The user need push service or not
*/
@property (nonatomic,assign,readonly,getter=isPushServiceEnable) BOOL pushServiceEnable;

/**
The user's token.
*/
@property (nonatomic,strong,readonly) XDGAccessToken *token;
/// The current user profile
+ (XDGUser *)currentUser;

+ (void)clearCurrentUser;

- (instancetype)initWithUserID:(NSString *)userID
                          name:(nullable NSString *)name
                     loginType:(LoginEntryType)loginType
               boundAccounts:(NSArray *)boundAccounts
                         token:(XDGAccessToken *)token;

- (instancetype)initWithUserID:(NSString *)userID
                          name:(nullable NSString *)name
                     loginType:(LoginEntryType)loginType
                 boundAccounts:(NSArray *)boundAccounts
                         token:(XDGAccessToken *)token
             pushServiceEnable:(BOOL)enable;

@end

NS_ASSUME_NONNULL_END
