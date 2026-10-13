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
    private Animator _animator;
    private ActionRequest excutionRequest;
    public bool inCombo => currentStep!=2;
    private int currentStep = 0;

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




    public Vector3 GetAttackDir()
    {
        Vector3 attackDir = _playerAim.mouseAim;
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
            !this.TryRequireComponent(out _playerAim) ||
            !this.TryRequireComponent(out _playerActionController))
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


    ActionTiming IPlayerActionBehavior.GetTiming(ActionRequest request)
    {
        return segments[currentStep].timing;
    }

    void IPlayerActionBehavior.Enter(ActionRequest request)
    {
        excutionRequest = request;
        if(excutionRequest.dir == Vector3.zero)
        {
            Debug.LogError("PlayerAttack:direction is zero.",this);
            excutionRequest.dir = _playerFacing.dir;
        }
        _playerFacing.ChangeFacing(excutionRequest.dir);
    }

    void IPlayerActionBehavior.OnPhaseEnter(ActionPhase phase)
    {
        switch (phase)
        {
            case ActionPhase.None:
                break;
            case ActionPhase.Startup:
                if(currentStep != 0)
                {
                    _animator.SetTrigger("ComboNext");
                    break;
                }
                _animator.SetTrigger("AttackLight");
                break;
            case ActionPhase.Active:
                HitBox.ResolveHit(segments[currentStep],transform.position,excutionRequest.dir);
                break;
            case ActionPhase.Recover:
                break;
        }
    }

    void IPlayerActionBehavior.Tick(float deltaTime)
    {
        return;
    }

    void IPlayerActionBehavior.Exit(ActionExitReason reason)
    {
        switch (reason)
        {
            case ActionExitReason.done:
                _animator.SetTrigger("AttackEnd");
                currentStep = 0;
                excutionRequest = ActionRequest.Empty;
                break;
            case ActionExitReason.derive:
                currentStep++;
                break;
        }
    }
    public bool InCombo()
    {
        return inCombo;
    }
}
