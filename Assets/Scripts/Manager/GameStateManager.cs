using System;
using UnityEditor;
using UnityEngine;
public enum GameState
{
    None,
    InGame,
    Pause
}
public class GameStateManager:MonoBehaviour
{
    public GameState currentState{get;private set;} = GameState.None;
    public event Action GameStarted;
    public event Action Paused;
    public event Action Resumed;
    void Awake()
    {
        StartGame();
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
        //specific resolution
        StartGame();
    }
}
