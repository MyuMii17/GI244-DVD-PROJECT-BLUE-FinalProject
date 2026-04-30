using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform target;
    public float mass = 1f;
    public float acceleration = 2f;
    public float linearDamp = 0f;
    private float moveForce;
    public float maxHealth = 100f;
    public float currentHealth;
    public float currentDamageRecived;
    public float damage = 10;
    public bool isHasHit;
    public bool isClash;
    public bool isMoving;
    private Coroutine onEnemyDamageCoroutine;
    private SpawnManager spawnManager;
    void Start()
    {
        spawnManager = SpawnManager.GetStatic();
        rb = GetComponent<Rigidbody2D>();
        currentDamageRecived = 0f;
        rb.mass = mass;
        rb.linearDamping = linearDamp;
        moveForce = rb.mass * acceleration;
        currentHealth = maxHealth;
    }
    void Update()
    {
        if(isHasHit == false  && isClash == false)
        {
            isMoving = true;
            GoToTarget();
        }
        else
        {
            isMoving = false;
        }
    }

    void GoToTarget()
    {
        target = FindClosest();

        if(target != null && isHasHit == false && isClash == false && isMoving == true)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            rb.linearVelocity = dir * moveForce;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

    }
    public void OnEnemyHit(float damage , Vector2 dir)
    {
        if(onEnemyDamageCoroutine != null)
        {
            StopCoroutine(onEnemyDamageCoroutine);
        }
        onEnemyDamageCoroutine = StartCoroutine(EnemyTakeDamage(damage,dir));
    }

    IEnumerator EnemyTakeDamage(float damage , Vector2 dir)
    {

        currentDamageRecived += damage;
        currentHealth -= damage;
        
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(-dir * moveForce * 2,ForceMode2D.Impulse);

        if(currentDamageRecived >= maxHealth)
        {
            spawnManager.enemiesDead++;
            Destroy(gameObject);
        }

        yield return new WaitForSeconds(0.5f);

        rb.linearVelocity = Vector2.zero;

        isHasHit = false; 
    }

    private Transform FindClosest()
    {
        float minDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (var friendly in FriendlyManager.friendlys)
        {
            if(friendly == null) continue;

            float distance = Vector2.Distance(gameObject.transform.position, friendly.transform.position);

            if(distance < minDistance)
            {
                minDistance = distance;
                closest = friendly.transform;
            }
        }
        return closest;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out FriendlyController friendlyController))    
        {
            isClash = true;
            isHasHit = true; 

            var dir = gameObject.transform.position - friendlyController.transform.position;
            dir.Normalize();

            friendlyController.OnFriendlyHit(damage, dir);
        }

        if (collision.gameObject.TryGetComponent(out PlayerController playerController))    
        {
            isClash = true;
            isHasHit = true; 

            var dir = gameObject.transform.position - playerController.transform.position;
            dir.Normalize();

            playerController.isHasHit = true;
            playerController.OnPlayerHit(damage, dir);
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Friendly"))    
        {
            isClash = false;;
        }
    }
}
