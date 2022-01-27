using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using System.Security.Cryptography;

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
    private IAppleAuthManager appleAuthManager;

    [SerializeField]
    private GameObject loding;

    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {

            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

                user.firebase = Firebase.Messaging.FirebaseMessaging.GetTokenAsync().Result;
            }
        });

#if UNITY_IOS

        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            var deserializer = new PayloadDeserializer();
            appleAuthManager = new AppleAuthManager(deserializer);
        }

#endif

    }

    private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {

    }

    private void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {

    }

#if UNITY_IOS

    void Update()
    {
        if (appleAuthManager != null)
        {
            appleAuthManager.Update();
        }
    }

#endif

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

    /*
 * [ iOS SignIn ]
 */

    public void AppleSignInEvent()
    {
#if UNITY_IOS
        var rawNonce = GenerateRandomString(32);
        var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);

        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName, nonce);

        appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential => {

                // Obtained credential, cast it to IAppleIDCredential
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    user.sns = "Apple";
                    user.id = appleIdCredential.User;
                    user.name = "Apple";
                    user.email = appleIdCredential.Email;
                    user.token = Encoding.UTF8.GetString(
                        appleIdCredential.IdentityToken,
                        0,
                        appleIdCredential.IdentityToken.Length);
                }

                SignEvent();
            },
            error => {

                var authorizationErrorCode = error.GetAuthorizationErrorCode();

            });
#endif
    }

    private string GenerateRandomString(int length)
    {
        const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
        var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
        var result = string.Empty;
        var remainingLength = length;

        var randomNumberHolder = new byte[1];
        while (remainingLength > 0)
        {
            var randomNumbers = new List<int>(16);
            for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
            {
                cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
                randomNumbers.Add(randomNumberHolder[0]);
            }

            for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
            {
                if (remainingLength == 0)
                {
                    break;
                }

                var randomNumber = randomNumbers[randomNumberIndex];
                if (randomNumber < charset.Length)
                {
                    result += charset[randomNumber];
                    remainingLength--;
                }
            }
        }

        return result;
    }

    private string GenerateSHA256NonceFromRawNonce(string rawNonce)
    {
        var sha = new SHA256Managed();
        var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
        var hash = sha.ComputeHash(utf8RawNonce);

        var result = string.Empty;
        for (var i = 0; i < hash.Length; i++)
        {
            result += hash[i].ToString("x2");
        }

        return result;
    }

    private void SignEvent()
    {
        if(loding != null)
        {
            if(loding.activeSelf == false)
            {
                loding.SetActive(true);
            }
        }
        
        StringBuilder sign = new StringBuilder();
        sign.Append("http://decopop.ganpandirect.com/login/sns_process.php?");
#if UNITY_ANDROID
        sign.Append("action=login&from=1&sns_section=5");
#elif UNITY_IOS
        sign.Append("action=login&from=1&sns_section=6");
#endif
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
                PlayerPrefs.SetString("Member", result.user.member_code);
                if (result.user.member_status.Equals("OK"))
                {
                    SceneManager.LoadScene("Main");
                }
            }
            finally
            {
                LogEvent(user.sns + " Login");
                if (loding != null)
                {
                    if (loding.activeSelf == true)
                    {
                        loding.SetActive(false);
                    }
                }
            }

        }));
    }

    private IEnumerator LogEvent(string log)
    {
        string member = PlayerPrefs.GetString("Member");
        string url = $"http://decopop.ganpandirect.com/log/use_log.php?m=input&m_code={member}&b={log}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
        }
    }

    private IEnumerator PostRequest(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            callback(request);
        }
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
}
