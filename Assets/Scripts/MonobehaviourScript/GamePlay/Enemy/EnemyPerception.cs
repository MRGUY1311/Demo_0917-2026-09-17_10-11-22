using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    [SerializeField]
    private float searchInterval = 0.5f;
    private float lastSearchTime = 0f;

    [SerializeField]
    private float serachRadius;

    [SerializeField,Range(0,1f)]
    private float thres;

    // 返回 true 表示本次实际执行了搜索，此时 found 才是有效的观察结果。
    // 返回 false 表示间隔未到、跳过本次请求，不代表未发现目标。
    public bool TrySearch(GameObject target, out bool found)
    {
        found = false;
        if (Time.time - lastSearchTime < searchInterval)
            return false;

        lastSearchTime = Time.time;
        found = TryFindPlayer(target);
        return true;
    }

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
