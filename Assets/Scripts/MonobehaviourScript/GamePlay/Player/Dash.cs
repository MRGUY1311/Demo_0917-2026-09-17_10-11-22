using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dash : MonoBehaviour
{
    private CharacterController _characterController;
    private Player _player;
    private PlayerAim _playerAim;
    private PlayerFacing _playerFacing;
    private PlayerActionState _playerActionState;
    private Animator _animator;
    [SerializeField]private AttackSegment dashAttack;
    [SerializeField]private bool isMouseAim = true;
    [SerializeField]private float dashDistance;
    [SerializeField]private float dashDuration;
    private float dashVelocity;
    private bool canDashAttack;

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
        Vector3 dir = isMouseAim?_playerAim.mouseAim:_playerFacing.dir;
        if(dir == Vector3.zero)
        {
            Debug.LogError("Dash:direction is zero.",this);
            return;
        }
        if(!_playerActionState.TryBegin(PlayerAction.Dash))
            return;
        

        _playerFacing.ChangeFacing(dir);

        StartCoroutine(Dashing(dir));
        StartCoroutine(MoveRoutine(dir));
    }
    private IEnumerator Dashing(Vector3 dir)
    {
        _animator.SetTrigger("Dash");
        yield return new WaitForSeconds(dashDuration/3);
        OpenDashAttackWindow();
        _playerActionState.SetPhase(PlayerAction.Dash,ActionPhase.Active);
        yield return new WaitForSeconds(dashDuration/3);
        _playerActionState.SetPhase(PlayerAction.Dash,ActionPhase.Recover);
        yield return new WaitForSeconds(dashDuration/3);
        CloseDashAttackWindow();
        EndDash();
    }
    private void EndDash()
    {
        _playerActionState.TryEnd(PlayerAction.Dash);
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
        if(!context.performed)
            return;
        StartDash();
    }
    public void OnAttack(InputAction.CallbackContext context)
    {

        if(!canDashAttack)
            return;
        if(!context.performed)
            return;
        StartDashAttack();
    }
    private void StartDashAttack()
    {
        Vector3 dir = isMouseAim?_playerAim.mouseAim:_playerFacing.dir;
        if(dir == Vector3.zero)
        {
            Debug.LogError("Dash:direction is zero.",this);
            return;
        }
        CloseDashAttackWindow();
        StopAllCoroutines();
        EndDash();

        if(!_playerActionState.TryBegin(PlayerAction.DashAttack))
            return;

        _animator.SetTrigger("DashAttack");
        StartCoroutine(DashAttacking(dir));
    }
    private IEnumerator DashAttacking(Vector3 dir)
    {
        _playerFacing.ChangeFacing(dir);
        _playerFacing.Lock();
        yield return new WaitForSeconds(dashAttack.startupDuration);

        _playerActionState.SetPhase(PlayerAction.DashAttack,ActionPhase.Active);
        yield return new WaitForSeconds(dashAttack.activeDuration);

        _playerActionState.SetPhase(PlayerAction.DashAttack,ActionPhase.Recover);
        yield return new WaitForSeconds(dashAttack.recoveryDuration);
        EndDashAttack();

    }
    private void OpenDashAttackWindow()
    {
        canDashAttack = true;
    }
    private void CloseDashAttackWindow()
    {
        canDashAttack = false;
    }

    private void EndDashAttack()
    {
        if(!_playerActionState.TryEnd(PlayerAction.DashAttack))
        {
            return;
        }
        _playerFacing.UnLock();
    }
    private bool TryInitialize()
    {
        if(!this.TryRequireComponent<CharacterController>(out _characterController)||
            !this.TryRequireComponent<Player>(out _player)||
            !this.TryRequireComponent<PlayerAim>(out _playerAim)||
            !this.TryRequireComponent<PlayerFacing>(out _playerFacing)||
            !this.TryRequireComponent<PlayerActionState>(out _playerActionState)
        )
            return false;
        return true;
    }
    
}
