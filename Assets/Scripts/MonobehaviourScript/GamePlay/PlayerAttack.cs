using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public enum AttackPhase
{
    None,
    Startup,
    Active,
    Recovery
}
public class PlayerAttack : MonoBehaviour
{
    private Player _player;
    private PlayerFacing _playerFacing;
    private PlayerAim _playerAim;
    private PlayerMovement _playerMovement;
    private Animator _animator;
    private Vector3 attackDir;
    private AttackPhase currentPhase = AttackPhase.None;

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
        if (!TryRequireComponent(out _player) ||
            !TryRequireComponent(out _playerFacing) ||
            !TryRequireComponent(out _playerAim) ||
            !TryRequireComponent(out _playerMovement))
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
    private bool TryRequireComponent<T>(out T component) where T : Component
    {
        if(TryGetComponent<T>(out component))
            return true;
        Debug.LogError($"Require {typeof(T).Name}.",this);
        return false;
    }
    
    public void OnAttack(InputAction.CallbackContext context)
    {
        // Learning checkpoint: Invoke Unity Events can call this for Started, Performed, and Canceled.
        // We intentionally do not filter `context.performed` yet.
        // Reproduce: hold the attack button until the attack ends, then release it. If currentPhase is None,
        // the Canceled callback can start an unintended second attack. This also becomes a potential
        // "ghost input" when an Input Buffer is added.
        // Future fix: add `if (!context.performed) return;` before calling startAttack().
        startAttack();
    }

    private void startAttack()
    {
        if(currentPhase != AttackPhase.None)
            return;
        attackDir = _playerAim.mouseAim;
        _playerFacing.ChangeFacing(attackDir);
        _playerFacing.Lock();
        _playerMovement.SetMoveAllowed(false);

        StartCoroutine(attacking());
    }
    IEnumerator attacking()
    {
        _animator.SetTrigger("AttackLight");
        currentPhase = AttackPhase.Startup;
        yield return new WaitForSeconds(0.333f);
        currentPhase = AttackPhase.Active;
        yield return new WaitForSeconds(0.133f);
        currentPhase = AttackPhase.Recovery;
        yield return new WaitForSeconds(0.333f);
        endAttack();
    }
    private void endAttack()
    {
        _playerMovement.SetMoveAllowed(true);
        _playerFacing.UnLock();
        currentPhase= AttackPhase.None;
    }

}
