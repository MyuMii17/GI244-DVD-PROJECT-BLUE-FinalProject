using System;
using System.Collections;
using UnityEngine;

public class ArrowChargingSkill : MonoBehaviour
{
    public float mass = 1f;
    public float acceleration;
    public float linearDamp = 0f;
    public float arrowDamage = 20f;

    public float arrowForce;
    public float arrowScale = 1f;
    public float maxArrowForce = 15f;
    public bool isCharging;
    private Rigidbody2D rb;
    public bool canShoot;
    private BoxCollider2D bc;
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        acceleration = 5f;
        canShoot = false;
        rb.mass = mass;
        rb.linearDamping = linearDamp;
    }
    void Update()
    {
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
                enemyController.OnEnemyHit(arrowDamage, dir);
            }
        }
    }

    void FixedUpdate()
    {
        if (canShoot)
        {
            rb.AddForce(transform.right * arrowForce, ForceMode2D.Impulse);
        }
    }

}
