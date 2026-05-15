using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private int gameIndex = 1;
    public void Play()
    {
        SceneManager.LoadScene(gameIndex);
    }
    public void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }
}
