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
    private float arrowScale = 1f;
    private float maxArrowForce = 15f;
    private bool isCharging;
    public bool isCanCharging;
    private Rigidbody2D rb;
    public bool canShoot;
    private BoxCollider2D bc;
    private Coroutine OnArrowChargingCoroutine;
    private PlayerController playerController;
    private PoolManager poolManager;
    void Start()
    {
        poolManager = PoolManager.GetStatic();
        playerController = PlayerController.GetStatic();
        bc = GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        acceleration = 5f;
        canShoot = false;
        rb.mass = mass;
        rb.linearDamping = linearDamp;
    }
    void Update()
    {
        if(playerController.isPlayerCharging == true)
        {
            isCanCharging = true;
            if(OnArrowChargingCoroutine == null)
            {

            }
        }
        else
        {
            isCanCharging = false;
        }
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
        StartCoroutine(onChargeSpawn());
        Destroy(gameObject,2f);
    }

    IEnumerator onChargeSpawn()
    {
        acceleration = playerController.currentChargeAccel;
        arrowForce = rb.mass * acceleration;
        arrowDamage = playerController.currentChargeDamage;

        rb.AddForce(transform.right * arrowForce, ForceMode2D.Impulse);

        yield return null;
    }

}
