using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
#if UNITY_ANDROID
        ApplicationChrome.statusBarState = ApplicationChrome.navigationBarState = ApplicationChrome.States.VisibleOverContent;
        ApplicationChrome.statusBarColor = ApplicationChrome.navigationBarColor = 0xFF111111;
#endif
    }
}
