using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public struct Category
{
    public List<Value> list;

    [Serializable]
    public struct Value
    {
        public string name;
        public string code;
        public string url;
    }
}

[Serializable]
public struct Banner
{
    public List<Value> list;

    [Serializable]
    public struct Value
    {
        public string imageUrl;
        public string webUrl;
    }
}

public class CategoryManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] sector = new RectTransform[4];

    [SerializeField]
    private GameObject loding;
    [SerializeField]
    private Canvas categoryCanvas;
    [SerializeField]
    private Canvas apiCanvas;
    [SerializeField]
    private Canvas pictureCanvas;
    [SerializeField]
    private ScrollRect scroll;
    [SerializeField]
    private RectTransform prefab;

    [SerializeField]
    private ApiManager apiManager;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private GameObject start;

    void Awake()
    {
        if(categoryCanvas.enabled == false)
        {
            categoryCanvas.enabled = true;
        }
    }

    void Start()
    {
        CategoryEvent().Forget();
    }

    private async UniTask CategoryEvent()
    {
        try
        {
            loding.SetActive(true);

            string url = "";
            float location = 0;
            for (int i = 0; i < sector.Length; i++)
            {
                switch (i)
                {

                    case 0:
                        url = $"http://decopop.ganpandirect.com/deco_api/category.php?mode=api&site={Application.identifier}&code=category";
                        using (UnityWebRequest request = UnityWebRequest.Get(url))
                        {
                            await request.SendWebRequest();
                            Category site = JsonConvert.DeserializeObject<Category>(request.downloadHandler.text);

                            float division = site.list.Count * 0.5f;
                            if (division == (int)division)
                            {
                                division = (int)division;
                            }
                            else
                            {
                                division = (int)division + 1;
                            }

                            sector[i].anchoredPosition = Vector3.zero;
                            sector[i].sizeDelta = new Vector2(1280f, 120f + (490f * division));
                            location = sector[i].anchoredPosition.y - sector[i].sizeDelta.y - 80f;

                            CategorySectorEvent(site);
                        }
                        break;

                    case 1:
                        sector[i].anchoredPosition = new Vector3(0, location, 0);
                        sector[i].sizeDelta = new Vector2(1280f, 120f + 490f);
                        location = sector[i].anchoredPosition.y - sector[i].sizeDelta.y - 80f;
                        break;

                    case 2:
                        url = $"http://decopop.ganpandirect.com/deco_api/banner.php?mode=dev&site={Application.identifier}";
                        using (UnityWebRequest request = UnityWebRequest.Get(url))
                        {
                            await request.SendWebRequest();
                            Banner banner = JsonConvert.DeserializeObject<Banner>(request.downloadHandler.text);
                            sector[i].anchoredPosition = new Vector3(0, location, 0);
                            sector[i].sizeDelta = new Vector2(1280f, 120f + (490f * banner.list.Count));
                            location = sector[i].anchoredPosition.y - sector[i].sizeDelta.y - 80f;

                            CategoryBannerEvent(banner);
                        }
                        break;

                    case 3:
                        sector[i].anchoredPosition = new Vector3(0, location, 0);
                        sector[i].sizeDelta = new Vector2(1280f, 200f);
                        location = sector[i].anchoredPosition.y - sector[i].sizeDelta.y - 80f;
                        break;
                }
            }

            scroll.content.sizeDelta = new Vector2(1280f, -location);
        }
        finally
        {
            await UniTask.Delay(800);
            loding.SetActive(false);
        }
    }

    private void CategorySectorEvent(Category site)
    {
        int index = 0;
        int location = 0;
        for (int i = 0; i < site.list.Count; i++)
        {            
            CategoryInstantiateEvent(site.list[i], index, location).Forget();

            location++;
            if(location == 2)
            {
                index++;
                location = 0;
            }
        }
    }

    private async UniTask CategoryInstantiateEvent(Category.Value value, int index, int location)
    {
        RectTransform obj = Instantiate(prefab, sector[0]);
        obj.anchoredPosition = new Vector3((680f * location), -120f - (490f * index), 0);
        obj.sizeDelta = new Vector2(600f, 450f);
        PrefabEvent ce = obj.GetComponent<PrefabEvent>();

        ce.button.onClick.AddListener(() =>
        {

            CanvasEvent(1);
            apiManager.ApiEvent(1, value.code);            

        });
        ce.text.text = value.name;
        
        using(UnityWebRequest request = UnityWebRequestTexture.GetTexture(value.url))
        {
            await request.SendWebRequest();
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            ce.image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            ce.image.color = Color.white;
        }
    }

    public void CanvasEvent(int selectNumber)
    {
        categoryCanvas.enabled = false;

        if (start.activeSelf == true)
        {
            start.SetActive(false);
        }

        if (selectNumber == 1)
        {
            uiManager.apiEnable = true;
            uiManager.OpenCanvasEvent();
        }
        else
        {
            uiManager.pictureEnable = true;
            uiManager.OpenCanvasEvent();
        }
    }

    private void CategoryBannerEvent(Banner banner)
    {
        int index = 0;
        int location = 0;
        for (int i = 0; i < banner.list.Count; i++)
        {
            CategoryBannerEvent(banner.list[i], index, location).Forget();

            index++;
        }
    }

    private async UniTask CategoryBannerEvent(Banner.Value value, int index, int location)
    {
        RectTransform obj = Instantiate(prefab, sector[2]);
        obj.anchoredPosition = new Vector3(0, -120f - (490f * index), 0);
        obj.sizeDelta = new Vector2(1280f, 450f);
        PrefabEvent ce = obj.GetComponent<PrefabEvent>();

        ce.button.onClick.AddListener(() =>
        {

            OpenUrlEvent(value.webUrl);

        });
        ce.text.text = "";

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(value.imageUrl))
        {
            await request.SendWebRequest();
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            ce.image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            ce.image.color = Color.white;
        }
    }

    private void OpenUrlEvent(string url)
    {
        if(string.IsNullOrEmpty(url))
        {
#if UNITY_ANDROID
            string message = "이벤트 준비중...";
            AndroidToestPopupEvent(message);
#elif UNITY_IOS

#endif
        }
        else
        {
            Application.OpenURL(url);
        }
    }

    private void AndroidToestPopupEvent(string message)
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
