//
//  TDSGlobalSDKLoginService.h
//  XDGAccountSDK
//
//  Created by JiangJiahao on 2020/11/23.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface XDGLoginService : NSObject
+ (void)login:(void (^)(NSString *result))callback;
+ (void)logout;

+ (void)loginSync:(void(^)(NSString *result))callback;

+ (void)addUserStatusChangeCallback:(void (^)(NSString *result))callback;

+ (void)getUser:(void (^)(NSString *result))callback;

+ (void)openUserCenter;

+ (void)loginByType:(NSString *)loginType bridgeCallback:(void (^)(NSString * _Nonnull))callback;

@end

NS_ASSUME_NONNULL_END
