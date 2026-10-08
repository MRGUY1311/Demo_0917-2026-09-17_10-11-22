using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityPipeline.Microsoft.CodeAnalysis.CSharp.Syntax;

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
    private Coroutine moveRoutine;
    private Coroutine dashingRoutine;
    private Coroutine startDashAttackRoutine;

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

        dashingRoutine = StartCoroutine(Dashing(dir));
        moveRoutine = StartCoroutine(MoveRoutine(dir));
    }
    private IEnumerator Dashing(Vector3 dir)
    {
        _animator.SetTrigger("Dash");
        yield return new WaitForSeconds(dashDuration/3);


        _playerActionState.SetPhase(PlayerAction.Dash,ActionPhase.Active);
        yield return new WaitForSeconds(dashDuration/3);
        _playerActionState.SetPhase(PlayerAction.Dash,ActionPhase.Recover);
        yield return new WaitForSeconds(dashDuration/3);

        dashingRoutine = null;
        EndDash();
    }
    private void EndDash()
    {
        _playerActionState.TryEnd(PlayerAction.Dash);
        if(_animator.GetBool("PendingDashAttack"))
        {
            StartDashAttack();
        }

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
        moveRoutine = null;
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if(!context.performed)
            return;
        StartDash();
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        bool canDashAttack = (_playerActionState.currentAction == PlayerAction.Dash&&(
            _playerActionState.currentPhase != ActionPhase.None
        )&&!_animator.GetBool("PendingDashAttack"));

        if(!canDashAttack)
            return;

        if(!context.performed)
            return;
        _animator.SetBool("PendingDashAttack",true);
        //StartDashAttack();//muti invoke
    }
    private void StartDashAttack()
    {
        Debug.Log("Dash: start dash",this);
        Vector3 dir = isMouseAim?_playerAim.mouseAim:_playerFacing.dir;
        if(dir == Vector3.zero)
        {
            Debug.LogError("Dash:direction is zero.",this);
            return;
        }
        // StopCoroutine(dashingRoutine);
        // StopCoroutine(moveRoutine);
        // EndDash();
        /*animator and Coroutine are out of sync,
        what really needed is invoke dash attack after dash clip.
        For now, transition of Dash to DashAttack 's has exit time is true,
        it matches we need.*/
        // yield return new WaitUntil(()=> _playerActionState.currentAction==PlayerAction.None&&
        //  _playerActionState.currentPhase == ActionPhase.None);//trigger is gave after dash transitioned to locomotion


        //_animator.SetBool("PendingDashAttack",true); //OnAttack proves start is legal,so we can write PendingDashAttack


        if(!_playerActionState.TryBegin(PlayerAction.DashAttack))
            return;

        //_animator.SetTrigger("DashAttack");
        StartCoroutine(DashAttacking(dir));

    }
    private IEnumerator DashAttacking(Vector3 dir)
    {
        _playerFacing.ChangeFacing(dir);
        _playerFacing.Lock();
        yield return new WaitForSeconds(dashAttack.startupDuration);

        HitBox.ResolveHit(dashAttack,transform.position,dir);
        _playerActionState.SetPhase(PlayerAction.DashAttack,ActionPhase.Active);
        yield return new WaitForSeconds(dashAttack.activeDuration);

        _playerActionState.SetPhase(PlayerAction.DashAttack,ActionPhase.Recover);
        yield return new WaitForSeconds(dashAttack.recoveryDuration);
        startDashAttackRoutine = null;
        EndDashAttack();

    }


    private void EndDashAttack()
    {
        _animator.SetBool("PendingDashAttack",false);
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
