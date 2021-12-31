using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionManager : MonoBehaviour
{
    void Awake()
    {
        Text version = GetComponent<Text>();
        version.text = "Version : " + Application.version;
    }
}
