using UnityEngine;

public class RangeDefControl : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out FriendlyController friendlyController))
        {
            if(friendlyController.isLongRange == true)
            {
                friendlyController.isHere = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out FriendlyController friendlyController))
        {
            if(friendlyController.isLongRange == true)
            {
                friendlyController.isHere = false;
            }
        }
    }
}
