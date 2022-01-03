using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public struct User
{
    public string sns;
    public string token;
    public string id;
    public string email;
    public string phone;
    public string name;
    public string profile;
    public string firebase;
    public string member;
}

[Serializable]
public struct Result
{
    public UserData user;

    [Serializable]
    public struct UserData
    {
        public string member_type;
        public string member_code;
        public string name;
        public string member_status;
    }
}

public class SignManager : MonoBehaviour
{
    private User user = new User();
    private Firebase.FirebaseApp app = null;

    [SerializeField]
    private GameObject loding;

    void Awake()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {

            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                app = Firebase.FirebaseApp.DefaultInstance;

                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

                user.firebase = Firebase.Messaging.FirebaseMessaging.GetTokenAsync().Result;
            }
        });
    }

    private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {

    }

    private void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {

    }

    public void KakaoSignEvent()
    {
#if UNITY_ANDROID
        AndroidJavaObject plugin = new AndroidJavaObject("com.unity3d.player.KakaoAuth");
            plugin.Call("KakaoSignIn");
#endif
    }

    private void KakaoError(string error)
    {
        string message = "로그인 중 에러가 발생하였습니다.";

#if UNITY_ANDROID
        AndroidToastPopupEvent(message);
#endif

    }
    
    private void KakaoToken(string token)
    {
        user.token = token;
    }

    private void KakaoSNS(string sns)
    {
        user.sns = sns;
    }

    private void KakaoID(string id)
    {
        user.id = id;
    }

    private void KakaoEmail(string email)
    {
        user.email = email;
    }

    private void KakaoName(string name)
    {
        user.name = name;
    }

    private void KakaoProfile(string profile)
    {
        user.profile = profile;
    }

    private void KakaoPhone(string phone)
    {
        user.phone = phone;
    }

    private void AndroidToastPopupEvent(string message)
    {
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

    private void SignEvent()
    {
        loding.SetActive(true);

        StringBuilder sign = new StringBuilder();
        sign.Append("http://decopop.ganpandirect.com/login/sns_process.php?");
        sign.Append("action=login&from=1&sns_section=5");
        sign.Append("&sns_id=" + user.id);
        sign.Append("&sns_name=" + user.name);
        sign.Append("&sns_token=" + user.token);
        sign.Append("&sns_email=" + user.email);
        sign.Append("&sns_member_type=1");
        sign.Append("&phone_token=" + user.firebase);

#if UNITY_ANDROID
        sign.Append("&phone_os=1");
#elif UNITY_IOS
        sign.Append("&phone_os=2");
#endif

        sign.Append("&device_token=" + SystemInfo.deviceUniqueIdentifier);
        sign.Append("&sns_phone=" + user.phone);

        StartCoroutine(PostRequest(sign.ToString(), (UnityWebRequest request) =>
        {

            try
            {
                Result result = JsonConvert.DeserializeObject<Result>(request.downloadHandler.text);

                string member = result.user.member_code;

                if (result.user.member_status.Equals("OK"))
                {
                    SceneManager.LoadScene("Main");
                }
            }
            catch
            {
                SceneManager.LoadScene("Main");
            }
            finally
            {
                loding.SetActive(false);
            }

        }));
    }

    private IEnumerator PostRequest(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            callback(request);
        }
    }
}
