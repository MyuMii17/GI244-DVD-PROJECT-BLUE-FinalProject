using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float mass = 1f;
    public float acceleration = 5f;
    public float linearDamp = 0f;
    public float arrowDamage = 10f;
    public float pushForce;
    private float arrowForce;
    private Rigidbody2D rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.linearDamping = linearDamp;
        arrowForce = rb.mass * acceleration;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            EnemyController enemyController = collision.GetComponent<EnemyController>();
            if(enemyController != null)
            {
                var dir = transform.position - enemyController.transform.position;
                dir.Normalize();
                
                enemyController.isHasHit = true;
                enemyController.isPlayerHit = true;
                enemyController.OnEnemyHit(arrowDamage, dir, PlayerManager.players[0], pushForce);
            }
            Destroy(gameObject);
        }

        if (collision.CompareTag("Construction"))
        {
            Destroy(gameObject);
        }
    }
    void FixedUpdate()
    {
        rb.AddForce(transform.right * arrowForce);
    }
}
