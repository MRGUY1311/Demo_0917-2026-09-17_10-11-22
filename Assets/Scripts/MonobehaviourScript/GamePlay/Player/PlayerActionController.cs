using System;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;
public enum PlayerAction
{
    None,
    LightAttack,
    Dash,
    DashAttack
}

public class PlayerActionController : MonoBehaviour
{
    
    
    
    public ActionPhase currentPhase {get;private set;}= ActionPhase.None;
    public PlayerAction currentAction {get;private set;}= PlayerAction.None;
    private IActionBehavior<PlayerAction> currentBehavior;
    private ActionRequest<PlayerAction> currentRequest;
    private ActionTiming currentTiming;
    private float elapsed;

    private ActionRequest<PlayerAction> pendingRequest;
    private Dash _dash;
    private PlayerAttack _playerAttack;

    private void Awake()
    {
        if (!TryInitialize())
        {
            enabled = false;
            return;
        }
    }

    private bool TryInitialize()
    {
        if (!this.TryRequireComponent(out _dash) ||
            !this.TryRequireComponent(out _playerAttack))
        {
            return false;
        }

        return true;
    }


    private bool TryBegin(ActionRequest<PlayerAction> request)
    {
        if(!CanBegin(request))
            return false;

        currentAction = request.action;

        if(request.action == PlayerAction.Dash||
        request.action == PlayerAction.DashAttack)
            currentBehavior = _dash;
        if(request.action == PlayerAction.LightAttack)
            currentBehavior = _playerAttack;
        currentPhase = ActionPhase.Startup;
        currentRequest = request;
        currentTiming = currentBehavior.GetTiming(request);
        elapsed = 0;

        currentBehavior.Enter(request);
        currentBehavior.OnPhaseEnter(ActionPhase.Startup);
        return true;
    }
    private bool SetPhase(PlayerAction owner,ActionPhase phase)
    {
        if(owner!=currentAction)
            return false;
        currentPhase = phase;
        return true;
    }
    private bool TryEnd(PlayerAction expectedAction)
    {
        if(!CanEnd(expectedAction))
            return false;
        var nextRequest = pendingRequest;
        ActionExitReason reason;
        if(nextRequest.action != PlayerAction.None)
            reason = ActionExitReason.derive;
        else
            reason = ActionExitReason.done;
        currentBehavior.Exit(reason);
        ClearCurrentExecution();
        ClearPendingRequest();

        if(nextRequest.action != PlayerAction.None)
            TryBegin(nextRequest);
        return true;
        
    }
    private void ClearCurrentExecution()
    {
        currentAction = PlayerAction.None;
        currentBehavior = null;
        currentRequest = ActionRequest<PlayerAction>.Empty;

        currentPhase = ActionPhase.None;
        currentTiming = ActionTiming.Empty;
        elapsed = 0;
    }

    private void ClearPendingRequest()
    {
        pendingRequest = ActionRequest<PlayerAction>.Empty;
    }
    
    public bool GetMoveAccess()
    {
        return currentPhase == ActionPhase.None;
    }

    private bool TryPend(ActionRequest<PlayerAction> request)
    {
        if(pendingRequest.action != PlayerAction.None)
            return false;
        if(!CanPend(request))
            return false;
        pendingRequest = request;
        return true;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(!isActiveAndEnabled || !context.performed)
            return;
        ActionDecision<PlayerAction> decision = AnalyzeAttack();
        ApplyDecision(decision);
        //light attack combo link shoule be interupted here
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        Debug.Log("PlayerActionController:dash.",this);
        if(!isActiveAndEnabled || !context.performed)
            return;
        if(currentAction != PlayerAction.None)
            return;
        ActionRequest<PlayerAction> request = new ActionRequest<PlayerAction>(PlayerAction.Dash,_dash.GetDashDir());
        TryBegin(request);
    }
    private ActionDecision<PlayerAction> AnalyzeAttack(PlayerAction action,ActionPhase phase)
    {
        if(action == PlayerAction.None)
        {
            return new ActionDecision<PlayerAction>(DecisionKind.Begin,
            new ActionRequest<PlayerAction>(PlayerAction.LightAttack,_playerAttack.GetAttackDir()));
        }

        if(action == PlayerAction.Dash)
        {
            if(phase != ActionPhase.None)
            {
                return new ActionDecision<PlayerAction>(DecisionKind.Pend,
                new ActionRequest<PlayerAction>(PlayerAction.DashAttack,_dash.GetDashDir())); 
            }
        }

        if(action == PlayerAction.LightAttack)
        {
            if(phase == ActionPhase.Active ||
            phase == ActionPhase.Recover)
            {
                if(_playerAttack.InCombo())
                {
                    return new ActionDecision<PlayerAction>(DecisionKind.Pend,
                    new ActionRequest<PlayerAction>(PlayerAction.LightAttack,_playerAttack.GetAttackDir()));
                }
            }
        }
        return ActionDecision<PlayerAction>.rejection;
    }
    private ActionDecision<PlayerAction> AnalyzeAttack()
    {
        return AnalyzeAttack(currentAction,currentPhase);
    }
    private void ApplyDecision(ActionDecision<PlayerAction> decision)
    {
        if(decision.kind == DecisionKind.Begin)
            TryBegin(decision.request);
        else if(decision.kind == DecisionKind.Pend)
            TryPend(decision.request);
        else
            return;
    }
    private bool CanPend(ActionRequest<PlayerAction> request)
    {
        if(currentAction == PlayerAction.Dash   &&
        currentPhase != ActionPhase.None)
            return request.action == PlayerAction.DashAttack;
        if(currentAction == PlayerAction.LightAttack &&
        (currentPhase == ActionPhase.Active ||
        currentPhase == ActionPhase.Recover))
            return request.action == PlayerAction.LightAttack;
        return false;
    }
    private bool CanBegin(ActionRequest<PlayerAction> request)
    {
        if(request.action == PlayerAction.None)
            return false;
        if(currentAction != PlayerAction.None)
            return false;
        return true;
    }
    private bool CanEnd(PlayerAction action)
    {
        return action == currentAction&&
        currentAction != PlayerAction.None&&
        currentPhase == ActionPhase.Recover&&
        elapsed >= currentTiming.TotalDuration;
        
    }
    private void Update()
    {
        if(currentAction == PlayerAction.None)
            return;
        float remainingtime = currentTiming.TotalDuration - elapsed;
        float deltaTime = System.MathF.Min(Time.deltaTime,remainingtime);
        currentBehavior.Tick(deltaTime);
        elapsed = System.MathF.Min(
            elapsed + deltaTime,
            currentTiming.TotalDuration
        );

        float startupEnd = currentTiming.startupDuration;
        float activeEnd = currentTiming.activeDuration+currentTiming.startupDuration;
        float actionEnd = currentTiming.TotalDuration;

        if(currentPhase == ActionPhase.Startup
        &&elapsed >= startupEnd)
            EnterPhase(ActionPhase.Active);
        if(currentPhase == ActionPhase.Active
        &&elapsed >= activeEnd)
            EnterPhase(ActionPhase.Recover);
        if(currentPhase == ActionPhase.Recover
        &&elapsed >= actionEnd)
            TryEnd(currentAction);
        
    }
    private void EnterPhase(ActionPhase nextPhase)
    {
        if(currentPhase == nextPhase)
            return;
        currentPhase = nextPhase;
        currentBehavior.OnPhaseEnter(nextPhase);
    }

}
