using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerAttack : MonoBehaviour
{
    private Player _player;
    private PlayerFacing _playerFacing;
    private PlayerAim _playerAim;
    private PlayerMovement _playerMovement;
    private PlayerActionState _playerActionState;
    private Animator _animator;
    private Vector3 attackDir;

    [SerializeField]private float hitRadius;
    [SerializeField]private float hitDistance;
    [SerializeField]private float hitHeight;
    [SerializeField]private LayerMask targetMask;

    [SerializeField]private float lightAttackDamage = 1;
    private void Awake()
    {
        if (!TryInitialize())
        {
            enabled = false;
            return;
        }
    }


    public void OnAttack(InputAction.CallbackContext context)
    {
        // Learning checkpoint: Invoke Unity Events can call this for Started, Performed, and Canceled.
        // We intentionally do not filter `context.performed` yet.
        // Reproduce: hold the attack button until the attack ends, then release it. Once PlayerActionState is None,
        // the Canceled callback can start an unintended second attack. This also becomes a potential
        // "ghost input" when an Input Buffer is added.
        // Future fix: add `if (!context.performed) return;` before calling startAttack().
        StartAttack();
    }

    private void StartAttack()
    {
        if(!_playerActionState.TryBegin(PlayerAction.LightAttack))
            return;
        attackDir = _playerAim.mouseAim;
        _playerFacing.ChangeFacing(attackDir);
        _playerFacing.Lock();
        _playerMovement.SetMoveAllowed(false);

        StartCoroutine(Attacking());
    }
    private IEnumerator Attacking()
    {
        _animator.SetTrigger("AttackLight");
        yield return new WaitForSeconds(0.333f);
        _playerActionState.SetPhase(PlayerAction.LightAttack, ActionPhase.Active);
        ResolveLightHit();
        yield return new WaitForSeconds(0.133f);
        _playerActionState.SetPhase(PlayerAction.LightAttack, ActionPhase.Recover);
        yield return new WaitForSeconds(0.333f);
        EndAttack();
    }
    private void EndAttack()
    {
        if(!_playerActionState.TryEnd(PlayerAction.LightAttack))
            return;
        _playerFacing.UnLock();
        _playerMovement.SetMoveAllowed(true);
    }
    private void ResolveLightHit()
    {
        Vector3 dir = attackDir.normalized;
        Vector3 center =  transform.position+dir * hitDistance + Vector3.up * hitHeight;
        Collider[] results = Physics.OverlapSphere(center,hitRadius,targetMask,QueryTriggerInteraction.Collide);
        foreach(Collider collider in results)
        {
            HurtBox hurtBox = collider.GetComponent<HurtBox>();
            if(!hurtBox)
            {
                Debug.LogError("Failed to get hurtbox.",this);
                continue;
            }
            hurtBox.ReceiveHit(lightAttackDamage);
        }


    }
    private bool TryInitialize()
    {
        if (!this.TryRequireComponent(out _player) ||
            !this.TryRequireComponent(out _playerFacing) ||
            !this.TryRequireComponent(out _playerAim) ||
            !this.TryRequireComponent(out _playerMovement)||
            !this.TryRequireComponent(out _playerActionState))
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
}
