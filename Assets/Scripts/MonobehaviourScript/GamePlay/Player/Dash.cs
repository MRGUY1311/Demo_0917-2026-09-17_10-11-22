using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dash : MonoBehaviour
{
    private CharacterController _characterController;
    private Player _player;
    private PlayerAim _playerAim;
    private PlayerFacing _playerFacing;
    private PlayerActionState _playerActionState;
    private Animator _animator;
    [SerializeField]private bool isMouseAim = true;
    [SerializeField]private float dashDistance;
    [SerializeField]private float dashDuration;
    private float dashVelocity;

    void Awake()
    {
        if (!TryInitialize())
        {
            enabled = false;
            return;
        }
        _animator = _player.GetAnimator();
        if(dashDistance <= 0||dashDuration <= 0)
        {
            Debug.LogError("Dash:distance and duration must more than 0.",this);
            enabled = false;
            return;
        }
        dashVelocity = dashDistance/dashDuration;
    }
    public void StartDash()
    {
        Vector3 dir = isMouseAim?_playerAim.mouseAim:_playerFacing.dir;
        if(dir == Vector3.zero)
        {
            Debug.LogError("Dash:direction is zero.",this);
            return;
        }
        if(!_playerActionState.TryBegin(PlayerAction.Dash))
            return;
        

        _playerFacing.ChangeFacing(dir);

        StartCoroutine(Dashing(dir));
        StartCoroutine(MoveRoutine(dir));
    }
    private IEnumerator Dashing(Vector3 dir)
    {
        _animator.SetTrigger("Dash");
        yield return new WaitForSeconds(dashDuration/3);
        _playerActionState.SetPhase(PlayerAction.Dash,ActionPhase.Active);
        yield return new WaitForSeconds(dashDuration/3);
        _playerActionState.SetPhase(PlayerAction.Dash,ActionPhase.Recover);
        yield return new WaitForSeconds(dashDuration/3);
        EndDash();
    }
    private void EndDash()
    {
        _playerActionState.TryEnd(PlayerAction.Dash);
    }
    private IEnumerator MoveRoutine(Vector3 dir)
    {
        dir.y = 0f;
        dir.Normalize();

        float moved = 0f;

        while (moved < dashDistance)
        {
            float step = dashVelocity * Time.deltaTime;
            step = Mathf.Min(step, dashDistance - moved);

            Vector3 before = transform.position;
            _characterController.Move(dir * step);

            float actual = Vector3.Distance(before, transform.position);
            moved += actual;

            if (actual < 1e-4f)
                yield break;

            yield return null;
        }
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        StartDash();
    }

    private bool TryInitialize()
    {
        if(!this.TryRequireComponent<CharacterController>(out _characterController)||
            !this.TryRequireComponent<Player>(out _player)||
            !this.TryRequireComponent<PlayerAim>(out _playerAim)||
            !this.TryRequireComponent<PlayerFacing>(out _playerFacing)||
            !this.TryRequireComponent<PlayerActionState>(out _playerActionState)
        )
            return false;
        return true;
    }
    
}
