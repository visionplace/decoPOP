using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    [SerializeField]
    private GameObject arMode;
    [SerializeField]
    private GameObject pictureMode;
    [SerializeField]
    private GameObject popup;
    [SerializeField]
    private GameObject joystick;
    [SerializeField]
    private GameObject wallPaper;
    private bool modeEnable = true;

    // 모드변경
    public void ModeEvent()
    {
        if(modeEnable == true)
        {
            if(popup.activeSelf == false)
            {
                popup.SetActive(true);
            }

            if(wallPaper.activeSelf == false)
            {
                wallPaper.SetActive(true);
            }

            arMode.SetActive(false);
            pictureMode.SetActive(true);
            modeEnable = false;
        }
        else
        {
            if (popup.activeSelf == true)
            {
                popup.SetActive(false);
            }

            if (wallPaper.activeSelf == true)
            {
                wallPaper.SetActive(false);
            }

            pictureMode.SetActive(false);
            arMode.SetActive(true);
            modeEnable = true;
        }

        if(joystick.activeSelf == true)
        {
            joystick.SetActive(false);
        }

        if(GameObject.FindWithTag("Target") != null)
        {
            DestroyImmediate(GameObject.FindWithTag("Target"));
        }

        if(GameObject.FindWithTag("WallPaper") != null)
        {
            DestroyImmediate(GameObject.FindWithTag("WallPaper"));
        }
    }    
}
