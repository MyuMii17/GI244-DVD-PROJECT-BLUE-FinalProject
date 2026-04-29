using System;
using System.Collections;
using UnityEngine;

public class ArrowChargingSkill : MonoBehaviour
{
    public float mass = 1f;
    private float acceleration;
    public float linearDamp = 0f;
    public float arrowDamage = 20f;

    private float arrowForce;
    private float arrowDestroyTime = 5f;
    private float arrowScale = 0.001f;
    private float maxArrowForce = 15f;
    private bool isCharging;
    private Rigidbody2D rb;
    private bool canShoot;
    public static Action ChargeSuccess;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        acceleration = 5f;
        canShoot = false;
        rb.mass = mass;
        rb.linearDamping = linearDamp;
        arrowForce = rb.mass * acceleration;
        isCharging = false;
        StartCoroutine(ChargeCoroutine());
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
                enemyController.OnEnemyHit(arrowDamage,dir);
            }
        }
    }
    IEnumerator ChargeCoroutine()
        {
            isCharging = true;
            while (isCharging && arrowForce < maxArrowForce)
            {
                transform.localScale += new Vector3(arrowScale, arrowScale, 0f);
                transform.position = PlayerController.GetStatic().shootPos.position;
                acceleration += 5f * Time.deltaTime;
                arrowForce = rb.mass * acceleration;
                if(arrowForce >= maxArrowForce)
                {
                    arrowForce = maxArrowForce;
                    isCharging = false;
                    ChargeSuccess?.Invoke();
                }
                yield return null;
            }
            PlayerController.GetStatic().enabled = true;
            canShoot = true;
            yield return new WaitForSeconds(arrowDestroyTime);
            Destroy(gameObject);
        }
    void FixedUpdate()
    {
        if (canShoot)
        {
            rb.AddForce(transform.right * arrowForce, ForceMode2D.Impulse);
        }
    }

}
