using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private EnemyActionController actionController;

    private float lastRequestTime;
    public static int survivalCount {get;private set;}= 0;
    [SerializeField]private float serachRadius;
    private EnemyLightAttack _enemyLightAttack;
    [SerializeField]private float requestInterval = 3f;
    [SerializeField]private GameObject player;

    private void Awake()
    {
        if (!TryInitialize())
        {
            enabled = false;
            return;
        }
        player = GameObject.Find("Player");
        _enemyLightAttack.TryChangeAttackTarget(player.transform);
        add();
    }
    private static void add()
    {
        survivalCount++;
        Debug.Log($"EnemyAI:survival count is {survivalCount}");
    }
    public static void sub()
    {
        if(survivalCount<=0)
        {
            Debug.LogError("Enemies are dead already.");
            return;
        }
        survivalCount--;
        Debug.Log($"EnemyAI:survival count is {survivalCount}");
    }
    // Update is called once per frame
    void Update()
    {
        if(!canRequest())
            return;
        if(playerInArea())
        {
            actionController.TryAttack();
            lastRequestTime = Time.time;
            return;
        }    
    }
    private bool playerInArea()
    {
        if(!player)
            return false;
        return Vector3.Distance(player.transform.position,transform.position)<=serachRadius;
    }
    private bool canRequest()
    {
        return Time.time - lastRequestTime >= requestInterval;
    }
    private bool TryInitialize()
    {
        if (!this.TryRequireComponent(out actionController) ||
            !this.TryRequireComponent(out _enemyLightAttack))
        {
            return false;
        }
        return true;
    }
}
