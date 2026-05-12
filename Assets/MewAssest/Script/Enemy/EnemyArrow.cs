using UnityEngine;

public class EnemyArrow : MonoBehaviour
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
        if(collision.CompareTag("Friendly") || collision.CompareTag("Player"))
        {
            FriendlyController friendlyController = collision.GetComponent<FriendlyController>();
            if(friendlyController != null)
            {
                var dir = transform.position - friendlyController.transform.position;
                dir.Normalize();
                
                friendlyController.isHasHit = true;
                friendlyController.OnFriendlyHit(arrowDamage, dir, pushForce);
                Destroy(gameObject);
            }

            PlayerController playerController = collision.GetComponent<PlayerController>();
            if(playerController != null)
            {
                var dir = transform.position - playerController.transform.position;
                dir.Normalize();
                
                playerController.isHasHit = true;
                playerController.OnPlayerHit(arrowDamage, dir, pushForce);
                Destroy(gameObject);
            }
        }
    }
    void FixedUpdate()
    {
        rb.AddForce(transform.right * arrowForce);
    }
}
