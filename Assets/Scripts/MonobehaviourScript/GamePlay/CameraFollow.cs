using System;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]private Transform desired;
    [SerializeField]private Vector3 offset;
    private float fixedHeight;

    [SerializeField]private float followSharpness;


    
    void Start()
    {

        if(!desired)
        {
            Debug.LogError("CameraFollow requires a target Transform.", this);
            enabled = false;
            return;
        }

        if(offset == Vector3.zero)
        {
            offset = new Vector3(20,0,-10);
        }
        fixedHeight = transform.position.y+offset.y;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    void LateUpdate()
    {
        float t = 1f - Mathf.Exp(-followSharpness * Time.deltaTime);
        Vector3 desiredpos = desired.position;

        Vector3 pos = Vector3.Lerp(transform.position,desiredpos+offset,t);
        pos.y = fixedHeight;

        transform.position = pos;
    }
}
