using UnityEngine;

public class EnemyFacing : MonoBehaviour
{
    public Vector3 dir {get;private set;}
    private Vector3 pendingMoveDir;
    private bool canChangeFacing = true;

    public void ChangeFacing(Vector3 targetDir)
    {
        if(canChangeFacing && targetDir.sqrMagnitude > 1e-4f)
            dir = targetDir;
    }
    public void WirtePendingMoveDir(Vector3 targetDir)
    {
        pendingMoveDir = targetDir;
    }
    private void OnStateChange()
    {
        if(pendingMoveDir.sqrMagnitude > 1e-4f)
        {
            dir = pendingMoveDir;
        }
        pendingMoveDir = Vector3.zero;
    }
    
    public void Lock()
    {
        canChangeFacing = false;
    }
    public void UnLock()
    {
        canChangeFacing = true;
        OnStateChange();
    } 

    void Update()
    {
        if(dir.sqrMagnitude > 1e-4f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}
