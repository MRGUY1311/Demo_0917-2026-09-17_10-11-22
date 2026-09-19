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
    private Vector3 attatckDir;
    private AttackPhase currentPhase = AttackPhase.None;

    void Awake()
    {
        _player = RequireComponent<Player>();
        _animator = _player.GetAnimator();
        _playerFacing = RequireComponent<PlayerFacing>();
        _playerAim = RequireComponent<PlayerAim>();
        _playerMovement = RequireComponent<PlayerMovement>();
    }
    private T RequireComponent<T>() where T : MonoBehaviour
    {
    T component = GetComponent<T>();

    if (component == null)
    {
        Debug.LogError($"Require {typeof(T).Name}.", this);
        enabled = false;
    }

    return component;
    }
    
    public void OnAttack(InputAction.CallbackContext context)
    {
        startAttack();
    }

    private void startAttack()
    {
        if(currentPhase != AttackPhase.None)
            return;
        attatckDir = _playerAim.mouseAim;
        _playerFacing.ChangeFacing(attatckDir);
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
