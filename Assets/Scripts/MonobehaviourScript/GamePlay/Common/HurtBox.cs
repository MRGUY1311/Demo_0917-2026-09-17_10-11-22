using UnityEngine;
[RequireComponent(typeof(Collider))]
public class HurtBox : MonoBehaviour
{
    [SerializeField]private Health _health;


    void Awake()
    {
        if(!this.TryRequireComponent<Health>(out _health))
        {
            enabled = false;
            return;
        }
    }

    public void ReceiveHit(float damage)
    {
        _health.TakeDamage(damage);
    }
    public void ReceiveHit(HitData hitdata)
    {
        ReceiveHit(hitdata.damage);
    }
}
