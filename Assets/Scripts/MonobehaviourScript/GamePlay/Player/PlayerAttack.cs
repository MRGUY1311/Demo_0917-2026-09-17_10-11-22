using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private enum LightAttackStep
    {
        A,
        B,
        C
    }
    [System.Serializable]
    private struct LightAttackSegment
    {
        public float startupDuration;
        public float activeDuration;
        public float recoveryDuration;

        public float damage;
        public float hitRadius;
        public float hitDistance;
    }
    [SerializeField] private LightAttackSegment[] segments = 
    new LightAttackSegment[Enum.GetValues(typeof(LightAttackStep)).Length];
    private Player _player;
    private PlayerFacing _playerFacing;
    private PlayerAim _playerAim;
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
        StartAttack();
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

        LightAttackSegment segment = segments[(int)step];

        
        inCombo = true;
        yield return new WaitForSeconds(segment.startupDuration);

        _playerActionState.SetPhase(PlayerAction.LightAttack, ActionPhase.Active);
        ResolveLightHit(segment.damage,segment.hitRadius,segment.hitDistance);
        
        yield return new WaitForSeconds(segment.activeDuration);

        _playerActionState.SetPhase(PlayerAction.LightAttack, ActionPhase.Recover);
        yield return new WaitForSeconds(segment.recoveryDuration);
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

    private void ResolveLightHit(float damage,float hitRadius,float hitDistance)
    {
        Vector3 dir = attackDir.normalized;
        Vector3 center =  transform.position+dir * hitDistance + Vector3.up * hitHeight;
        Collider[] results = Physics.OverlapSphere(center,hitRadius,targetMask,QueryTriggerInteraction.Collide);
        foreach(Collider collider in results)
        {
            HurtBox hurtBox = collider.GetComponent<HurtBox>();
            if(!hurtBox)
            {
                Debug.LogError("Failed to get hurtbox.",this);
                continue;
            }
            hurtBox.ReceiveHit(damage);
        }
    }
    private bool TryInitialize()
    {
        if (!this.TryRequireComponent(out _player) ||
            !this.TryRequireComponent(out _playerFacing) ||
            !this.TryRequireComponent(out _playerAim) ||
            !this.TryRequireComponent(out _playerActionState))
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
}
