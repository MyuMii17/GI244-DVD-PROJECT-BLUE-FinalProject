using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D cd;
    private Transform target;
    private LayerMask objectLayer;
    private LayerMask constructionLayer;
    private float rangeDistance;
    public Transform shootPosition;
    public Transform shootRoataion;
    public GameObject arrowPrefeb;
    public float mass = 1f;
    public float acceleration = 2f;
    public float linearDamp = 0f;
    public float circleRange;
    public float pushForce;
    private float moveForce;
    public float maxHealth = 100f;
    public float currentHealth;
    public float currentDamageRecived;
    public float damage = 10;
    public bool isHasHit;
    public bool isClash;
    public bool isMoving;
    public bool isPlayerHit;
    public bool isFind;

    [Header("Range Enemy")]
    public float shootCooldown;
    private float nextShoot;

    public bool isLongRange;
    private Coroutine onEnemyDamageCoroutine;
    private SpawnManager spawnManager;
    void Start()
    {
        spawnManager = SpawnManager.GetStatic();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<Collider2D>();
        objectLayer = LayerMask.GetMask("Object");
        constructionLayer = LayerMask.GetMask("Construction");

        currentDamageRecived = 0f;
        rb.mass = mass;
        rb.linearDamping = linearDamp;
        moveForce = rb.mass * acceleration;
        currentHealth = maxHealth;
        target = null;
        
        shootRoataion = transform.GetChild(0).transform;
        shootPosition = shootRoataion.transform.GetChild(0).transform.GetChild(0).transform;

        if (isLongRange)
        {
            damage = 0;
        }
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

        if(PlayerManager.players.Count == 0 && isPlayerHit == true)
        {
            isPlayerHit = false;
        }
    }

    void GoToTarget()
    {
        FindCloset();

        if (target != null)
        {
            rangeDistance = Vector2.Distance(gameObject.transform.position, target.transform.position);
        }

        if (isLongRange == true && isFind && rangeDistance <= 5 && !isHasHit)
        {

            if (target != null)
            {
                rb.linearVelocity = Vector2.zero;
                Shoot();
            }
        }
        else if(target != null && !isHasHit && !isClash && isMoving)
        {
            Vector2 dir = (target.position - transform.position).normalized;

            rb.linearVelocity = dir * moveForce;

            float angle = Mathf.Atan2(dir.y ,dir.x) * Mathf.Rad2Deg;
            shootRoataion.transform.rotation = Quaternion.Euler(0, 0, angle); 
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

    }
    public void OnEnemyHit(float damage , Vector2 dir, Transform transform, float push)
    {
        if(onEnemyDamageCoroutine != null)
        {
            StopCoroutine(onEnemyDamageCoroutine);
        }
        onEnemyDamageCoroutine = StartCoroutine(EnemyTakeDamage(damage,dir,transform,push));
    }

    IEnumerator EnemyTakeDamage(float damage , Vector2 dir, Transform transform, float push)
    {
        currentDamageRecived += damage;
        currentHealth -= damage;
        if (!transform.CompareTag("Construction"))
        {
            target = transform;
        }
        
        rb.linearVelocity = Vector2.zero;

        rb.AddForce(-dir * push * 2,ForceMode2D.Impulse);

        if(currentDamageRecived >= maxHealth)
        {
            spawnManager.enemiesDead++;
            Destroy(gameObject);
        }

        yield return new WaitForSeconds(1);

        rb.linearVelocity = Vector2.zero;
        isHasHit = false;
    }

    public void FindCloset()
    {
        if(ObjectManager.objects.Count == 0) return;
        
        float minDistance = circleRange;
        Transform closest = null;

        string[] priorityTags = {"Friendly","Player"};
        Collider2D[] founds = Physics2D.OverlapCircleAll(transform.position, circleRange, objectLayer);
        Collider2D[] foundConstructions = Physics2D.OverlapCircleAll(transform.position, circleRange, constructionLayer);

        if(foundConstructions.Length > 0 && founds.Length == 0 && isLongRange)
        {
            isFind = true;
        }
        else if(foundConstructions.Length == 0 && founds.Length == 0)
        {
            isFind = false;
        }
        

        if(founds.Length == 0)
        {   
            //target = null;
            if (isPlayerHit == false)
            {
                minDistance = Mathf.Infinity;
                target = null;
                Transform detected = null;
                foreach (var objects in ObjectManager.objects) // หาสิ่งก่อสร้าง
                {
                    if(objects == null) continue;

                    float distance = Vector2.Distance(gameObject.transform.position, objects.transform.position);

                    if(distance < minDistance)
                    {
                        minDistance = distance;
                        detected = objects.transform;
                    }
                }
                target = detected;
            }
        }
        else if(founds.Length > 0)
        {
            foreach(string tag in priorityTags)
            {
                foreach(Collider2D found in founds)
                {
                    if(found == null) continue;

                    if (found.CompareTag(tag))
                    {
                        float distance = Vector2.Distance(gameObject.transform.position, found.transform.position);

                        if(distance < minDistance)
                        {
                            minDistance = distance;
                            closest = found.transform;
                        }
                    }
                }

                if(closest != null)
                {
                    isFind = true;
                    target = closest;
                    return;
                }
                else
                {
                    isFind = false;
                }
            }
        }
    }

    void Shoot()
    {
        var targetPosition = target.transform.position;

        var dir = (targetPosition - transform.position).normalized;

        float angle = Mathf.Atan2(dir.y ,dir.x) * Mathf.Rad2Deg;

        shootRoataion.transform.rotation = Quaternion.Euler(0, 0, angle); 
        
        if (Time.time >= nextShoot)
        {
            var arrow = Instantiate(
                arrowPrefeb,
                shootPosition.position,
                shootPosition.rotation
            );
            Destroy(arrow, 2);
            nextShoot = Time.time + shootCooldown;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, circleRange);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out FriendlyController friendlyController))    
        {
            isClash = true;
            isHasHit = true; 

            var dir = gameObject.transform.position - friendlyController.transform.position;
            dir.Normalize();

            if (isLongRange)
            {
                damage = 0;
            }

            friendlyController.OnFriendlyHit(damage, dir, pushForce);
        }

        if (collision.gameObject.TryGetComponent(out PlayerController playerController))    
        {
            isClash = true;
            isHasHit = true; 

            var dir = gameObject.transform.position - playerController.transform.position;
            dir.Normalize();
            
            if (isLongRange)
            {
                damage = 0;
            }

            playerController.OnPlayerHit(damage, dir, pushForce);
        }

        if(collision.gameObject.TryGetComponent(out BuildingClass buildingClass))
        {
            isClash = true;
            isHasHit = true; 

            var dir = gameObject.transform.position - buildingClass.transform.position;
            dir.Normalize();
            
            if (isLongRange)
            {
                damage = 0;
            }

            // buildingClass.OnConstructionHit(damage, dir, pushForce);
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
