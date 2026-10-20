using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    private Player _player;
    private Camera aimCamera;
    private InputAction aimPositionAction;
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
        if (!aimCamera)
        {
            enabled = false;
            return;
        }

        if (!this.TryRequireComponent(out PlayerInput playerInput))
        {
            enabled = false;
            return;
        }

        InputActionAsset actions = playerInput.actions;
        if (actions == null)
        {
            Debug.LogError("PlayerAim requires PlayerInput Actions.", this);
            enabled = false;
            return;
        }

        aimPositionAction = actions.FindAction("Player/AimPosition");
        if (aimPositionAction == null)
        {
            Debug.LogError("PlayerAim requires Player/AimPosition.", this);
            enabled = false;
            return;
        }

    }
    void Update()
    {
        // Player Map 的启停由外部管理；禁用期间保留最后的瞄准方向。
        if (!aimPositionAction.enabled)
            return;

        Vector2 screenPosition = aimPositionAction.ReadValue<Vector2>();
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = aimCamera.ScreenPointToRay(screenPosition);

        if (!plane.Raycast(ray, out float distance))
            return;

        Vector3 point = ray.GetPoint(distance);
        Vector3 aim = point - transform.position;
        aim.y = 0f;

        
        mouseAim = aim==Vector3.zero?transform.forward:aim.normalized;

    }


}
