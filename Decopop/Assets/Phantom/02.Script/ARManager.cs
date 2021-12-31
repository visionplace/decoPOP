using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using UnityEngine.Networking;

public class ARManager : MonoBehaviour
{
    private string targetCode = "";
    private GameObject target;
    private string targetDimension;

    private GameObject wallPaper;

    public void TargetEvent(string code, string dimension, string address)
    {
        if(targetCode == code)
        {
            return;
        }

        targetCode = code;

        if(dimension == "2D")
        {
            TwoDimensionTargetEvent(address).Forget();
        }
        else
        {
            //ThreeDimensionTargetEvent(address).Forget();
        }
    }

    private async UniTask TwoDimensionTargetEvent(string url)
    {
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
            target.AddComponent<BoxCollider2D>();
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
                target.GetComponent<BoxCollider2D>().size = new Vector2(texture.width * 0.01f, texture.height * 0.01f);               
            }
            finally
            {
                TargetResetEvent();
            }
        }
    }



    //private async UniTask ThreeDimensionTargetEvent(string atlas)
    //{

    //}

    private void TargetResetEvent()
    {
        if(target != null)
        {
            target.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 1) + (Vector3.up * 0.18f);
            target.transform.rotation = Camera.main.transform.rotation;
            target.transform.localScale = new Vector3(0.025f, 0.025f, 1f);
        }
    }

    public void WallPaperEvent()
    {
        if(wallPaper == null)
        {
            wallPaper = new GameObject();
            wallPaper.tag = "WallPaper";
            wallPaper.AddComponent<SpriteRenderer>();
            wallPaper.AddComponent<BoxCollider2D>();
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
                wallPaper.GetComponent<BoxCollider2D>().size = new Vector2(texture.width * 0.01f, texture.height * 0.01f);
                wallPaper.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 1) + (Vector3.up * 0.18f);
                wallPaper.transform.rotation = Camera.main.transform.rotation;
                wallPaper.transform.localScale = new Vector3(0.025f, 0.025f, 1);
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
}
