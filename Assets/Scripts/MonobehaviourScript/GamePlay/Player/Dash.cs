using System.Collections;
using Unity.AppUI.Core;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityPipeline.Microsoft.CodeAnalysis.CSharp.Syntax;

public class Dash : MonoBehaviour,IActionBehavior<PlayerAction>
{
    private CharacterController _characterController;
    private Player _player;
    private PlayerAim _playerAim;
    private PlayerFacing _playerFacing;

    private Animator _animator;
    [SerializeField]private AttackSegment dashAttack;
    [SerializeField]private bool isMouseAim = true;
    [SerializeField]private DashSettings dashSettings;
    private float dashVelocity;
    private ActionRequest<PlayerAction> excutionRequest;
    private float moved;
    void Awake()
    {
        if (!TryInitialize())
        {
            enabled = false;
            return;
        }
        _animator = _player.GetAnimator();
        float totalDuration = dashSettings.timing.TotalDuration;
        if(dashSettings.distance <= 0||totalDuration <= 0)
        {
            Debug.LogError("Dash:distance and duration must more than 0.",this);
            enabled = false;
            return;
        }
        dashVelocity = dashSettings.distance/totalDuration;
    }
    
    public Vector3 GetDashDir()
    {
        Vector3 dir = isMouseAim?_playerAim.mouseAim:_playerFacing.dir;
        return dir;
    }
    private bool TryInitialize()
    {
        if(!this.TryRequireComponent<CharacterController>(out _characterController)||
            !this.TryRequireComponent<Player>(out _player)||
            !this.TryRequireComponent<PlayerAim>(out _playerAim)||
            !this.TryRequireComponent<PlayerFacing>(out _playerFacing)
        )
            return false;
        return true;
    }

    ActionTiming IActionBehavior<PlayerAction>.GetTiming(ActionRequest<PlayerAction> request)
    {
        switch (request.action)
        {
            case PlayerAction.Dash  :
                return dashSettings.timing;
            case PlayerAction.DashAttack:
                return dashAttack.timing;
            default:
                Debug.LogError("Dash:request is invalid.",this);
                return ActionTiming.Empty;
        }
        
    }

    void IActionBehavior<PlayerAction>.Enter(ActionRequest<PlayerAction> request)
    {
        excutionRequest = request;
        if(excutionRequest.dir == Vector3.zero)
        {
            Debug.LogError("Dash:direction is zero.",this);
            excutionRequest.dir = _playerFacing.dir;
        }
        _playerFacing.ChangeFacing(excutionRequest.dir);

    }

    void IActionBehavior<PlayerAction>.OnPhaseEnter(ActionPhase phase)
    {
        switch (phase)
        {
            case ActionPhase.None:
                return;
            case ActionPhase.Startup:
                if(excutionRequest.action == PlayerAction.Dash)
                {
                    _animator.SetTrigger("Dash");
                    return;
                }
                if(excutionRequest.action == PlayerAction.DashAttack)
                {
                    _animator.SetBool("PendingDashAttack",true);
                    return;
                }
                return;
            case ActionPhase.Active:
                if(excutionRequest.action == PlayerAction.Dash)
                    return;
                HitBox.ResolveHit(dashAttack,transform.position,excutionRequest.dir);
                return;
                
        }
    }

    void IActionBehavior<PlayerAction>.Tick(float deltaTime)
    {
        switch (excutionRequest.action)
        {
            case PlayerAction.Dash:
                {   
                    if(deltaTime <= 0)
                        return;
                    var dir = excutionRequest.dir;
                    if(moved < dashSettings.distance)
                    {
                        float step = dashVelocity * deltaTime;
                        step = Mathf.Min(step,dashSettings.distance - moved);

                        Vector3 before = transform.position;
                        CollisionFlags flags = _characterController.Move(dir * step);

                        float actual = Vector3.Distance(before,transform.position);
                        moved += actual;

                        bool hitObstacle = (flags & CollisionFlags.Sides)!=0;

                        if(hitObstacle || actual < 1e-4f && Time.timeScale!=0)
                        {
                            moved = dashSettings.distance;
                        }
                    }
                }
                return;
            case PlayerAction.DashAttack:
                return;
        }
    }

    void IActionBehavior<PlayerAction>.Exit(ActionExitReason reason)
    {
        switch (reason)
        {
            case ActionExitReason.done:
                excutionRequest = ActionRequest<PlayerAction>.Empty;
                moved = 0;
                _animator.SetBool("PendingDashAttack",false);
                break;
            case ActionExitReason.derive:
                break;
        }
        
    }

}
