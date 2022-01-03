using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonManager : MonoBehaviour
{
    private int click = 0;
    private bool timerEnable = false;
    private float timer = 0f;

    [SerializeField]
    private Canvas category;
    [SerializeField]
    private Canvas capture;

    void Update()
    {
#if UNITY_ANDROID
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(category.enabled == true)
            {
                category.enabled = false;
            }
            else if(capture.enabled == true)
            {
                capture.enabled = false;
            }
            else
            {
                timerEnable = true;
                click++;
                AndroidToastPopupEvent();
            }
        }

        if (timerEnable == true)
        {
            timer += Time.deltaTime;

            if (click == 2 && timer <= 2f)
            {
                Application.Quit();
            }

            if (click == 2 && timer > 2f)
            {
                click = 1;
                timer = 0;
            }
        }

#endif
    }

    private void AndroidToastPopupEvent()
    {
        string message = "'뒤로'버튼 한번더 클릭시 앱이 종료됩니다.";

        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 3);
                toastObject.Call("show");
            }));
        }
    }
}
