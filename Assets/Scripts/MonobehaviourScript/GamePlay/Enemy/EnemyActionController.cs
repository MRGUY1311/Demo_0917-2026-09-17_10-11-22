using UnityEngine;
public enum EnemyAction
{
    None,
    LightAttack
}
public class EnemyActionController : MonoBehaviour
{
    public ActionPhase currentPhase {get;private set;}= ActionPhase.None;
    public EnemyAction currentAction {get;private set;}= EnemyAction.None;
    private IActionBehavior<EnemyAction> currentBehavior;
    private ActionRequest<EnemyAction> currentRequest;
    private ActionTiming currentTiming;
    private float elapsed;

    private ActionRequest<EnemyAction> pendingRequest;
    private EnemyLightAttack _enemyLightAttack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        if(!this.TryRequireComponent(out _enemyLightAttack))
            return false;
        return true;
    }

    public bool TryAttack()
    {
        ActionDecision<EnemyAction> decision = AnalyzeAttack();
        ApplyDecision(decision);
        return decision.kind == DecisionKind.Reject;
    }
    public bool TryAttack(Vector3 dir)
    {
        ActionDecision<EnemyAction> decision = AnalyzeAttack(dir);
        ApplyDecision(decision);
        return decision.kind == DecisionKind.Reject;
    }

    private void Update()
    {
        if(currentAction == EnemyAction.None)
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
    private ActionDecision<EnemyAction> AnalyzeAttack(EnemyAction action,ActionPhase phase,Vector3 attackDir)
    {
        if(action == EnemyAction.None)
        {
            return new ActionDecision<EnemyAction>(DecisionKind.Begin,
            new ActionRequest<EnemyAction>(EnemyAction.LightAttack,attackDir));
        }

        return ActionDecision<EnemyAction>.rejection;
    }
    private ActionDecision<EnemyAction> AnalyzeAttack()
    {
        return AnalyzeAttack(currentAction,currentPhase,_enemyLightAttack.GetAttackDir());
    }
    private ActionDecision<EnemyAction> AnalyzeAttack(Vector3 dir)
    {
        return AnalyzeAttack(currentAction,currentPhase,dir);
    }
    private void ApplyDecision(ActionDecision<EnemyAction> decision)
    {
        if(decision.kind == DecisionKind.Begin)
            TryBegin(decision.request);
        else if(decision.kind == DecisionKind.Pend)
            TryPend(decision.request);
        else
            return;
    }


    private bool TryBegin(ActionRequest<EnemyAction> request)
    {
        if(!CanBegin(request))
            return false;

        currentAction = request.action;

        if(request.action == EnemyAction.LightAttack)
            currentBehavior = _enemyLightAttack;
        currentPhase = ActionPhase.Startup;
        currentRequest = request;
        currentTiming = currentBehavior.GetTiming(request);
        elapsed = 0;

        currentBehavior.Enter(request);
        currentBehavior.OnPhaseEnter(ActionPhase.Startup);
        return true;
    }
    private bool TryEnd(EnemyAction expectedAction)
    {
        if(!CanEnd(expectedAction))
            return false;
        var nextRequest = pendingRequest;
        ActionExitReason reason;
        if(nextRequest.action != EnemyAction.None)
            reason = ActionExitReason.derive;
        else
            reason = ActionExitReason.done;
        currentBehavior.Exit(reason);
        ClearCurrentExecution();
        ClearPendingRequest();

        if(nextRequest.action != EnemyAction.None)
            TryBegin(nextRequest);
        return true;
        
    }
    private void ClearCurrentExecution()
    {
        currentAction = EnemyAction.None;
        currentBehavior = null;
        currentRequest = ActionRequest<EnemyAction>.Empty;

        currentPhase = ActionPhase.None;
        currentTiming = ActionTiming.Empty;
        elapsed = 0;
    }

    private void ClearPendingRequest()
    {
        pendingRequest = ActionRequest<EnemyAction>.Empty;
    }
    
    public bool GetMoveAccess()
    {
        return currentPhase == ActionPhase.None;
    }

    private bool TryPend(ActionRequest<EnemyAction> request)
    {
        if(pendingRequest.action != EnemyAction.None)
            return false;
        if(!CanPend(request))
            return false;
        pendingRequest = request;
        return true;
    }
    private bool CanBegin(ActionRequest<EnemyAction> request)
    {
        if(request.action == EnemyAction.None)
            return false;
        if(currentAction != EnemyAction.None)
            return false;
        return true;
    }
    private bool CanEnd(EnemyAction action)
    {
        return action == currentAction&&
        currentAction != EnemyAction.None&&
        currentPhase == ActionPhase.Recover&&
        elapsed >= currentTiming.TotalDuration;
        
    }
    private bool CanPend(ActionRequest<EnemyAction> request)
    {
        return false;
    }

}
