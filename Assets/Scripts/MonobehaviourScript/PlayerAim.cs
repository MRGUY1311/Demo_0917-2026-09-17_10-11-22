using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    private Player _player;
    private Camera aimCamera;
    void Awake()
    {
        _player = GetComponent<Player>();
        if (!_player)
        {
            Debug.LogError("Can't get player component",this);
            enabled = false;
            return;
        }
        aimCamera = _player.GetCamera();
        if(!aimCamera)
        {
            Debug.LogError("Require aim camera.",this);
            enabled = false;
            return;
        }

    }
    void Update()
    {
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = aimCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!plane.Raycast(ray, out float distance))
            return;

        Vector3 point = ray.GetPoint(distance);
        Vector3 dir = point - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 1e-4f)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}
