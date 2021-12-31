using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PictureManager : MonoBehaviour
{
    private List<PrefabEvent> pictureList = new List<PrefabEvent>();
    [SerializeField]
    private ScrollRect scroll;
    [SerializeField]
    private RectTransform prefab;

    [SerializeField]
    private ARManager manager;

    void Start()
    {
        PictureEvent();
    }

    private void PictureEvent()
    {
        string directoryPath = Application.persistentDataPath + "/" + "MyGallery";
        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        string[] extensions = new[] { ".jpg", ".jpeg", ".png" };
        FileInfo[] image = directoryInfo.GetFiles().Where(e => extensions.Contains(e.Extension.ToLower())).ToArray();
        FileInfo[] imageArray = image.Union(image).OrderByDescending(o => o.CreationTime).ToArray();

        int index = 0;
        int location = 0;
        for(int i = 0; i < imageArray.Length; i++)
        {
            if( i >= 10)
            {
                File.Delete(imageArray[i].FullName);
            }
            else
            {
                RectTransform obj = Instantiate(prefab, scroll.content);
                pictureList.Add(obj.GetComponent<PrefabEvent>());

                PictureSettingEvent("file://" + imageArray[i].FullName, index, location).Forget();
                index++;
                location += 490;
            }
        }

        if(location < 1440f)
        {
            if(scroll.enabled == true)
            {
                scroll.enabled = false;
            }
        }
        else
        {
            if (scroll.enabled == false)
            {
                scroll.enabled = true;
            }
        }
        scroll.content.sizeDelta = new Vector2(location + 80, 300);
    }

    private async UniTask PictureSettingEvent(string url, int index, int location)
    {
        PrefabEvent pe = pictureList[index];
        pe.rectTransform.anchoredPosition = new Vector3(location + 80, 0, 0);
        pe.rectTransform.sizeDelta = new Vector2(450, 300);
        pe.button.onClick.RemoveAllListeners();
        pe.button.onClick.AddListener(() =>
        {

            manager.TargetEvent(url, "2D", url);

        });

        pe.image.sprite = null;
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            await request.SendWebRequest();
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            pe.image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5F, 0.5F));
            pe.image.type = Image.Type.Simple;
            pe.image.preserveAspect = true;
        }
    }

    public void GalleryImageLoadEvent()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {

        if (path != null)
        {
            Texture2D texture = NativeGallery.LoadImageAtPath(path);

            if (texture != null)
            {
                string[] split = path.Split('.');                
                texture = ImageResettingEvent(texture);

                string directoryPath = Application.persistentDataPath + "/" + "MyGallery";
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                    byte[] bytes = texture.EncodeToPNG();
                    string textureName = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + split[split.Length - 1];
                    string filePath = directoryPath + "/" + textureName;
                    File.WriteAllBytes(filePath, bytes);
                    
                    if(pictureList.Count >= 10)
                    {
                        for(int i = 9; i < pictureList.Count; i++)
                        {
                            pictureList.RemoveAt(i);
                        }                        
                    }

                    RectTransform obj = Instantiate(prefab, scroll.content);
                    pictureList.Insert(0, obj.GetComponent<PrefabEvent>());
                    PictureSettingEvent("file://" + filePath, 0, 0).Forget();
                    
                    for(int i = 1; i < pictureList.Count; i++)
                    {
                        pictureList[i].rectTransform.anchoredPosition = new Vector3((490 * i) + 80, 0, 0);
                    }

                    scroll.horizontalNormalizedPosition = 0;
                    scroll.content.sizeDelta = new Vector2((490f * pictureList.Count) + 80, 300);
                }
            }
        });
    }

    private Texture2D ImageResettingEvent(Texture2D texture)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(texture, renderTexture);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;
        Texture2D newTexture = new Texture2D(texture.width, texture.height);
        newTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        newTexture.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        return newTexture;
    }
}
