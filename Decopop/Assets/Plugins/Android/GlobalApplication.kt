package com.unity3d.player

import android.app.Application
import com.kakao.sdk.common.KakaoSdk
import com.kakao.sdk.common.util.Utility

class GlobalApplication : Application() {

	override fun onCreate() {
        super.onCreate()
        KakaoSdk.init(this, "5ed4dbecdbd90ffb0edc16fe8fe79381")
    }
}