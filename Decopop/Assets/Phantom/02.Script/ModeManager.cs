using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    [SerializeField]
    private GameObject arMode;
    [SerializeField]
    private GameObject pictureMode;
    private bool modeEnable = true;

    public void ModeEvent()
    {
        if(modeEnable == true)
        {
            arMode.SetActive(false);
            pictureMode.SetActive(true);
            modeEnable = false;
        }
        else
        {            
            pictureMode.SetActive(false);
            arMode.SetActive(true);
            modeEnable = true;
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
