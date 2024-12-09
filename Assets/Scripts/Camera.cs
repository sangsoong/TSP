using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Camera : MonoBehaviour
{

    public Transform target;
    public float smooth = 0.1f;
    public Vector3 adjustCamPos;
    public Vector2 minCamLimit;
    public Vector2 maxCamLimit;


    void Start()
    {
        
    }
    
    void Update()
    {
        if (target == null) return;

      // 카메라의 대상 위치간의 보간
      Vector3 pos = Vector3.Lerp(transform.position, target.position, smooth);

      // 대상과 한계 위치에 따른 카메라 위치
      transform.position = new Vector3(
         Mathf.Clamp(pos.x, minCamLimit.x, maxCamLimit.x) + adjustCamPos.x,
         Mathf.Clamp(pos.y, minCamLimit.y, maxCamLimit.y) + adjustCamPos.y,
         -10f + adjustCamPos.z);

    }
}
