using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    [SerializeField]
    private float serachRadius;

    [SerializeField,Range(0,1f)]
    private float thres;
    public bool TryFindPlayer(GameObject player)
    {
        return playerInArea(player)&&
        Vector3.Dot(transform.forward,(player.transform.position-transform.position).normalized)>= thres;
    }
    public bool TryFindPlayer(GameObject player,float _serachRadius)
    {
        return playerInArea(player,_serachRadius)&&
        Vector3.Dot(transform.forward,(player.transform.position-transform.position).normalized)>= thres;
    }

    public bool playerInArea(GameObject player)
    {
        return playerInArea(player, serachRadius);
    }

    public bool playerInArea(GameObject player, float _serachRadius)
    {
        if(!player)
            return false;
        return Vector3.Distance(player.transform.position,transform.position)<=_serachRadius;
    }
}
