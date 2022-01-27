using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform animationUI;
    [SerializeField]
    private RectTransform animationIcon;
    private bool animationEnable = true;
    
    [SerializeField]
    private Canvas apiCanvas;
    [HideInInspector]
    public bool apiEnable = false;

    [SerializeField]
    private Canvas pictureCanvas;
    [HideInInspector]
    public bool pictureEnable = false;
    
    private Vector2 startPos;
    private Vector2 endPos;
    

    public void CloseCanvasEvent()
    {
        animationUI.DOSizeDelta(new Vector2(0, 100f), 0.3f, false).OnStart(() => 
        {

            if (apiCanvas.enabled == true)
            {
                apiCanvas.enabled = false;
                apiEnable = true;
            }
            else
            {
                apiCanvas.enabled = false;
            }

            if (pictureCanvas.enabled == true)
            {
                pictureCanvas.enabled = false;
                pictureEnable = true;
            }
            else
            {
                pictureCanvas.enabled = false;
            }

        });

        animationIcon.DORotate(new Vector3(0, 0, 180), 0.3f);
        animationEnable = false;
    }

    public void OpenCanvasEvent()
    {
        animationUI.DOSizeDelta(new Vector2(0, 900f), 0.3f, false).OnComplete(() =>
        {

            if (apiEnable == true)
            {
                apiCanvas.enabled = true;
                apiEnable = false;
            }
            else
            {
                apiCanvas.enabled = false;
            }

            if (pictureEnable == true)
            {
                pictureCanvas.enabled = true;
                pictureEnable = false;
            }
            else
            {
                pictureCanvas.enabled = false;
            }

        });

        animationIcon.DORotate(Vector3.zero, 0.3f);
        animationEnable = true;
    }
    
    public void OnBeginEvent()
    {
        startPos = Input.mousePosition;
    }

    public void OnEndEvent()
    {
        endPos = Input.mousePosition;

        if(startPos.y - endPos.y < 0)
        {
            OpenCanvasEvent();
            animationEnable = true;
        }
        else
        {            
            CloseCanvasEvent();
            animationEnable = false;
        }
    }

    public void OnClickEvent()
    {
        if(animationEnable == false)
        {
            OpenCanvasEvent();
            animationEnable = true;
        }
        else
        {
            CloseCanvasEvent();
            animationEnable = false;
        }
    }
}
