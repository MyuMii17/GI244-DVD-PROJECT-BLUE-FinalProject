using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    private static CameraController StaticInstance = null;
    public static CameraController GetStatic()
    {
        return StaticInstance;
    }
     void Awake()
    {
        if(StaticInstance != null)
        {
            Destroy(this.gameObject);
        }

        StaticInstance = this;
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
