using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour,IPlayerActionBehavior
{
    private enum LightAttackStep
    {
        A,
        B,
        C
    }


    [SerializeField] private AttackSegment[] segments =
    new AttackSegment[Enum.GetValues(typeof(LightAttackStep)).Length];
    private Player _player;
    private PlayerFacing _playerFacing;
    private PlayerAim _playerAim;
    private PlayerActionController _playerActionController;
    private PlayerActionState _playerActionState;
    private Animator _animator;
    private Vector3 attackDir;
    private bool inCombo = false;
    private bool queuedNextAttack = false;
    [SerializeField]private float hitHeight;
    [SerializeField]private LayerMask targetMask;


    private void Awake()
    {
        if (!TryInitialize())
        {
            enabled = false;
            return;
        }
    }


    public void OnAttack(InputAction.CallbackContext context)
    {
        if(!context.performed)
            return;
        // TODO：攻击迁移期间停用旧输入入口，避免 Dash 验收误触旧攻击协程。
    }

    private void StartAttack()
    {
        if(inCombo)
        {
            queuedNextAttack = true;
            return;
        }
        if(!queuedNextAttack)
        {
            if(!_playerActionState.TryBegin(PlayerAction.LightAttack))
                return;
        }
        _animator.SetTrigger("AttackLight");
        StartCoroutine(Attacking(LightAttackStep.A));

    }

    private IEnumerator Attacking(LightAttackStep step)
    {
        attackDir = _playerAim.mouseAim;
        _playerFacing.ChangeFacing(attackDir);
        _playerFacing.Lock();

        AttackSegment segment = segments[(int)step];
        if(step != LightAttackStep.A)
            _playerActionState.SetPhase(PlayerAction.LightAttack,ActionPhase.Startup);
        yield return new WaitForSeconds(segment.timing.startupDuration);

        inCombo = true;
        _playerActionState.SetPhase(PlayerAction.LightAttack, ActionPhase.Active);

        HitBox.ResolveHit(segment.hitdata,transform.position,attackDir);
        yield return new WaitForSeconds(segment.timing.activeDuration);

        _playerActionState.SetPhase(PlayerAction.LightAttack, ActionPhase.Recover);
        yield return new WaitForSeconds(segment.timing.recoveryDuration);
        inCombo = false;
        if(!queuedNextAttack||step==LightAttackStep.C)
        {
            _animator.SetTrigger("AttackEnd");
            queuedNextAttack = false;
            EndAttack();
        }    
        else
        {
            _playerFacing.UnLock();
            _animator.SetTrigger("ComboNext");
            queuedNextAttack = false;
            StartCoroutine(Attacking(step+1));
        }
        
    }
    private void EndAttack()
    {
        if(!_playerActionState.TryEnd(PlayerAction.LightAttack))
        {
            return;
        }
        _playerFacing.UnLock();
    }

    public Vector3 GetAttackDir()
    {
        Vector3 attackDir = Vector3.one;
        return attackDir;
    }
    public int GetLightAttackStep()
    {
        return 0;
    }
    private bool TryInitialize()
    {
        if (!this.TryRequireComponent(out _player) ||
            !this.TryRequireComponent(out _playerFacing) ||
            !this.TryRequireComponent(out _playerAim) )
        {
            return false;
        }

        _animator = _player.GetAnimator();

        if (_animator == null)
        {
            Debug.LogError("Require Animator.", this);
            return false;
        }

        return true;
    }

    // TODO：临时占位，只跑通管理器生命周期；不播放动画、转向或造成命中。
    ActionTiming IPlayerActionBehavior.GetTiming(ActionRequest request)
    {
        // 零时长让管理器在下一次 Update 结束占位动作，恢复移动与 Dash 权限。
        return ActionTiming.Empty;
    }

    void IPlayerActionBehavior.Enter(ActionRequest request)
    {
    }

    void IPlayerActionBehavior.OnPhaseEnter(ActionPhase phase)
    {
    }

    void IPlayerActionBehavior.Tick(float deltaTime)
    {
    }

    void IPlayerActionBehavior.Exit(ActionExitReason reason)
    {
    }
}
