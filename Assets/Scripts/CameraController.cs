using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Camera : MonoBehaviour
{

    public Transform target;
    public Vector3 adjustCamPos;
    public Vector2 minCamLimit;
    public Vector2 maxCamLimit;


    void Start()
    {
        
    }
    
    void Update()
    {
        if (target == null) return;

        Vector2 pos = target.position;

        transform.position = new Vector3(
            Mathf.Clamp(pos.x, minCamLimit.x, maxCamLimit.x) + adjustCamPos.x,
            Mathf.Clamp(pos.y, minCamLimit.y, maxCamLimit.y) + adjustCamPos.y,
            -5f + adjustCamPos.z);

    }
}
