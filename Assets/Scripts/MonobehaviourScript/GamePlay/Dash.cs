using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public enum DashState
{
    None,
    Startup,
    Active,
    Recovery
}
public class Dash : MonoBehaviour
{
    private CharacterController _characterController;
    private Player _player;
    private PlayerMovement _playerMovement;
    private PlayerAim _playerAim;
    private PlayerFacing _playerFacing;
    private PlayerAttack _playerAttack;
    private Animator _animator;
    [SerializeField]private bool isMouseAim = true;
    private bool canDash = true;
    private DashState currentState = DashState.None;
    
    [SerializeField]private float dashDistance;
    
    [SerializeField]private float dashDuration;
    private float dashVelocity;

    void Awake()
    {
        if (!TryInitialize())
        {
            enabled = false;
            return;
        }
        _animator = _player.GetAnimator();
        if(dashDistance <= 0||dashDuration <= 0)
        {
            Debug.LogError("Dash:distance and duration must more than 0.",this);
            enabled = false;
            return;
        }
        dashVelocity = dashDistance/dashDuration;
    }
    public void StartDash()
    {
        if(!canDash)
            return;
        if(currentState != DashState.None)
            return;

        Vector3 dir = isMouseAim?_playerAim.mouseAim:_playerFacing.dir;
        if(dir == Vector3.zero)
        {
            Debug.LogError("Dash:direction is zero.",this);
            return;
        }
        _playerMovement.SetMoveAllowed(false);
        _playerAttack.SetAttackAllowed(false);
        SetDashAllowed(false);
        StartCoroutine(Dashing(dir));
        StartCoroutine(MoveRoutine(dir));
    }
    private IEnumerator Dashing(Vector3 dir)
    {
        _playerFacing.ChangeFacing(dir);
        _animator.SetTrigger("Dash");                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
        currentState = DashState.Startup;
        yield return new WaitForSeconds(dashDuration/3); 
        currentState = DashState.Active;
        yield return new WaitForSeconds(dashDuration/3);
        currentState = DashState.Recovery;
        yield return new WaitForSeconds(dashDuration/3);
        EndDash();
    }
    private void EndDash()
    {
        currentState = DashState.None;
        _playerMovement.SetMoveAllowed(true);
        _playerAttack.SetAttackAllowed(true);
        SetDashAllowed(true);
        
    }
    private IEnumerator MoveRoutine(Vector3 dir)
    {
        dir.y = 0f;
        dir.Normalize();

        float moved = 0f;

        while (moved < dashDistance)
        {
            float step = dashVelocity * Time.deltaTime;
            step = Mathf.Min(step, dashDistance - moved);

            Vector3 before = transform.position;
            _characterController.Move(dir * step);

            float actual = Vector3.Distance(before, transform.position);
            moved += actual;

            if (actual < 1e-4f)
                yield break;

            yield return null;
        }
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        StartDash();
    }
    public void SetDashAllowed(bool state)
    {
        canDash = state;
    }
    private bool TryRequireComponent<T>(out T component)where T : Component
    {
        if(!TryGetComponent<T>(out component))
        {
            Debug.LogError($"Require {typeof(T).Name}.",this);
            return false;
        }
        return true;
    }
    private bool TryInitialize()
    {
        if(!TryRequireComponent<CharacterController>(out _characterController)||
            !TryRequireComponent<Player>(out _player)||
            !TryRequireComponent<PlayerAim>(out _playerAim)||
            !TryRequireComponent<PlayerMovement>(out _playerMovement)||
            !TryRequireComponent<PlayerFacing>(out _playerFacing)||
            !TryRequireComponent<PlayerAttack>(out _playerAttack)
        )
            return false;
        return true;
    }
    
}
