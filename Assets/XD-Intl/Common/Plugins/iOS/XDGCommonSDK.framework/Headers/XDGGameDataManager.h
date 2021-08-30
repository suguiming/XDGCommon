
#import <Foundation/Foundation.h>

@class TDSGlobalGame;

NS_ASSUME_NONNULL_BEGIN
@interface XDGGameDataManager : NSObject
+ (XDGGameDataManager *)shareInstance;

+ (TDSGlobalGame *)currentGameData;
+ (NSArray *)currentLoginEntries;
+ (NSArray *)currentBindEntries;
+ (NSString *)serviceTermsUrl;
+ (NSString *)serviceAgreementUrl;
+ (NSString *)californiaPrivacyUrl;
+ (NSArray *)gameLogos;
+ (NSString *)tapServerUrl;
+ (NSDictionary *)configData;
+ (NSDictionary *)configTapDict;


+ (void)setLanguageLocale:(NSInteger)locale;

+ (void)getClientConfig:(void (^)(BOOL success))handler;

/// 是否已经初始化
+ (BOOL)isGameInited;
/// 是否需要客服
+ (BOOL)needReportService;
#pragma mark - configs
+ (BOOL)isGameInKorea;
+ (BOOL)isGameInNA;

+ (BOOL)googleEnable;
+ (BOOL)facebookEnable;
+ (BOOL)twitterEnable;
+ (BOOL)taptapEnable;
+ (BOOL)adjustEnable;
+ (BOOL)appsflyersEnable;
+ (BOOL)lineEnable;
+ (BOOL)tapDBEnable;
@end

NS_ASSUME_NONNULL_END
