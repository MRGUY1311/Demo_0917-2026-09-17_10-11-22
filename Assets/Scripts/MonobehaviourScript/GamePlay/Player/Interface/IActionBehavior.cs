
using UnityPipeline.Microsoft.CodeAnalysis.CSharp.Syntax;

public interface IActionBehavior<TAction> where TAction:struct,System.Enum
{
    ActionTiming GetTiming(ActionRequest<TAction> request);

    void Enter(ActionRequest<TAction> request);
    void OnPhaseEnter(ActionPhase phase);
    void Tick(float deltaTime);
    void Exit(ActionExitReason reason);
}
