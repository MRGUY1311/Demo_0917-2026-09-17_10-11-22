using UnityEngine;
public enum PlayerAction
{
    None,
    LightAttack,
    Dash
}
public enum ActionPhase
{
    None,
    Startup,
    Active,
    Recover
}

public class PlayerActionState : MonoBehaviour
{
    private ActionPhase currentPhase = ActionPhase.None;
    private PlayerAction currentAction = PlayerAction.None;
    public bool TryBegin(PlayerAction action)
    {
        if(currentAction != PlayerAction.None)
            return false;
        currentAction = action;
        currentPhase = ActionPhase.Startup;
        return true;
    }
    public bool SetPhase(PlayerAction owner,ActionPhase phase)
    {
        if(owner!=currentAction)
            return false;
        currentPhase = phase;
        return true;
    }
    public bool TryEnd(PlayerAction owner)
    {
        if(owner!=currentAction)
            return false;
        currentAction = PlayerAction.None;
        currentPhase = ActionPhase.None;
        return true;
    }

    public bool GetMoveAccess()
    {
        return currentPhase == ActionPhase.None;
    }
}
