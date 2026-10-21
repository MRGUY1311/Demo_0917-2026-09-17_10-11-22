using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum GameState
{
    None,
    InGame,
    Pause,
    End
}
public class GameStateManager:MonoBehaviour
{
    public GameState currentState{get;private set;} = GameState.None;
    public event Action GameStarted;
    public event Action Paused;
    public event Action Resumed;
    public event Action StageCleared;
    public event Action PlayerDead;

    // 场景接线集中在这里，Health 和菜单不需要反向引用管理器。
    [SerializeField]private Health playerHealth;
    [SerializeField]private Health[] enemyHealths = Array.Empty<Health>();
    [SerializeField]private DeadMenuController deadMenu;

    void Awake()
    {
        StartGame();
    }

    private void OnEnable()
    {
        if (playerHealth)
            playerHealth.Died += HandlePlayerDeath;

        foreach (Health health in enemyHealths)
        {
            if (health)
                health.Died += HandleEnemyDeath;
        }

        if (deadMenu)
        {
            deadMenu.RestartRequested += Restart;
            deadMenu.QuitRequested += HandleQuitRequested;
            PlayerDead += deadMenu.Show;
            GameStarted += deadMenu.Hide;
            StageCleared += deadMenu.Show;
            StageCleared += Pause;
        }
    }

    private void OnDisable()
    {
        if (playerHealth)
            playerHealth.Died -= HandlePlayerDeath;

        foreach (Health health in enemyHealths)
        {
            if (health)
                health.Died -= HandleEnemyDeath;
        }

        if (deadMenu)
        {
            deadMenu.RestartRequested -= Restart;
            deadMenu.QuitRequested -= HandleQuitRequested;
            PlayerDead -= deadMenu.Show;
            GameStarted -= deadMenu.Hide;
            StageCleared -= deadMenu.Show;
            StageCleared -= Pause;
        }
    }

    private void HandlePlayerDeath()
    {
        // TODO: 在这里处理玩家死亡后的状态与 Gameplay 权限。
        PlayerDead?.Invoke();
    }

    private void HandleEnemyDeath()
    {
        EnemyAI.sub();
        if(EnemyAI.survivalCount==0)
            Publish(StageCleared);
    }

    private void HandleQuitRequested()
    {

    }

    public void Publish(Action action)
    {
        action?.Invoke();
    }
    public void Pause()
    {
        if(!CanPause())
            return;
        currentState = GameState.Pause;
        Time.timeScale = 0;
        Paused?.Invoke();
    }
    private bool CanPause()
    {
        return currentState == GameState.InGame;
    }
    public void Resume()
    {
        if(!CanResume())
            return;
        currentState = GameState.InGame;
        Time.timeScale = 1;
        Resumed?.Invoke();
    }
    private bool CanResume()
    {   
        return currentState == GameState.Pause;
    }
    public void StartGame()
    {
        if(!CanStartGame())
            return;
        currentState = GameState.InGame;
        GameStarted?.Invoke();
    }
    private bool CanStartGame()
    {
        return true;
    }
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
