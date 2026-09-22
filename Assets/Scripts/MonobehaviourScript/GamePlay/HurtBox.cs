using UnityEngine;
[RequireComponent(typeof(Collider))]
public class HurtBox : MonoBehaviour
{
    [SerializeField]private Health _health;


    void Awake()
    {
        if(!TryRequireComponent<Health>(out _health))
        {
            enabled = false;
            return;
        }
    }
    private bool TryRequireComponent<T>(out T component)where T :Component
    {
        if(TryGetComponent<T>(out component))
            return true;
        Debug.LogError($"Require {typeof(T).Name}.",this);
        return false;
    }
    


    public void ReceiveHit(float damage)
    {
        Debug.Log("hit",this);
        _health.TakeDamage(damage);
    }
}
