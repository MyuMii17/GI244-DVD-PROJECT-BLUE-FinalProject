using UnityEngine;

public class SettingManager : MonoBehaviour
{
    private GameStateManager gameStateManager;
    private AudioSource audioSource;
    void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    void Start()
    {
        gameStateManager = GameStateManager.GetStatic();
    }
}
