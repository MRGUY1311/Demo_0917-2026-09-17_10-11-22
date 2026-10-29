
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EnemyAI : MonoBehaviour
{
    public EnemyState currentState {get;private set;}= EnemyState.None;
    [SerializeField]
    private EnemyStateData stateData = EnemyStateData.Default;
    private EnemyActionController actionController;

    public static int survivalCount {get;private set;}= 0;
    private EnemyLightAttack _enemyLightAttack;
    private EnemyMovement _enemyMovement;
    private EnemyPerception _enemyPerception;
    private float lastRequestTime = 0;
    private float lastSerachTime = 0;
    private float elapsed => Time.time - lastRequestTime;
    private float serachElapsed => Time.time - lastSerachTime;
    [SerializeField]
    private float requestInterval = 3f;
    [SerializeField]
    private GameObject player;
    public float playerDistance => Vector3.Distance(transform.position,player.transform.position);
    public Animator _animator{get;private set;}
    public CharacterController _characterController{get;private set;}
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
    private void Start()
    {
        ChangeState(EnemyState.Idle);
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
        switch (currentState)
        {
            case EnemyState.None:
            case EnemyState.Idle:
                TickIdle();
                return;
            case EnemyState.Attack:
                TickAttack();
                return;
            case EnemyState.Chase:
                TickChase();
                return;
            case EnemyState.Patrol:
                TickPatrol();
                return;
        }

        if(!canRequest())
            return;
        if(_enemyPerception.playerInArea(player))
        {
            actionController.TryAttack();
            return;
        }    
    }
    private void ChangeState(EnemyState next)
    {
        if(currentState == next)
            return;
        Debug.Log($"EnemyAI:last state is {currentState}");
        Debug.Log($"EnemyAI:current state is {next}");
        OnStateExit(currentState);
        currentState = next;
        OnStateEnter(currentState);
        lastRequestTime = Time.time;
    }
    private void OnStateEnter(EnemyState state)
    {
        switch (currentState)
        {
            case EnemyState.None:
            case EnemyState.Idle:
                _enemyMovement.Stop();
                return;
            case EnemyState.Attack:
                return;
            case EnemyState.Chase:
                RefreshChaseMoveIntent();
                lastRequestTime = Time.time;
                return;
            case EnemyState.Patrol:
                RefreshPatrolMoveIntent();
                lastRequestTime = Time.time;
                return;
        }
    }
    private void OnStateExit(EnemyState state)
    {
        switch (currentState)
        {
            case EnemyState.None:
            case EnemyState.Idle:
                
                return;
            case EnemyState.Attack:
                return;
            case EnemyState.Chase:
                _enemyMovement.Stop();
                return;
            case EnemyState.Patrol:
                _enemyMovement.Stop();
                return;
        }
    }
    private void TickIdle()
    {
        if(serachElapsed < stateData.serachDuration) 
            return;
        lastSerachTime = Time.time;
        if(_enemyPerception.TryFindPlayer(player))
        {
            ChangeState(EnemyState.Chase);
            return;
        }
        if (UnityEngine.Random.value < 0.5f)
        {
            ChangeState(EnemyState.Patrol);
            return;
        }
    }
    private void TickChase()
    {
        if(serachElapsed > stateData.serachDuration)
        {
            lastSerachTime = Time.time;
            if (_enemyPerception.TryFindPlayer(player)&&
            playerDistance<=stateData.attackRange)
            {
                ChangeState(EnemyState.Attack);
                return;
            }
        }
        if(playerDistance > stateData.chaseDistance)
        {
            ChangeState(EnemyState.Patrol);
            return;
        }
        if(elapsed >= stateData.chaseDuration)
        {
            RefreshChaseMoveIntent();
            lastRequestTime = Time.time;
        }
        
    }
    private void TickPatrol()
    {
        if(serachElapsed > stateData.serachDuration)
        {
            lastSerachTime = Time.time;
            if (_enemyPerception.TryFindPlayer(player))
            {
                ChangeState(EnemyState.Chase);
                return;
            }
        }
        if(elapsed < stateData.patrolDuration)
            return;
        RefreshPatrolMoveIntent();
        lastRequestTime = Time.time;
    }
    private void RefreshChaseMoveIntent()
    {
        Vector3 offset = player.transform.position - transform.position;
        Vector2 dir = new Vector2(offset.x, offset.z).normalized;
        _enemyMovement.SetMoveIntent(dir, stateData.chaseVelocity);
    }
    private void RefreshPatrolMoveIntent()
    {
        float angle = UnityEngine.Random.Range(-180f*(1f-stateData.patrolProcesion),180f*(1f-stateData.patrolProcesion));
        Debug.Log($"EnemyAI:patrol angle is {angle}");
        float angleRad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);

        Vector3 offset = player.transform.position - transform.position;
        Vector2 dir = new Vector2(offset.x, offset.z).normalized;
        dir = new Vector2(
            dir.x * cos - dir.y * sin,
            dir.x * sin + dir.y * cos
        );
        _enemyMovement.SetMoveIntent(dir, stateData.patrolVelocitiy);
    }
    private void TickAttack()
    {
        
    }
    private bool canRequest()
    {
        return Time.time - lastRequestTime >= requestInterval;
    }
    private bool TryInitialize()
    {
        if (!this.TryRequireComponent(out actionController) ||
            !this.TryRequireComponent(out _enemyLightAttack)||
            !this.TryRequireComponent(out _enemyMovement)||
            !this.TryRequireComponent(out _enemyPerception))
        {
            return false;
        }
        return true;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Initialize()
    {
        survivalCount = 0;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    private static void OnSceneUnloaded(Scene scene)
    {
        survivalCount = 0;
    }
    public static void reset()
    {
        survivalCount=0;
    }
}
