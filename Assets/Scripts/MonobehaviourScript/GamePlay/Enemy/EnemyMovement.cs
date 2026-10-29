using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private CharacterController _controller;
    private EnemyFacing _enemyFacing;
    private EnemyActionController _enemyActionController;
    private Animator _animator;

    [SerializeField]private float maxVelocity;
    private Vector2 dir;
    private float moveAmount;
    void Awake()
    {
        if (!TryInitialize())
        {
            enabled = false;
            return;
        }
        dir = Vector2.zero;
    }
    private bool TryInitialize()
    {
        if (!this.TryRequireComponent(out _enemyFacing)||
            !this.TryRequireComponent(out _enemyActionController)||
            !this.TryRequireComponent(out _controller)||
            !this.TryRequireComponent(out _animator, searchChildren: true))
        {
            return false;
        }
        return true;
    }

    void Update()
    {
        if(dir == Vector2.zero)
            return;
        Move();
    }
    public void ChangeDir(Vector2 _dir)
    {
        dir = _dir;
    }
    // 只更新移动意图，实际位移仍由 Update 检查动作权限后执行。
    public void SetMoveIntent(Vector2 moveDir, float speed)
    {
        dir = moveDir;
        maxVelocity = speed;
    }
    // 清除移动意图和移动动画，不改变动作权限或朝向。
    public void Stop()
    {
        dir = Vector2.zero;
        maxVelocity = 0f;
        moveAmount = 0f;
        _animator.SetFloat("MoveSpeed", 0f);
    }
    private void Move()
    {
        moveAmount = dir.magnitude;
        Vector3 worldDir = new Vector3(dir.x, 0f, dir.y);
        if(_enemyActionController.GetMoveAccess())
        {
            
            _enemyFacing.ChangeFacing(worldDir);
            if(dir.sqrMagnitude > 1)
            {    
                worldDir.Normalize();
            }

            _controller.Move(worldDir*maxVelocity*Time.deltaTime);
            _animator.SetFloat("MoveSpeed",moveAmount);
        }
        else
        {
            _enemyFacing.WirtePendingMoveDir(worldDir);
            _animator.SetFloat("MoveSpeed",0f);
        }  
    }
    public void SetSpeed(float speed)
    {
        maxVelocity = speed;
    }
    public void ResetMoveSpeed()
    {
        _animator.SetFloat("MoveSpeed",0f);
    }

}
