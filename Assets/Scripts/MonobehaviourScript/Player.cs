using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]private Camera referenceCamera;
    void Awake()
    {
        if (!referenceCamera)
        {
            Debug.LogError("Require referenceCamera",this);
            enabled = false;
            return;
        }
    }
    public Camera GetCamera()
    {
        return referenceCamera;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
