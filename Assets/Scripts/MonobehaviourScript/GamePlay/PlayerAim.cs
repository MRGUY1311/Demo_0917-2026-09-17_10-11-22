using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    private Player _player;
    private Camera aimCamera;
    public Vector3 mouseAim{get;private set;}
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

    }
    void Update()
    {
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = aimCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!plane.Raycast(ray, out float distance))
            return;

        Vector3 point = ray.GetPoint(distance);
        Vector3 aim = point - transform.position;
        aim.y = 0f;
        
        mouseAim = aim==Vector3.zero?transform.forward:aim;

    }


}
