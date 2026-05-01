using UnityEngine;

public class PlayerAdd : MonoBehaviour
{
    void OnEnable()
    {
        PlayerManager.player.Add(transform);
    }
    void OnDisable()
    {
        PlayerManager.player.Remove(transform);
    }
}
