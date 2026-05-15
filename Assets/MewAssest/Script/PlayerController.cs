using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Basic Character Setting")]
    
    public int playerSpeedUpCount;
    public int enemySpeedDownCount;
    public float maxHealth;
    public float currentHealth;
    public float acceleration = 10f;
    public float mass = 1f;
    public float linearDamp = 5f;
    public float shootCooldown = 1f;
    public float clashDamage = 0;
    public float skillRicochetCooldown = 5f;
    public float skillChargingCooldown = 6f;
    public float healCooldown = 5f;
    public float heal = 5f;
    public bool isChargeSpawn;
    public bool isCanCharge;
    public float chargeAccelerator;
    public float chargeDamage;
    public float currentChargeAccel;
    public float currentChargeDamage;
    public float currentDamageRecive;
    public bool isHasHit;
    
    [Header("GameObject Setting")]
    public Camera cam;
    public GameObject arrowPrefeb;
    public GameObject arrowFirstSkillPrefeb;
    public GameObject arrowSecondSkillPrefeb; 
    public Transform shootPos;
    public Transform shootRotation;
    public bool isPlayerCharging;
    public float pushForce;
    // Hidden Setting
    private float speedBoost;
    private float nextHealTime;
    private bool isHealSetTime;
    private float moveForce;
    private float nextShoot;
    private float currentRicochetCooldown;
    private float time;
    [SerializeField] private GameObject shootCharge;
    private GameStateManager gameStateManager;
    private CameraController cameraController;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction AttackAction;
    private InputAction ChargeAction;
    private InputAction setDefAction;
   
    private Rigidbody2D rb;
    private Collider2D cd;
    private DefenceManager defenceManager;
    private static PlayerController staticInstance;
    private Coroutine onEnemySpeedDown;
    private Coroutine onPlayerSpeedUp;
    private Coroutine onPlayerDamageCoroutine;
    public static PlayerController GetStatic()
    {
        return staticInstance;
        
    }
    void Awake()
    {
        if(staticInstance != null)
        {
            Destroy(this.gameObject);
        }

        staticInstance = this;
        maxHealth = 100;
    }

    void Start()
    {
        nextHealTime = 0;
        playerSpeedUpCount = 0;
        enemySpeedDownCount = 0;
        speedBoost = 1;

        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        defenceManager = DefenceManager.GetStatic();
        isCanCharge = true;
        currentHealth = maxHealth;

        cameraController = CameraController.GetStatic();
        gameStateManager = GameStateManager.GetStatic();

        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("look");
        AttackAction = InputSystem.actions.FindAction("Attack");
        ChargeAction = InputSystem.actions.FindAction("Charge");
        setDefAction = InputSystem.actions.FindAction("SetDef");

        rb = gameObject.GetComponent<Rigidbody2D>();
        cd = gameObject.GetComponent<Collider2D>();
        rb.mass = mass;
        rb.linearDamping = linearDamp;
        moveForce = rb.mass * acceleration;
        nextShoot = 0;
        currentRicochetCooldown = 0;

        cameraController.playerTransform = gameObject.transform;
    }

    void Update()
    {

        if(gameStateManager.isGamePause) return;

        time = Time.time;

        if(isHasHit != true)
        {
            if(isHealSetTime == false)
            {
                nextHealTime = time +  healCooldown;
                isHealSetTime = true;
            }

            if(time >= nextHealTime && currentDamageRecive > 0)
            {
                currentDamageRecive -= heal;
                if(currentHealth < maxHealth)
                {
                    currentHealth += heal;
                }
                
                nextHealTime = time +  healCooldown;
                isHealSetTime = true;

                if(currentDamageRecive < 0)
                {
                    currentDamageRecive = 0;
                }

                if(currentHealth > maxHealth )
                {
                    currentHealth = maxHealth;
                }
            }
        }
        else
        {
            isHealSetTime = false;
        }

        CharacterRotation();
        if ( AttackAction.triggered && time >= nextShoot && isHasHit == false && isPlayerCharging == false)
        {
            CharacterShoot();
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame && enemySpeedDownCount > 0)
        {
            if (onEnemySpeedDown != null) return;
            enemySpeedDownCount--;
            gameStateManager.speedDownTime = 10;
            onEnemySpeedDown = StartCoroutine(EnemySpeedDown());
        }
        gameStateManager.speedDownCounts = enemySpeedDownCount;

        if (Keyboard.current.digit2Key.wasPressedThisFrame && playerSpeedUpCount > 0)
        {
            if (onPlayerSpeedUp != null) return;
            playerSpeedUpCount--;
            gameStateManager.speedUpTime = 10;
            onPlayerSpeedUp = StartCoroutine(playerSpeedUp());
        }
        gameStateManager.speedUpCounts = playerSpeedUpCount;

        if (Keyboard.current.cKey.wasPressedThisFrame && currentRicochetCooldown <= 0 && isHasHit == false && isPlayerCharging == false)
        {
            RicochetSkill();
        }

        if (ChargeAction.IsPressed() && isHasHit == false)
        {
            isPlayerCharging = true;
            shootCharge.SetActive(true);
            StartCoroutine(OnCharge());
        }
        else if(!ChargeAction.IsPressed())
        {
            isPlayerCharging = false;
            shootCharge.SetActive(false);
            shootCharge.transform.localScale = new Vector3(0.1f, 0.1f, 0);

            if (isChargeSpawn == true && isHasHit == false)
            {
                currentChargeAccel = chargeAccelerator;
                currentChargeDamage = chargeDamage;
                isChargeSpawn = false;
                Instantiate(arrowSecondSkillPrefeb,shootPos.position,shootPos.rotation);
            }
            else if (isChargeSpawn == false)
            {
                chargeAccelerator = 0;
                chargeDamage = 0;
            }
        }

        if(currentRicochetCooldown > 0)
        {
            currentRicochetCooldown -= Time.deltaTime;
        }
        else
        {
            gameStateManager.isSkillCooldown = false;
        }

        Vector2 mouseDirection = lookAction.ReadValue<Vector2>();

        if (setDefAction.WasReleasedThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(mouseDirection);
            Debug.DrawRay(ray.origin, ray.direction * 5, Color.red, 5f);

            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null)
            {
                if(hit.collider.TryGetComponent(out FriendlyController friendlyController))
                {
                    var defRangePositions = defenceManager.defRangePositions;
                    var hasSelectDefRange = defenceManager.hasSelectDefRange;

                    var defNormalPositions = defenceManager.defNormalPositions;
                    var hasSelectDefNormal = defenceManager.hasSelectDefNormal;
                    if(friendlyController.isLongRange == true && friendlyController.isDef == false)
                    {
                        friendlyController.isDef = true;

                        int index = UnityEngine.Random.Range(0,defRangePositions.Count);

                        hasSelectDefRange.Add(defenceManager.defRangePositions[index]);

                        friendlyController.defPos = defRangePositions[index];

                        if(defRangePositions.Count > 0)
                        {
                            defRangePositions.RemoveAt(index);
                        }
                    }
                    else if(friendlyController.isLongRange == true && friendlyController.isDef == true)
                    {
                        friendlyController.isDef = false;

                        defRangePositions.Add(friendlyController.defPos);
                        hasSelectDefRange.Remove(friendlyController.defPos);

                        friendlyController.defPos = null;
                    }

                    if (friendlyController.isLongRange == false && friendlyController.isDef == false)
                    {
                        friendlyController.isDef = true;

                        int index = UnityEngine.Random.Range(0,defNormalPositions.Count);

                        hasSelectDefNormal.Add(defNormalPositions[index]);

                        friendlyController.defPos = defNormalPositions[index];

                        if(defNormalPositions.Count > 0)
                        {
                            defNormalPositions.RemoveAt(index);
                        }
                    }
                    else if(friendlyController.isLongRange == false && friendlyController.isDef == true)
                    {
                        friendlyController.isDef = false;

                        defNormalPositions.Add(friendlyController.defPos);
                        hasSelectDefNormal.Remove(friendlyController.defPos);

                        friendlyController.defPos = null;
                    }
                }
                
            }
        }

    }
    public void OnPlayerHit(float damage , Vector2 dir, float push)
    {
        if(onPlayerDamageCoroutine != null)
        {
            StopCoroutine(onPlayerDamageCoroutine);
        }
        onPlayerDamageCoroutine = StartCoroutine(PlayeraTakeDamage(damage,dir,push));
    }

    IEnumerator EnemySpeedDown()
    {
        gameStateManager.isSpeedDownCooldown = true;

        foreach(var enemy in EnemyManager.enemies)
        {
            if(enemy.gameObject.TryGetComponent(out EnemyController enemyController))
            {
                enemyController.acceleration *= 0.5f;
            }
        }

        yield return new WaitForSeconds(2.5f);


        foreach(var enemy in EnemyManager.enemies)
        {
            if(enemy.gameObject.TryGetComponent(out EnemyController enemyController))
            {
                enemyController.acceleration += enemyController.acceleration;
            }
        }
        yield return new WaitForSeconds(7.5f);

        onEnemySpeedDown = null;
    }

    IEnumerator playerSpeedUp()
    {
        gameStateManager.isSpeedUpCooldown = true;

        speedBoost += 0.5f;

        yield return new WaitForSeconds(2.5f);

        speedBoost -= 0.5f;

        yield return new WaitForSeconds(7.5f);

        onPlayerSpeedUp = null;
    }

    IEnumerator PlayeraTakeDamage(float damage , Vector2 dir, float push)
    {

        currentDamageRecive += damage;
        currentHealth -= damage;
        cd.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(-dir * push * 2,ForceMode2D.Impulse);

        if(currentDamageRecive >= maxHealth)
        {
            Destroy(gameObject);
        }

        yield return new WaitForSeconds(0.5f);

        rb.linearVelocity = Vector2.zero;
        cd.enabled = true;
        isHasHit = false; 
    }

    IEnumerator OnCharge()
    {
        if(isCanCharge == false) yield break;
        Vector3 scale = shootCharge.transform.localScale;
        isCanCharge = false;

        scale += new Vector3(0.8f * Time.deltaTime, 0.8f * Time.deltaTime, 0); 
        scale.x = Mathf.Clamp(scale.x, 0.1f,1.5f);
        scale.y = Mathf.Clamp(scale.y, 0.1f,1.5f);

        shootCharge.transform.localScale = scale;

        float accel = chargeAccelerator;
        accel += 8f * Time.deltaTime;
        accel = Mathf.Clamp(accel, 0.1f,2f);
        chargeAccelerator = accel;

        float damage = chargeDamage;
        damage += 8f * Time.deltaTime;
        damage = Mathf.Clamp(damage, 0.1f,15f);
        chargeDamage = damage;

        yield return null;
        
        isCanCharge = true;
        isChargeSpawn = true;
    }
    
    void FixedUpdate()
    {
        if (moveAction.IsPressed() && isHasHit == false)
        {
            CharacterMove();
        }
    }
    private void CharacterMove()
    {
        var v = moveAction.ReadValue<Vector2>();
        rb.AddForce(v * moveForce * speedBoost, ForceMode2D.Force);
    }
    private void CharacterRotation()
    {
        Vector2 mouseDirection = lookAction.ReadValue<Vector2>();

        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseDirection);
        mouseWorld.z = 0;
        
        Vector2 direction = mouseWorld - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        shootRotation.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void CharacterShoot()
    {
        var arrow = Instantiate(
            arrowPrefeb,
            shootPos.position,
            shootPos.rotation
        );
        Destroy(arrow, 2);
        nextShoot = Time.time + shootCooldown;
    }
    private void RicochetSkill()
    {
        Instantiate(
            arrowFirstSkillPrefeb,
            shootPos.position,
            shootPos.rotation
        );
        currentRicochetCooldown = skillRicochetCooldown;
        gameStateManager.skillCoolDownCount = skillRicochetCooldown;
        gameStateManager.isSkillCooldown = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out EnemyController enemyController))    
        {
            isHasHit = true; 

            var dir = gameObject.transform.position - enemyController.transform.position;
            dir.Normalize();

            enemyController.isHasHit = true;
            enemyController.OnEnemyHit(clashDamage, dir, gameObject.transform, pushForce);
        }

        
    }

    
}
