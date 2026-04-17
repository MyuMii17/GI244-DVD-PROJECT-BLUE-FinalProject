using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float mass = 1f;
    public float acceleration = 5f;
    public float linearDamp = 0f;
    private float arrowForce;
    private Rigidbody2D rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.linearDamping = linearDamp;
        arrowForce = rb.mass * acceleration;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(transform.right * arrowForce);
    }
}
