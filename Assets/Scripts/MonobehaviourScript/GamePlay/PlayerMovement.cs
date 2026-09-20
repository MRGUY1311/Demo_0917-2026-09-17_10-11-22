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
        if (!TryInitialize())
        {
            enabled = false;
            return;
        }
    }
    private bool TryInitialize()
    {
        if (!TryRequireComponent(out _player) ||
            !TryRequireComponent(out _playerFacing))
        {
            return false;
        }

        referenceCamera = _player.GetCamera();
        _controller = _player.GetCharacterController();
        if (referenceCamera == null)
        {
            Debug.LogError("Require reference camera.", this);
            return false;
        }
        if(_controller == null)
        {
            Debug.LogError("Require character controller.",this);
            return false;
        }
        return true;
    }

    private bool TryRequireComponent<T>(out T component) where T : Component
    {
        if(TryGetComponent<T>(out component))
            return true;
        Debug.LogError($"Require {typeof(T).Name}.",this);
        return false;
    }
    void Update()
    {
        Move();
    }
    private void Move()
    {
        Vector3 cameraForward = referenceCamera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();
        Vector3 cameraRight = referenceCamera.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();
        Vector3 worldMoveDir = cameraRight*dir.x+cameraForward*dir.y;

        if(canMove)
            _playerFacing.ChangeFacing(worldMoveDir);
            if(worldMoveDir.sqrMagnitude > 1)
                worldMoveDir.Normalize();
            _controller.Move(worldMoveDir*maxVelocity*Time.deltaTime);
            if(Time.frameCount%30 == 0)
                Debug.Log($"velocitiy is {_controller.velocity}");
        else
            _playerFacing.WirtePendingMoveDir(worldMoveDir);

        
        
    }
    public void SetMoveAllowed(bool state)
    {
        canMove = state;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        dir = context.ReadValue<Vector2>();
    }

}
