using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public struct Product
{
    public List<Value> list;

    [Serializable]
    public struct Value
    {
        public string name;
        public string dimension;
        public string atlas;
        public string thumbnail;
        public string ar;
        public string url;
        public string content;
        public string code;
    }
}

public class ApiManager : MonoBehaviour
{
    private string categoryCode = "";
    private List<PrefabEvent> categoryList = new List<PrefabEvent>();
    [SerializeField]
    private ScrollRect categoryScroll;
    [SerializeField]
    private RectTransform categoryPrefab;
    
    private int currentIndex = 0;
    private Color fristColor;
    private Color secondColor;

    private string productCode = "";
    private List<PrefabEvent> productList = new List<PrefabEvent>();
    [SerializeField]
    private ScrollRect productScroll;
    [SerializeField]
    private RectTransform productPrefab;

    private string currentUrl;
    [SerializeField]
    private ARManager manager;
    [SerializeField]
    private Text fristText;
    [SerializeField]
    private Text secondText;
    [SerializeField]
    private GameObject detail;

    void Awake()
    {
        ColorUtility.TryParseHtmlString("#202020", out fristColor);
        ColorUtility.TryParseHtmlString("#FFFFFF", out secondColor);
    }

    public void ApiEvent(int index, string code)
    {
        if(index == 1)
        {
            if (categoryCode == code)
            {
                return;
            }

            CategoryEvent(code).Forget();
        }
        else
        {
            if (productCode == code)
            {
                return;
            }

            ProductEvent(code).Forget();
        }     
    }

