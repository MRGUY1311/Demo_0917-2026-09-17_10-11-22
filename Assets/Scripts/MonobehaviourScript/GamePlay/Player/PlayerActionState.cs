using UnityEngine;
using UnityEngine.InputSystem;
public enum PlayerAction
{
    None,
    LightAttack,
    Dash,
    DashAttack
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
    public ActionPhase currentPhase {get;private set;}= ActionPhase.None;
    public PlayerAction currentAction {get;private set;}= PlayerAction.None;
    private PlayerAction pendAction = PlayerAction.None;
    public bool TryBegin(PlayerAction action)
    {
        if(currentAction != PlayerAction.None)
        {
            Debug.LogWarning($"PlayerActionState:current action is {currentAction.ToString()}.",this);
            return false;
        }
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
        Debug.Log($"PlayerActionState:sucessfully end action,last action is {currentAction.ToString()}.",this);

        currentAction = PlayerAction.None;
        currentPhase = ActionPhase.None;

        ExcuteCache();

        return true;
    }

    
    public bool GetMoveAccess()
    {
        return currentPhase == ActionPhase.None;
    }
    public bool TryPend(PlayerAction action)
    {
        bool canPendAction =true;
        if(!canPendAction)
        {
            return false;
        }
        pendAction = action;
        return true;
    }
    private bool ExcuteCache()
    {
        return TryBegin(pendAction);
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if(!context.performed)
        {
            switch (currentAction)
            {
                case PlayerAction.None:
                    TryBegin(PlayerAction.LightAttack);
                    break;
                case PlayerAction.LightAttack:
                    TryPend(PlayerAction.LightAttack);
                    break;
                case PlayerAction.Dash:
                    TryPend(PlayerAction.DashAttack);
                    break;
                case PlayerAction.DashAttack:
                    break;
            }
        }
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if(!context.performed)
            TryBegin(PlayerAction.Dash);
    }

}
