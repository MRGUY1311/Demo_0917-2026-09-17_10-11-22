using Unity.Mathematics;
using UnityEngine;
[System.Serializable]
public struct HitData
{
    public float damage;
    public float radius;
    public float distance;
    public float height;
    public LayerMask targetMask;
}

public class HitBox : MonoBehaviour
{

    public static void ResolveHit(HitData hitdata,Vector3 position,Vector3 dir)
    {
        Vector3 center = position + dir * hitdata.distance +Vector3.up * hitdata.height;

        Collider[] results = Physics.OverlapSphere(center,hitdata.radius,hitdata.targetMask,QueryTriggerInteraction.Collide);
        foreach(Collider collider in results)
        {
            if(!collider.TryRequireComponent<HurtBox>(out HurtBox hurtBox))
                continue;
            hurtBox.ReceiveHit(hitdata);
        }
    }
    public static void ResolveHit(AttackSegment segment,Vector3 position,Vector3 dir)
    {
        ResolveHit(segment.hitdata,position,dir);
    }
}
