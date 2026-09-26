using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController _controller;
    private Player _player;
    private PlayerFacing _playerFacing;
    private PlayerActionState _playerActionState;
    private Camera referenceCamera;
    private Animator _animator;

    [SerializeField]private float maxVelocity;
    private Vector2 dir;
    private float moveAmount;
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
        if (!this.TryRequireComponent(out _player) ||
            !this.TryRequireComponent(out _playerFacing)||
            !this.TryRequireComponent(out _playerActionState))
        {
            return false;
        }

        referenceCamera = _player.GetCamera();
        _controller = _player.GetCharacterController();
        _animator = _player.GetAnimator();
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

        moveAmount = dir.magnitude;

        if(_playerActionState.GetMoveAccess())
        {
            _playerFacing.ChangeFacing(worldMoveDir);
            if(worldMoveDir.sqrMagnitude > 1)
            {    
                worldMoveDir.Normalize();
            }

            _controller.Move(worldMoveDir*maxVelocity*Time.deltaTime);
            
            _animator.SetFloat("MoveSpeed",moveAmount);
        }
        else
        {
            _playerFacing.WirtePendingMoveDir(worldMoveDir);
            _animator.SetFloat("MoveSpeed",0f);
        }

        
        
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        dir = context.ReadValue<Vector2>();
    }

}
