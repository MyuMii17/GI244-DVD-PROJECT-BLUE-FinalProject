using UnityEngine;

public class NormalDefControl : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out FriendlyController friendlyController))
        {
            if(friendlyController.isLongRange == false)
            {
                friendlyController.isHere = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out FriendlyController friendlyController))
        {
            if(friendlyController.isLongRange == false)
            {
                friendlyController.isHere = false;
            }
        }
    }
}
