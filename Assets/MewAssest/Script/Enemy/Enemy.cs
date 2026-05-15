using UnityEngine;

public class Enemy : MonoBehaviour
{
    void OnEnable()
    {
        EnemyManager.enemies.Add(transform);
    }
    void OnDisable()
    {
        EnemyManager.enemies.Remove(transform);
    }
}
