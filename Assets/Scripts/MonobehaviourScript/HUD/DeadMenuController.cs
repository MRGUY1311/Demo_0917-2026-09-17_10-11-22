using System;
using UnityEngine;
using UnityEngine.UIElements;

public class DeadMenuController : MonoBehaviour
{
    private PanelRenderer panelRenderer;
    private VisualElement deadOverlay;
    private bool menuVisible;
    private Button restartButton;
    private Button quitButton;

    public event Action RestartRequested;
    public event Action QuitRequested;

    private void Awake()
    {
        if (!this.TryRequireComponent(out panelRenderer))
        {
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnDisable()
    {
        if (panelRenderer != null)
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);

        UnsubscribeButtons();
    }

    public void Show()
    {
        SetVisible(true);
    }

    public void Hide()
    {
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        menuVisible = visible;

        if (deadOverlay != null)
            deadOverlay.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void OnUIReload(
        PanelRenderer renderer,
        VisualElement root,
        int version)
    {
        UnsubscribeButtons();

        deadOverlay = root.Q<VisualElement>("DeadOverlay");
        restartButton = root.Q<Button>("RestartButton");
        quitButton = root.Q<Button>("QuitButton");

        if (deadOverlay == null || restartButton == null || quitButton == null)
        {
            Debug.LogError("DeadMenuController requires DeadOverlay, RestartButton and QuitButton.", this);
            return;
        }

        SetVisible(menuVisible);
        restartButton.clicked += OnRestartClicked;
        quitButton.clicked += OnQuitClicked;
    }

    private void UnsubscribeButtons()
    {
        if (restartButton != null)
            restartButton.clicked -= OnRestartClicked;

        if (quitButton != null)
            quitButton.clicked -= OnQuitClicked;
    }

    private void OnRestartClicked()
    {
        RestartRequested?.Invoke();
    }

    private void OnQuitClicked()
    {
        QuitRequested?.Invoke();
    }
}
