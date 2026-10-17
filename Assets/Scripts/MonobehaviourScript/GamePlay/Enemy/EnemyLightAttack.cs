using UnityEngine;

public class EnemyLightAttack : MonoBehaviour,IActionBehavior<EnemyAction>
{
    [SerializeField]private AttackSegment lightAttack;
    private EnemyFacing _enemyFacing;
    private Transform target;
    private ActionRequest<EnemyAction> excutionRequest;
    private Animator _animator;
    

    private void Awake()
    {
        if (!TryInitialize())
        {
            enabled = false;
            return;
        }
        _animator = GetComponentInChildren<Animator>();
    }
    public bool TryChangeAttackTarget(Transform _target)
    {
        if(!CanChangeTarget())
            return false;
        target = _target;
        return true;
    }
    private bool CanChangeTarget()
    {
        return true;
    }
    public Vector3 GetAttackDir()
    {
        return (target.position - transform.position).normalized;
    }
    void IActionBehavior<EnemyAction>.Enter(ActionRequest<EnemyAction> request)
    {
        excutionRequest = request;
        if(excutionRequest.dir == Vector3.zero)
        {
            Debug.LogError("PlayerAttack:direction is zero.",this);
            excutionRequest.dir = _enemyFacing.dir;
        }
        _enemyFacing.ChangeFacing(excutionRequest.dir);
    }

    void IActionBehavior<EnemyAction>.Exit(ActionExitReason reason)
    {
        switch (reason)
        {
            case ActionExitReason.done:
                _animator.SetTrigger("AttackEnd");
                excutionRequest = ActionRequest<EnemyAction>.Empty;
                break;
            case ActionExitReason.derive:
                
                break;
        }
    }

    ActionTiming IActionBehavior<EnemyAction>.GetTiming(ActionRequest<EnemyAction> request)
    {
        return lightAttack.timing;
    }

    void IActionBehavior<EnemyAction>.OnPhaseEnter(ActionPhase phase)
    {
        switch (phase)
        {
            case ActionPhase.None:
                break;
            case ActionPhase.Startup:
                _animator.SetTrigger("AttackLight");
                break;
            case ActionPhase.Active:
                HitBox.ResolveHit(lightAttack,transform.position,excutionRequest.dir);
                break;
            case ActionPhase.Recover:
                break;
        }
    }


    void IActionBehavior<EnemyAction>.Tick(float deltaTime)
    {
        return;
    }
    private bool TryInitialize()
    {
        if(!this.TryRequireComponent(out _enemyFacing))
            return false;
        return true;
    }

}
