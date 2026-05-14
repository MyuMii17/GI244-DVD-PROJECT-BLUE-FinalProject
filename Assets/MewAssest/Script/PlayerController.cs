using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Basic Character Setting")]
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
    private float nextHealTime;
    private bool isHealSetTime;
    private float moveForce;
    private float nextShoot;
    private float currentRicochetCooldown;
    private float time;
    [SerializeField] private GameObject shootCharge;
    private CameraController cameraController;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction AttackAction;
    private InputAction ChargeAction;
    private InputAction setDefAction;
    private InputAction repairAction;
    private Rigidbody2D rb;
    private Collider2D cd;
    private DefenceManager defenceManager;
    private static PlayerController staticInstance;
    private Coroutine OnChargingCoroutine;
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

        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        defenceManager = DefenceManager.GetStatic();
        isCanCharge = true;
        currentHealth = maxHealth;

        cameraController = CameraController.GetStatic();

        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("look");
        AttackAction = InputSystem.actions.FindAction("Attack");
        ChargeAction = InputSystem.actions.FindAction("Charge");
        setDefAction = InputSystem.actions.FindAction("SetDef");
        repairAction = InputSystem.actions.FindAction("Interact");
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

        if (Keyboard.current.digit1Key.wasPressedThisFrame && currentRicochetCooldown <= 0 && isHasHit == false && isPlayerCharging == false)
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
        rb.AddForce(v * moveForce, ForceMode2D.Force);
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out BuildingClass buildingClass))
        {
            var fixCooldown = 1f;
            var nextFixTime = Time.time + fixCooldown;
            if (repairAction.WasPressedThisFrame() && Time.time >= nextFixTime)
            {
                Debug.Log("Repairing... Current Health: " + currentHealth);
                buildingClass.Repair();
                nextFixTime = Time.time + fixCooldown;
            }
        }
    }
}
