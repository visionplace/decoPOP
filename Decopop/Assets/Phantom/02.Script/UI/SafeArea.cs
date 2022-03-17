using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    // 이미지 SafeArea설정
    void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        Rect safeArae = Screen.safeArea;

        Vector2 anchorMin = safeArae.position;
        Vector2 anchorMax = safeArae.position + safeArae.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
