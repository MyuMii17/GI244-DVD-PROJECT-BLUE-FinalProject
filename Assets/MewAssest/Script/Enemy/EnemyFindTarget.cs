using System.Collections.Generic;
using UnityEngine;

public class EnemyFindTarget : MonoBehaviour
{
    public float mass = 1f;
    public float acceleration = 5f;
    public float linearDamp = 0f;
    private float moveForce;
    private Rigidbody2D rb;
    private Transform target;
    
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.linearDamping = linearDamp;
        moveForce = rb.mass * acceleration;
    }

    void Update()
    {
        target = FindClosest("Friendly");
    }
    private Transform FindClosest(string tag)
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
}
