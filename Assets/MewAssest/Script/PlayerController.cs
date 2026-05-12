using System.Collections;
using System.Data.Common;
using Unity.Mathematics;
using Unity.VisualScripting;
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
    public bool isChargeSpawn;
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
    private Rigidbody2D rb;
    private static PlayerController staticInstance;
    private Coroutine OnChargingCoroutine;
    public bool isCanCharge;
    public float chargeAccelerator;
    public float chargeDamage;
    public float currentChargeAccel;
    public float currentChargeDamage;
    public float currentDamageRecive;
    public bool isHasHit;
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
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        isCanCharge = true;
        currentHealth = maxHealth;

        cameraController = CameraController.GetStatic();

        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("look");
        AttackAction = InputSystem.actions.FindAction("Attack");
        ChargeAction = InputSystem.actions.FindAction("Charge");
        rb = gameObject.GetComponent<Rigidbody2D>();
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
        
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(-dir * push * 2,ForceMode2D.Impulse);

        if(currentDamageRecive >= maxHealth)
        {
            Destroy(gameObject);
        }

        yield return new WaitForSeconds(0.5f);

        rb.linearVelocity = Vector2.zero;

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
}
