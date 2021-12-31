package com.unity3d.player

import com.kakao.sdk.auth.model.OAuthToken
import com.kakao.sdk.user.UserApiClient
import com.unity3d.player.UnityPlayer

class KakaoAuth {

    fun KakaoSignIn() {
        val callback: (OAuthToken?, Throwable?) -> Unit = { token, error ->
            if (error != null) {
                UnityPlayer.UnitySendMessage("SignManager", "KakaoError", "")
            } else if (token != null) {
                UnityPlayer.UnitySendMessage("SignManager", "KakaoToken", "${token.accessToken}")
                KakaoUserInfo()
            }
        }

        // 카카오톡이 설치되어 있으면 카카오톡으로 로그인, 아니면 카카오계정으로 로그인
        if (UserApiClient.instance.isKakaoTalkLoginAvailable(UnityPlayer.currentActivity)) {
            UserApiClient.instance.loginWithKakaoTalk(
                UnityPlayer.currentActivity,
                callback = callback
            )
        } else {
            UserApiClient.instance.loginWithKakaoAccount(
                UnityPlayer.currentActivity,
                callback = callback
            )
        }
    }

    fun KakaoSignOut() {
        UserApiClient.instance.logout { error ->
            if (error != null) {
                UnityPlayer.UnitySendMessage("SignManager", "KakaoError", "")
            } else {
                UnityPlayer.UnitySendMessage("SignManager", "", "logout")
            }
        }
    }

    fun KakaoUnlink() {
        UserApiClient.instance.unlink { error ->
            if (error != null) {
                
            } else {
                
            }
        }
    }


    fun KakaoUserInfo(){
        UserApiClient.instance.me { user, error ->
            if (error != null) {
                UnityPlayer.UnitySendMessage("SignManager", "KakaoError", "user")
            }
            else if (user != null) {
				UnityPlayer.UnitySendMessage("SignManager", "KakaoSNS", "Kakao")
                UnityPlayer.UnitySendMessage("SignManager", "KakaoID", "${user.id}")
                UnityPlayer.UnitySendMessage("SignManager", "KakaoEmail", "${user.kakaoAccount?.email}")
                UnityPlayer.UnitySendMessage("SignManager", "KakaoName", "${user.kakaoAccount?.profile?.nickname}")
                UnityPlayer.UnitySendMessage("SignManager", "KakaoProfile", "${user.kakaoAccount?.profile?.thumbnailImageUrl}")
				UnityPlayer.UnitySendMessage("SignManager", "KakaoPhone", "${user.kakaoAccount?.phoneNumber}")
				
				UnityPlayer.UnitySendMessage("SignManager", "SignEvent", "")
            }
        }
    }
}