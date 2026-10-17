using UnityEngine;

public enum ActionPhase
{
    None,
    Startup,
    Active,
    Recover
}

public enum DecisionKind
{
    Begin,
    Pend,
    Reject,
}

public struct ActionRequest<TAction> where TAction:struct,System.Enum
{
    public TAction action;
    public Vector3 dir;

    public ActionRequest(TAction _action,Vector3 _dir)
    {
        action = _action;
        dir = _dir;
    }

    public static readonly ActionRequest<TAction> Empty = new ActionRequest<TAction>(default(TAction),Vector3.zero);
}

public struct ActionDecision<TAction> where TAction:struct,System.Enum
{
    public DecisionKind kind;
    public ActionRequest<TAction> request;

    public ActionDecision(DecisionKind _kind,ActionRequest<TAction> _request)
    {
        kind = _kind;
        request = _request;
    }

    public static readonly ActionDecision<TAction> rejection = new ActionDecision<TAction>(DecisionKind.Reject,ActionRequest<TAction>.Empty);
}

public enum ActionExitReason
{
    done,
    derive,
}
