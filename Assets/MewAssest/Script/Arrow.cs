using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float mass = 1f;
    public float acceleration = 5f;
    public float linearDamp = 0f;
    public float arrowDamage = 10f;
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
            EnemyDetail enemy = collision.GetComponent<EnemyDetail>();
            if(enemy != null)
            {
                enemy.TakeDamage(arrowDamage);
            }
            Destroy(gameObject);
        }
    }
    void FixedUpdate()
    {
        rb.AddForce(transform.right * arrowForce);
    }
}