    private async UniTask CategoryEvent(string code)
    {
        categoryCode = code;
        string url = $"http://decopop.ganpandirect.com/deco_api/category.php?mode=api&site={Application.identifier}&code=" + categoryCode;

        using(UnityWebRequest request = UnityWebRequest.Get(url))
        {
            try
            {
                await request.SendWebRequest();
                Category category = JsonConvert.DeserializeObject<Category>(request.downloadHandler.text);

                if (category.list.Count > categoryList.Count)
                {
                    for(int i = 0; i < category.list.Count; i++)
                    {
                        if(i < categoryList.Count)
                        {
                            if(categoryList[i].gameObject.activeSelf == false)
                            {
                                categoryList[i].gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            RectTransform obj = Instantiate(categoryPrefab, categoryScroll.content);
                            categoryList.Add(obj.GetComponent<PrefabEvent>());
                        }
                    }
                }
                else
                {
                    for(int i = 0; i < categoryList.Count; i++)
                    {
                        if(i < category.list.Count)
                        {
                            if (categoryList[i].gameObject.activeSelf == false)
                            {
                                categoryList[i].gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            if (categoryList[i].gameObject.activeSelf == true)
                            {
                                categoryList[i].gameObject.SetActive(false);
                            }
                        }
                    }
                }

                int index = 0;
                int location = 0;
                for(int i = 0; i < category.list.Count; i++)
                {
                    CategorySettingEvent(category.list[i], index, location);

                    index++;
                    location += ((string.Concat(category.list[i].name.Where(x => !char.IsWhiteSpace(x))).Length + 2) * 40) + 40;                   
                }

                if (location < 1440f)
                {                    
                    if(categoryScroll.enabled == true)
                    {
                        categoryScroll.enabled = false;
                    }                    
                }
                else
                {
                    if(categoryScroll.enabled == false)
                    {
                        categoryScroll.enabled = true;
                    }
                }
                categoryScroll.horizontalNormalizedPosition = 0;
                categoryScroll.content.sizeDelta = new Vector2(location + 80, 160);

                CategoryUIEvent(0);
                ApiEvent(2, category.list[0].code);
            }
            finally
            {
                fristText.text = "";
                secondText.text = "";
            }
        }
    }

    private void CategorySettingEvent(Category.Value value, int index, int location)
    {
        PrefabEvent pe = categoryList[index];
        pe.rectTransform.anchoredPosition = new Vector3(location + 80, 0, 0);
        pe.rectTransform.sizeDelta = new Vector2((string.Concat(value.name.Where(x => !char.IsWhiteSpace(x))).Length + 2) * 40, 160);
        pe.text.text = value.name;
        pe.button.onClick.RemoveAllListeners();
        pe.button.onClick.AddListener(() =>
        {

            CategoryUIEvent(index);
            ApiEvent(2, value.code);

        });
    }

    private void CategoryUIEvent(int index)
    {
        categoryList[currentIndex].image.color = fristColor;
        categoryList[currentIndex].text.color = secondColor;

        currentIndex = index;

        categoryList[currentIndex].image.color = secondColor;
        categoryList[currentIndex].text.color = fristColor;
    }

    private async UniTask ProductEvent(string code)
    {
        productCode = code;
        string url = $"http://decopop.ganpandirect.com/deco_api/product.php?mode=api&site={Application.identifier}&category=" + productCode;

        using(UnityWebRequest request = UnityWebRequest.Get(url))
        {
            try
            {
                await request.SendWebRequest();
                Product product = JsonConvert.DeserializeObject<Product>(request.downloadHandler.text);

                if (product.list.Count > productList.Count)
                {
                    for (int i = 0; i < product.list.Count; i++)
                    {
                        if (i < productList.Count)
                        {
                            if (productList[i].gameObject.activeSelf == false)
                            {
                                productList[i].gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            RectTransform obj = Instantiate(productPrefab, productScroll.content);
                            productList.Add(obj.GetComponent<PrefabEvent>());
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < productList.Count; i++)
                    {
                        if (i < product.list.Count)
                        {
                            if (productList[i].gameObject.activeSelf == false)
                            {
                                productList[i].gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            if (productList[i].gameObject.activeSelf == true)
                            {
                                productList[i].gameObject.SetActive(false);
                            }
                        }
                    }
                }

                int index = 0;
                int location = 0;
                for(int i = 0; i < product.list.Count; i++)
                {
                    ProductSettingEvent(product.list[i], index, location).Forget();

                    index++;
                    location += 490;                    
                }

                if (location < 1440f)
                {
                    if (productScroll.enabled == true)
                    {
                        productScroll.enabled = false;
                    }
                }
                else
                {
                    if (productScroll.enabled == false)
                    {
                        productScroll.enabled = true;
                    }
                }
                productScroll.horizontalNormalizedPosition = 0;
                productScroll.content.sizeDelta = new Vector2(location + 80, 300);
            }
            finally
            {
                fristText.text = "";
                secondText.text = "";
            }
        }
    }

    private async UniTask ProductSettingEvent(Product.Value value, int index, int location)
    {
        PrefabEvent pe = productList[index];
        pe.rectTransform.anchoredPosition = new Vector3(location + 80, 0, 0);
        pe.rectTransform.sizeDelta = new Vector2(450, 300);
        pe.button.onClick.RemoveAllListeners();
        pe.button.onClick.AddListener(() =>
        {

            URLCheckEvent(value.url);
            manager.TargetEvent(value.code, value.dimension, value.ar);
            fristText.text = value.name;
            secondText.text = value.content;

        });

        pe.image.sprite = null;
        using(UnityWebRequest request = UnityWebRequestTexture.GetTexture(value.thumbnail))
        {
            await request.SendWebRequest();
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            pe.image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5F, 0.5F));
            pe.image.type = Image.Type.Simple;
            pe.image.preserveAspect = true;
        }
    }

    private void URLCheckEvent(string url)
    {
        if(string.IsNullOrEmpty(url))
        {
            if(detail.activeSelf == true)
            {
                fristText.rectTransform.sizeDelta = new Vector2(1280, 100);
                secondText.rectTransform.sizeDelta = new Vector2(1280, 180);
                detail.SetActive(false);
            }
        }
        else
        {
            currentUrl = url;
            if (detail.activeSelf == false)
            {
                fristText.rectTransform.sizeDelta = new Vector2(1000, 100);
                secondText.rectTransform.sizeDelta = new Vector2(1000, 180);
                detail.SetActive(true);
            }
        }
    }

    public void OpenUrlEvent()
    {
        Application.OpenURL(currentUrl);
    }
}
