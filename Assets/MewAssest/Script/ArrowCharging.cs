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
    private Coroutine OnChargingCoroutine;
    private PlayerController playerController;
    void Start()
    {
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
            if(OnChargingCoroutine == null)
            {
                OnChargingCoroutine = StartCoroutine(OnCharge());
            }
        }
        else
        {
            isCanCharging = false;
        }
    }

    IEnumerator OnCharge()
    {
        isCharging = true;
        canShoot = false;

        while (isCanCharging == true)
        {
            bc.enabled = false;
            transform.localScale += new Vector3(arrowScale * Time.deltaTime, arrowScale * Time.deltaTime, 0f);
            transform.position = PlayerController.GetStatic().shootPos.position;

            acceleration += 5f * Time.deltaTime;
            arrowForce = rb.mass * acceleration;

            if(arrowForce >= maxArrowForce)
            {
                arrowForce = maxArrowForce;
            }
        }

        if(isCanCharging == false)
        {
            bc.enabled = true;
            isCharging = false;
            canShoot = true;
        }

        yield return new WaitForSeconds(1);
        
        playerController.isChargeSpawn = false;
        OnChargingCoroutine = null;
        Destroy(gameObject,0.5f);
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
