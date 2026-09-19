using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController _controller;
    private Player _player;
    private PlayerFacing _playerFacing;

    private Camera referenceCamera;
    private Vector2 dir;
    private bool canMove = true;

    [SerializeField]private float maxVelocity;
    void Awake()
    {   
        
        _player = RequireComponent<Player>();
        referenceCamera = _player.GetCamera();
        _controller = _player.GetCharacterController();
        _playerFacing = RequireComponent<PlayerFacing>();
        
    }
    private void init<T>(T component)where T:MonoBehaviour
    {
        component = GetComponent<T>();
        if (component == null)
        {
            Debug.LogError($"Require {typeof(T).Name} .",this);
            enabled = false;
            return;
        }
    }
    private T RequireComponent<T>() where T : MonoBehaviour
{
    T component = GetComponent<T>();

    if (component == null)
    {
        Debug.LogError($"Require {typeof(T).Name}.", this);
        enabled = false;
    }

    return component;
}
    void Update()
    {
        Move();
    }
    private void Move()
    {
        if(!canMove)
            return;
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
    public void SetMoveAllowed(bool state)
    {
        canMove = state;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        dir = context.ReadValue<Vector2>();

        Vector3 moveDir = new Vector3(dir.x,0f,dir.y);
        _playerFacing.ChangeFacing(moveDir);
    }

}
