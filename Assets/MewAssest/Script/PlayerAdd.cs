using UnityEngine;

public class PlayerAdd : MonoBehaviour
{
    void OnEnable()
    {
        PlayerManager.players.Add(transform);
    }
    void OnDisable()
    {
        PlayerManager.players.Remove(transform);
    }
}
