using System;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController _controller;
    private Player _player;
    private Camera referenceCamera;
    private Vector2 dir;
    [SerializeField]private float maxVelocity;
    void Awake()
    {   
        _player = GetComponent<Player>();
        if (!_player)
        {
            Debug.LogError("Can't get player component",this);
            enabled = false;
            return;
        }
        
        referenceCamera = _player.GetCamera();
        if(!referenceCamera)
        {
            Debug.LogError("Require reference camera",this);
            enabled = false;
            return;
        }

        _controller = GetComponent<CharacterController>();
        if(!_controller)
        {
            Debug.LogError("Can't get CharacterController.",this);
            enabled = false;
            return;
        }
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraForward = referenceCamera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();
        Vector3 cameraRight = referenceCamera.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        Vector3 move = cameraRight*dir.x+cameraForward*dir.y;
        if(move.sqrMagnitude > 1)
            move.Normalize();
        _controller.Move(move*maxVelocity*Time.deltaTime);
        if(Time.frameCount%30 == 0)
            Debug.Log($"velocitiy is {_controller.velocity}");
    
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //dir = value.Get<Vector2>();
        dir = context.ReadValue<Vector2>();
    }

}
