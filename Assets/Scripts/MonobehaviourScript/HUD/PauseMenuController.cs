using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    private PanelRenderer panelRenderer;

    private VisualElement pauseOverlay;
    private bool menuVisible;
    private Button resumeButton;
    private Button restartButton;
    private Button quitButton;

    [SerializeField]private PauseManager _pauseManager;


    private void OnEnable()
    {
        panelRenderer = GetComponent<PanelRenderer>();
        panelRenderer.RegisterUIReloadCallback(OnUIReload);

    }
    void Awake()
    {
        GameObject.Find("Managers").TryGetComponent(out _pauseManager);
    }
    void Start()
    {
        SetVisible(false);
    }

    private void OnDisable()
    {
        if (panelRenderer != null)
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        
        UnsubscribeButtons();
    }

    public void SetVisible(bool visible)
    {
        menuVisible = visible;

        if (pauseOverlay == null)
            return;

        pauseOverlay.style.display =
            visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
    private void OnUIReload(
        PanelRenderer renderer,
        VisualElement root,
        int version)
    {
        // 防止重复订阅
        UnsubscribeButtons();

        // 根据 UI Builder 中的 Name 获取元素
        resumeButton = root.Q<Button>("ResumeButton");
        restartButton = root.Q<Button>("RestartButton");
        quitButton = root.Q<Button>("QuitButton");
        pauseOverlay = root.Q<VisualElement>("PauseOverlay");
        SetVisible(menuVisible);
        // 订阅点击事件
        if (resumeButton != null)
            resumeButton.clicked += OnResumeClicked;

        if (restartButton != null)
            restartButton.clicked += OnRestartClicked;

        if (quitButton != null)
            quitButton.clicked += OnQuitClicked;
    }

    private void UnsubscribeButtons()
    {
        if (resumeButton != null)
            resumeButton.clicked -= OnResumeClicked;

        if (restartButton != null)
            restartButton.clicked -= OnRestartClicked;

        if (quitButton != null)
            quitButton.clicked -= OnQuitClicked;
    }

    private void OnResumeClicked()
    {
        _pauseManager.Resume();
        Debug.Log("Resume clicked");
    }

    private void OnRestartClicked()
    {
        _pauseManager.Restart();
        Debug.Log("Restart clicked");
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit clicked");
    }
}