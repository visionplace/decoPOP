using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using UnityEngine.Networking;

public class ARManager : MonoBehaviour
{
    [SerializeField]
    private GameObject loding;
    [SerializeField]
    private GameObject popup;
    [SerializeField]
    private GameObject joyStick;

    private string targetCode = "";
    private string targetDimension;
    private GameObject target;    
    private int targetNumber = 0;
    private bool targetEnable = false;

    private GameObject wallPaper;

    void Awake()
    {
        if(loding.activeSelf == true)
        {
            loding.SetActive(false);
        }
    }

    // 증강움직이기게 하기
    void Update()
    {
        if(targetEnable == true)
        {
            switch(targetNumber)
            {
                case 1:
                    target.transform.eulerAngles += new Vector3(0, -1f, 0);                    
                    break;
                case 2:
                    target.transform.eulerAngles += new Vector3(-1f, 0, 0);
                    break;
                case 3:
                    target.transform.eulerAngles += new Vector3(0, 1f, 0);                    
                    break;
                case 4:
                    target.transform.eulerAngles += new Vector3(1f, 0, 0);
                    break;
            }
        }
    }

    // 증강 이벤트 발생 2d 3d 구분
    public void TargetEvent(string code, string dimension, string address)
    {
        if(targetCode == code)
        {
            if(target != null)
            {
                TargetResetEvent();
            }

            return;
        }

        targetCode = code;

        if(popup.activeSelf == true)
        {
            popup.SetActive(false);
        }

        if(joyStick.activeSelf == false)
        {
            joyStick.SetActive(true);
        }

        if(dimension == "2D")
        {
            TwoDimensionTargetEvent(address).Forget();
        }
        else
        {
            //ThreeDimensionTargetEvent(address).Forget();
        }
    }

    //2D 증강
    private async UniTask TwoDimensionTargetEvent(string url)
    {
        if(loding.activeSelf == false)
        {
            loding.SetActive(true);
        }        

        if(targetDimension == "3D")
        {
            DestroyImmediate(target);
        }

        if(target == null)
        {
            target = new GameObject();
            target.tag = "Target";
            targetDimension = "2D";
            target.AddComponent<SpriteRenderer>();
            target.AddComponent<BoxCollider>();
            target.AddComponent<LeanSelectableByFinger>();
            target.AddComponent<LeanDragTranslate>();
            target.AddComponent<LeanPinchScale>();
        }

        using(UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            try
            {
                await request.SendWebRequest();
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                
                if (texture.width > 1440)
                {
                    int width = 1000;
                    int height = (texture.height * 1000) / texture.width;
                    texture = TextureResizeEvent(texture, width, height);
                }

                target.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                target.GetComponent<BoxCollider>().size = new Vector3(texture.width * 0.01f, texture.height * 0.01f, 0.1f);               
            }
            finally
            {
                if (loding.activeSelf == true)
                {
                    loding.SetActive(false);
                }

                TargetResetEvent();
            }
        }
    }



    //private async UniTask ThreeDimensionTargetEvent(string atlas)
    //{

    //}

    // 증강제품 초기화
    public void TargetResetEvent()
    {
        if(target != null)
        {
            target.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 1) + (Vector3.up * 0.18f);
            target.transform.rotation = Camera.main.transform.rotation;
            target.transform.localScale = new Vector3(0.025f, 0.025f, 1f);
        }
    }

    // 배경화면 증강
    public void WallPaperEvent()
    {
        if (loding.activeSelf == false)
        {
            loding.SetActive(true);
        }

        if (popup.activeSelf == true)
        {
            popup.SetActive(false);
        }

        if (wallPaper == null)
        {
            wallPaper = new GameObject();
            wallPaper.tag = "WallPaper";
            wallPaper.AddComponent<SpriteRenderer>();
            wallPaper.AddComponent<BoxCollider>();
            wallPaper.AddComponent<LeanSelectableByFinger>();
            wallPaper.AddComponent<LeanDragTranslate>();
            wallPaper.AddComponent<LeanPinchScale>();
        }

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {

            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path);
                texture = ImageResettingEvent(texture);

                if (texture.width > 1440)
                {
                    int width = 1280;
                    int height = (texture.height * 1280) / texture.width;
                    texture = TextureResizeEvent(texture, width, height);
                }

                wallPaper.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                wallPaper.GetComponent<BoxCollider>().size = new Vector3(texture.width * 0.01f, texture.height * 0.01f, 0.1f);
                wallPaper.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 10) + (Vector3.up * 0.18f);
                wallPaper.transform.rotation = Camera.main.transform.rotation;
                wallPaper.transform.localScale = new Vector3(0.25f, 0.25f, 1);
            }

        });

        if (loding.activeSelf == true)
        {
            loding.SetActive(false);
        }
    }

    // 읽기 전용 이미지를 읽기/쓰기 전용으로 변경
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

    // 이미지 크기 조절
    private Texture2D TextureResizeEvent(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    
    public void PlayTargetEvent(int number)
    {
        if(target != null)
        {
            targetNumber = number;
            targetEnable = true;
        }
    }

    public void StopTargetEvent()
    {
        if(target != null)
        {
            targetNumber = 0;
            targetEnable = false;
        }        
    }
}
