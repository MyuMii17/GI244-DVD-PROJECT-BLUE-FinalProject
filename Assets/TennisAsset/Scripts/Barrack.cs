using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Barrack : BuildingClass, IPointerClickHandler
{
    [SerializeField] private Transform rallyPos;
    [SerializeField] private GameObject unitBuyUi;

    private void Awake()
    {
        HealthBar.transform.position = new Vector2(transform.position.x, transform.position.y + 0.8f);
        gameObject.SetActive(false);
        HealthBar.SetActive(true);
        unitBuyUi.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
        Debug.Log("Clicked");
        if (unitBuyUi.activeSelf)
        {
            unitBuyUi.SetActive(false);
        }
        else
        {
            unitBuyUi.SetActive(true);
        }  
        
    }

    public void TrainInfantry()
    {
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
