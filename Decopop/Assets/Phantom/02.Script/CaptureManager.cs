using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureManager : MonoBehaviour
{
    private Texture2D texture;

    [SerializeField]
    private Canvas captureCanvas;
    [SerializeField]
    private Canvas uiCanvas;
    [SerializeField]
    private Canvas apiCanvas;
    private bool apiEnable = false;
    [SerializeField]
    private Canvas pictureCanvas;
    private bool pictureEnable = false;

    [SerializeField]
    private GameObject waterMark;
    [SerializeField]
    private GameObject preview;
    [SerializeField]
    private Image previewImage;

    void Awake()
    {
        if(waterMark.activeSelf == true)
        {
            waterMark.SetActive(false);
        }

        if(preview.activeSelf == true)
        {
            preview.SetActive(false);
        }
    }

    public void GalleryEvent()
    {
        var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        var intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.MAIN");
        intent.Call<AndroidJavaObject>("addCategory", "android.intent.category.LAUNCHER");
        intent.Call<AndroidJavaObject>("addCategory", "android.intent.category.APP_GALLERY");
        activity.Call("startActivity", intent);
    }

    public void CaptureEvent()
    {
        StartCoroutine(Action());
    }

    IEnumerator Action()
    {
        captureCanvas.enabled = true;
        uiCanvas.enabled = false;
        
        if(apiCanvas.enabled == true)
        {
            apiCanvas.enabled = false;
            apiEnable = true;
        }

        if(pictureCanvas.enabled == true)
        {
            pictureCanvas.enabled = false;
            pictureEnable = true;
        }

        waterMark.gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();

        texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.name = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        waterMark.gameObject.SetActive(false);

        if(apiEnable == true)
        {
            apiCanvas.enabled = true;
            apiEnable = false;
        }

        if(pictureEnable == true)
        {
            pictureCanvas.enabled = true;
            apiEnable = false;
        }

        NativeGallery.SaveImageToGallery(texture.EncodeToPNG(), "°£ÆÇAR", texture.name);

        PreviewEvent();
    }

    private void PreviewEvent()
    {
        if(texture != null)
        {
            previewImage.color = Color.white;
            previewImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            previewImage.color = Color.gray;
        }
        
        preview.SetActive(true);
    }

    public void ShareEvent()
    {
        new NativeShare().AddFile(texture).SetTitle("[SignAR]").SetSubject("VisionPlace").SetText(texture.name).Share();
    }

    public void BackButton()
    {
        DestroyImmediate(texture);
        preview.SetActive(false);
        captureCanvas.enabled = false;
    }
}
