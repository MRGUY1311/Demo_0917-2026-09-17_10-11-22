using System;
using UnityEngine;
[Serializable]
public enum EnemyState
{
    None,
    Idle,
    Patrol,
    Chase,
    Attack,
    
}
[Serializable]
public struct EnemyStateData
{
    public float serachDuration;
    public float patrolDuration;
    public float patrolVelocitiy;
    [SerializeField, Range(0,1f)]
    public float patrolProbability;
    [SerializeField, Range(0,1f)]
    public float patrolProcesion;
    public float chaseDuration;
    public float chaseVelocity;
    public float chaseDistance;
    public float attackRange;

    // C# 9 的 struct 不能自定义无参构造，通过该入口获取预设值。
    public static readonly EnemyStateData Default = new EnemyStateData(
        0.5f, 3f, 1f, 0.8f, 0.8f, 3f, 2f,4f,1f);

    public EnemyStateData(float _serachDuration,
    float _patrolDuration,float _patroVelocity,float _patrolProbability,float _patrolProcesion,
    float _chaseDuration,float _chaseVelocity,float _chaseDistance,
    float _attackRange)
    {
        serachDuration = _serachDuration;

        patrolDuration = _patrolDuration;
        patrolVelocitiy = _patroVelocity;
        patrolProbability = _patrolProbability;
        patrolProcesion = _patrolProcesion;

        chaseDuration = _chaseDuration;
        chaseVelocity = _chaseVelocity;
        chaseDistance = _chaseDistance;

        attackRange = _attackRange;
    }
}
