using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 프레임 및, 안드로이드 상태바 변경
    void Awake()
    {
        Application.targetFrameRate = 60;
#if UNITY_ANDROID
        ApplicationChrome.statusBarState = ApplicationChrome.navigationBarState = ApplicationChrome.States.VisibleOverContent;
        ApplicationChrome.statusBarColor = ApplicationChrome.navigationBarColor = 0xFF111111;
#endif
    }
}
