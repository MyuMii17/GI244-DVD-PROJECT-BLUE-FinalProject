using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Barrack : BuildingClass, IPointerClickHandler
{
    [SerializeField] private Transform rallyPos;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked");
        if (Resource.GetInstance().Citizen >= 3 && Resource.GetInstance().Gold >= 10 && Resource.GetInstance().Food >= 10)
        {
           
            var friend = FriendliesPool.GetInstance().GetFriend();
            var friendlyController = friend.GetComponent<FriendlyController>();
            friendlyController.isHasHit = false;
            Resource.GetInstance().MaxCitizen -= 3;
            Resource.GetInstance().Citizen -= 3;
            Resource.GetInstance().Gold -= 10;
            Resource.GetInstance().Food -= 10;
            friend.transform.position = rallyPos.position;
        }
        
    }

  
}
