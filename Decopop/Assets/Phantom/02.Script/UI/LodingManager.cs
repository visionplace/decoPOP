using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LodingManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform circle;
    private float speed = 5;

    // 로딩
    void Update()
    {
        Vector3 angle = circle.localEulerAngles;
        angle.z -= speed;
        circle.localEulerAngles = angle;
    }
}
