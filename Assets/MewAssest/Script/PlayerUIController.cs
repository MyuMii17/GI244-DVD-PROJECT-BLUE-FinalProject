using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    private GameStateManager gameStateManager;
    private InputAction pauseAction;
    private bool isPause;
    void Start()
    {
        isPause = false;
        gameStateManager = GameStateManager.GetStatic();
        pauseAction = InputSystem.actions.FindAction("Pause");
    }
    void Update()
    {
        if (pauseAction.WasReleasedThisFrame() && !gameStateManager.isSettingOpen)
        {
            isPause = !isPause;
        }

        if(isPause == true)
        {
            gameStateManager.Pause();
            gameStateManager.pausePanel.SetActive(true);
        }
        else
        {
            gameStateManager.Resume();
        }
    }
}
