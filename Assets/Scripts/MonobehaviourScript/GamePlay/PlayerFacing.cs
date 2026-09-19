using UnityEngine;

public class PlayerFacing : MonoBehaviour
{
    private CharacterController controller;
    private Player _player;
    private Vector3 dir;
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
        controller = _player.GetCharacterController();
        
    }

    public void ChangeFacing(Vector3 targetDir)
    {
        if(canChangeFacing)
            dir = targetDir;
    }
    
    public void Lock()
    {
        canChangeFacing = false;
    }
    public void UnLock()
    {
        canChangeFacing = true;
    } 
    void Update()
    {
        if(dir.sqrMagnitude > 1e-4f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}
