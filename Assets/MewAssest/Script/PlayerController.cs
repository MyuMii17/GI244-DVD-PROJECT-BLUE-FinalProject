using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Basic Character Setting")]
    public float acceleration = 10f;
    public float mass = 1f;
    public float linearDamp = 5f;
    public float shootCooldown = 1f;
    public float skillRicochetCooldown = 5f;
    public float skillChargingCooldown = 6f;
    [Header("GameObject Setting")]
    public Camera cam;
    public GameObject arrowPrefeb;
    public GameObject arrowFirstSkillPrefeb;
    public GameObject arrowSecondSkillPrefeb;
    public Transform shootPos;
    public Transform shootRotation;
    // Hidden Setting
    private float moveForce;
    private float nextShoot;
    private float currentRicochetCooldown;
    private float currentChargingCooldown;
    private float time;
    private bool isChargeSuccess;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction AttackAction;
    private Rigidbody2D rb;
    private static PlayerController staticInstance;
    public static PlayerController GetStatic()
    {
        return staticInstance;
    }

    void Start()
    {
        staticInstance = this;
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("look");
        AttackAction = InputSystem.actions.FindAction("Attack");
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.linearDamping = linearDamp;
        moveForce = rb.mass * acceleration;
        nextShoot = 0;
        currentRicochetCooldown = 0;
        currentChargingCooldown = 0;
    }

    void Update()
    {
        time = Time.time;
        CharacterRotation();
        if ( AttackAction.triggered && time >= nextShoot)
        {
            CharacterShoot();
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame && currentRicochetCooldown <= 0)
        {
            RicochetSkill();
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame && currentChargingCooldown <= 0)
        {
            this.enabled = false;
            ChargeSkill();
        }

        if(currentRicochetCooldown > 0)
        {
            currentRicochetCooldown -= Time.deltaTime;
        }   

        if(currentChargingCooldown > 0 && isChargeSuccess)
        {
            currentChargingCooldown -= Time.deltaTime;
        }
        else
        {
            isChargeSuccess = false;
        }

    }
    void FixedUpdate()
    {
        if (moveAction.IsPressed())
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
        Instantiate(
            arrowPrefeb,
            shootPos.position,
            shootPos.rotation
        );
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
    private void ChargeSkill()
    {
        Instantiate(
            arrowSecondSkillPrefeb,
            shootPos.position,
            shootPos.rotation
        );
        currentChargingCooldown = skillChargingCooldown;
    }
    public bool isUseCharging()
    {
        if (GetStatic().currentChargingCooldown > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void ChargeSkillCooldown()
    {
        isChargeSuccess = true;
    }
    void OnEnable()
    {
        ArrowChargingSkill.ChargeSuccess += ChargeSkillCooldown;
    }
}
