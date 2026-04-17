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
    [Header("GameObject Setting")]
    public Camera cam;
    public GameObject arrowPrefeb;
    public GameObject arrowFirstSkillPrefeb;
    public GameObject arrowSecondSkillPrefeb;
    public Transform shootPos;
    // Hidden Setting
    private float moveForce;
    private float nextShoot;
    private float time;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction AttackAction;
    private Rigidbody2D rb;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("look");
        AttackAction = InputSystem.actions.FindAction("Attack");
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.linearDamping = linearDamp;
        moveForce = rb.mass * acceleration;
        nextShoot = 0;
    }

    void Update()
    {
        time = Time.time;
        CharacterRotation();
        if ( AttackAction.triggered && time >= nextShoot)
        {
            CharacterShoot();
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            FirstSkill();
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SecondSkill();
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
        transform.rotation = Quaternion.Euler(0, 0, angle);
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
    private void FirstSkill()
    {
        Instantiate(
            arrowFirstSkillPrefeb,
            shootPos.position,
            shootPos.rotation
        );
    }
    private void SecondSkill()
    {
        Instantiate(
            arrowSecondSkillPrefeb,
            shootPos.position,
            shootPos.rotation
        );
    }
}
