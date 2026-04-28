using UnityEngine;

public class EnemyDetail : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentDamageRecived;
    void Start()
    {
        currentDamageRecived = 0f;
    }
    public void TakeDamage(float damage)
    {
        currentDamageRecived += damage;
        if(currentDamageRecived >= maxHealth)
        {
            Destroy(gameObject);
        }
    }
}
