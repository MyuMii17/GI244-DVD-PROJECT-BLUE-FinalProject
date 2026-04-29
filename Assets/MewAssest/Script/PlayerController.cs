using System.Collections;
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
    public bool isCharging;
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
    private InputAction ChargeAction;
    private Rigidbody2D rb;
    private Coroutine OnChargingCoroutine;
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
        ChargeAction = InputSystem.actions.FindAction("Charge");
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

        if (ChargeAction.IsPressed())
        {
            if(OnChargingCoroutine != null)
            {
                isCharging = true;
            }
            else
            {
                isCharging = true;
                OnChargingCoroutine = StartCoroutine(OnCharge());
            }
        }
        else if(!ChargeAction.IsPressed())
        {
            isCharging = false;
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
    IEnumerator OnCharge()
    {
        var box = arrowSecondSkillPrefeb.GetComponent<BoxCollider2D>();
        var arrowCharge = arrowSecondSkillPrefeb.GetComponent<ArrowChargingSkill>();

        ChargeSkill();

        arrowSecondSkillPrefeb.transform.localScale = new Vector3(0.8834218f,0.8834218f,0);

        arrowCharge.isCharging = true;
        arrowCharge.canShoot = false;

        while (isCharging == true)
        {

            box.enabled = false;

            arrowSecondSkillPrefeb.transform.localScale += new Vector3(arrowCharge.arrowScale * Time.deltaTime, arrowCharge.arrowScale * Time.deltaTime, 0f);
            arrowSecondSkillPrefeb.transform.position = PlayerController.GetStatic().shootPos.position;

            arrowCharge.acceleration += 2f * Time.deltaTime;

            if(arrowCharge.arrowForce >= arrowCharge.maxArrowForce)
            {
                arrowCharge.arrowForce = arrowCharge.maxArrowForce;
            }


        }

        if(isCharging == false)
        {

            box.enabled = true;
            arrowCharge.isCharging = false;
            arrowCharge.canShoot = true;
        }

        yield return new WaitForSeconds(1);
        
        OnChargingCoroutine = null;
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
    }

}
