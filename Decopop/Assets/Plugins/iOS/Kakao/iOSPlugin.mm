#import <KakaoOpenSDK/KakaoOpenSDK.h>

extern "C" {
    void _KakaoSimpleSignIn()
    {
        [[KOSession sharedSession] close];

            [[KOSession sharedSession] openWithCompletionHandler:^(NSError *error) {
                
                if (error) {
                    NSLog(@"login failed. - error: %@", error);
                }
                else {
                    NSLog(@"login succeeded.");
                }

            }authType:(KOAuthType)KOAuthTypeTalk, nil];
        }

    void _KakaoSignIn()
    {
        // Close old session
        if ( ! [[KOSession sharedSession] isOpen] ) {
            NSLog(@"in isOpen condition");
            [[KOSession sharedSession] close];
            NSLog(@"Old session closed");
        }

        // session open with completion handler
        [[KOSession sharedSession] openWithCompletionHandler:^(NSError *error) {
            if (error) {
                NSLog(@"login failed. - error: %@", error);
            }
            else {
                NSLog(@"login succeeded.");
            }
            

            // get user info
            [KOSessionTask userMeTaskWithCompletion:^(NSError *error, KOUserMe *me) {
                if (error){
                    NSLog(@"get user info failed. - error: %@", error);
                } else {
                    NSLog(@"get user info. - user info: %@", me);
                    
                    UnitySendMessage("SignManager", "KakaoSNS", "iOS");

                    if(me.ID != nil){
                        UnitySendMessage("SignManager", "KakaoID", [me.ID UTF8String]);
                    }

                    if(me.account.profile.nickname != nil){
                        UnitySendMessage("SignManager", "KakaoName", [me.account.profile.nickname UTF8String]);
                    }

                    if(me.account.email != nil){
                        UnitySendMessage("SignManager", "KakaoEmail", [me.account.email UTF8String]);
                    }

                    if(me.account.profile.profileImageURL != nil){
                        UnitySendMessage("SignManager", "KakaoProfile", [me.account.profile.profileImageURL.absoluteString UTF8String]);
                    }

                    UnitySendMessage("SignManager", "SignEvent", "");
                }
            }];
        }];
    }

    void _KakaoSignOut()
    {
        [[KOSession sharedSession] logoutAndCloseWithCompletionHandler:^(BOOL success, NSError *error){
            if (error){
                NSLog(@"failed to logout. - error: %@", error);
            }
            else
            {
                NSLog(@"logout success");
            }
        }];
    }

    void _KakaoUnlink()
    {
        [KOSessionTask unlinkTaskWithCompletionHandler:^(BOOL success, NSError *error) {
            if(error){
                NSLog(@"unlink logout. - error: %@", error);
            }
            else {
                NSLog(@"unlink succeeded.");
            }
        }];
    }

    void _KakaoGetToken()
    {
        [KOSessionTask accessTokenInfoTaskWithCompletionHandler:^(KOAccessTokenInfo *accessTokenInfo, NSError *error) {
                if (error) {
                    switch (error.code) {
                        case KOErrorDeactivatedSession:
                            NSLog(@"세션이 만료된(access_token, refresh_token이 모두 만료된 경우) 상태");
                            break;
                        default:
                            NSLog(@"예기치 못한 에러. 서버 에러");
                            break;
                    }
                } else {
                    // 성공 (토큰이 유효함)
                    NSLog(@"success request - access token info:  %@", accessTokenInfo);
                }
        }];
    }
}
