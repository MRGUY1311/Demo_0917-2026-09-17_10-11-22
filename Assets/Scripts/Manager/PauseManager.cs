using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    private GameStateManager _gameStateManager;
    [SerializeField]private PlayerInput _playerInput;
    [SerializeField]private PauseMenuController pauseMenu;

    private void Awake()
    {
        if (!this.TryRequireComponent(out _gameStateManager))
        {
            enabled = false;
            return;
        }

        if (!_playerInput)
        {
            Debug.LogError("PauseManager requires PlayerInput.", this);
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        if (!_gameStateManager)
            return;

        _gameStateManager.Paused += OnPause;
        _gameStateManager.Resumed += OnResume;
    }

    private void OnDisable()
    {
        if (!_gameStateManager)
            return;

        _gameStateManager.Paused -= OnPause;
        _gameStateManager.Resumed -= OnResume;
    }

    public void Restart()
    {
        _gameStateManager.Restart();
    }
    public void Resume()
    {
        _gameStateManager.Resume();
    }
    
    public void OnPauseClcked(InputAction.CallbackContext context)
    {
        if(!context.performed)
            return;
        switch (_gameStateManager.currentState)
        {
            case GameState.InGame:
                _gameStateManager.Pause();
                break;
            case GameState.Pause:
                _gameStateManager.Resume();
                break;
        }
    }
    public void OnPause()
    {
        if (!_playerInput)
            return;

        _playerInput.SwitchCurrentActionMap("UI");
        pauseMenu.SetVisible(true);
        Debug.Log("PauseManager:OnPause",this);
    }

    private void OnResume()
    {
        if (!_playerInput)
            return;

        _playerInput.SwitchCurrentActionMap("Player");
        pauseMenu.SetVisible(false);
    }
}
