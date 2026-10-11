
public interface IPlayerActionBehavior
{
    ActionTiming GetTiming(ActionRequest request);

    void Enter(ActionRequest request);
    void OnPhaseEnter(ActionPhase phase);
    void Tick(float deltaTime);
    void Exit(ActionExitReason reason);
}
