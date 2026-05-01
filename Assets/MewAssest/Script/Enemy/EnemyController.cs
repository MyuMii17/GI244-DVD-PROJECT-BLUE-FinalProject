using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
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
    public bool isFriendlyHit;
    public bool isPlayerHit;
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

        target = FindClosest(gameObject.transform);
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

        if (isPlayerHit == true)
        {
            isFriendlyHit = false;
        }

        if (FriendlyManager.friendlys.Contains(null) == true)
        {
            isFriendlyHit = false;
            target = FindClosest(gameObject.transform);
        }

        if (PlayerManager.player.Contains(null) == true)
        {
            isFriendlyHit = false;
            target = FindClosest(gameObject.transform);
        }

        if(isFriendlyHit == true)
        {
            isPlayerHit = false;
        }
    }

    void GoToTarget()
    {
        // target = FindClosest();

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
    public void OnEnemyHit(float damage , Vector2 dir, Transform transform)
    {
        if(onEnemyDamageCoroutine != null)
        {
            StopCoroutine(onEnemyDamageCoroutine);
        }
        onEnemyDamageCoroutine = StartCoroutine(EnemyTakeDamage(damage,dir,transform));
    }

    IEnumerator EnemyTakeDamage(float damage , Vector2 dir, Transform transform)
    {

        currentDamageRecived += damage;
        currentHealth -= damage;
        target = FindClosest(transform);
        
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

    public Transform FindClosest(Transform transform)
    {
        float minDistance = Mathf.Infinity;
        Transform closest = null;
        var target = transform;
        
        if(isFriendlyHit == false && isPlayerHit == false)
        {
            foreach (var objects1 in objectManafer.objects) // หาสิ่งก่อสร้าง
            {
                if(objects1 == null) continue;

                float distance = Vector2.Distance(gameObject.transform.position, objects1.transform.position);

                if(distance < minDistance)
                {
                    minDistance = distance;
                    closest = objects1.transform;
                }
            }
        }
        else if(isFriendlyHit == true && isPlayerHit == false)
        {
            closest = target;
        }
        else if(isFriendlyHit == false && isPlayerHit == true)
        {
            closest = target;
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
            
            playerController.OnPlayerHit(damage, dir);
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Friendly"))    
        {
            isClash = false;;
        }
        if (collision.gameObject.CompareTag("Player"))    
        {
            isClash = false;;
        }
    }
}
