using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform playerTransform;
    private Camera cam;

     void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        cam = GetComponent<Camera>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        if(playerTransform != null)
        {
            transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, -10f);
        }
    }
}
