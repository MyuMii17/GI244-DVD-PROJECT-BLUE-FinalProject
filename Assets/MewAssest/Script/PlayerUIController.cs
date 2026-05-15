using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{

    private GameStateManager gameStateManager;
    private InputAction pauseAction;
    private bool isPause;
    public GameObject buildingButtons;
    public bool isPauseOpen;
    void Start()
    {
        isPause = false;
        gameStateManager = GameStateManager.GetStatic();
        pauseAction = InputSystem.actions.FindAction("Pause");
        buildingButtons.SetActive(false);
    }
    void Update()
    {
        if (pauseAction.WasReleasedThisFrame() && !gameStateManager.isSettingOpen)
        {
            isPause = !isPause;
        }

        if(Keyboard.current.bKey.wasReleasedThisFrame)
        {
            if (buildingButtons.activeSelf)
            {
                buildingButtons.SetActive(false);
            }
            else
            {
                buildingButtons.SetActive(true);
            }
        }

        if(isPause == true && !gameStateManager.isSettingOpen && gameStateManager.isGameOver != true && gameStateManager.isWin != true)
        {
            if(isPauseOpen) return;
            isPauseOpen = true;
            gameStateManager.Pause();
            gameStateManager.pausePanel.SetActive(true);
        }
        else if(isPause == false && !gameStateManager.isSettingOpen && gameStateManager.isGameOver != true && gameStateManager.isWin != true)
        {
            if(!isPauseOpen) return;
            isPauseOpen = false;
            gameStateManager.Resume();
        }
    }
}
