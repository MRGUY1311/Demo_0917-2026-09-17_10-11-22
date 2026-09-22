using System;
using UnityEngine;

public class PlayerFacing : MonoBehaviour
{
    private Player _player;
    private Vector3 dir;
    private Vector3 pendingMoveDir;
    private bool canChangeFacing = true;
    void Awake()
    {

        _player = GetComponent<Player>();
        if (!_player)
        {
            Debug.LogError("Can't get player component",this);
            enabled = false;
            return;
        }
        
    }

    public void ChangeFacing(Vector3 targetDir)
    {
        if(canChangeFacing && targetDir.sqrMagnitude > 1e-4f)
            dir = targetDir;
    }
    public void WirtePendingMoveDir(Vector3 targetDir)
    {
        pendingMoveDir = targetDir;
        Debug.Log($"PlayerFacing:pendingMoveDir is {pendingMoveDir}.",this);
    }
    private void OnStateChange()
    {
        if(pendingMoveDir.sqrMagnitude > 1e-4f)
        {
            dir = pendingMoveDir;
            Debug.Log($"PlayerFacings:sample pending direction.",this);
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
