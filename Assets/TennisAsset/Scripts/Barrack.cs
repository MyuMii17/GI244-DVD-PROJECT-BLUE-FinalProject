using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Barrack : BuildingClass, IPointerClickHandler
{
    [SerializeField] private Transform rallyPos;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked");
        var friend = FriendliesPool.GetInstance().GetFriend();
        friend.transform.position = rallyPos.position;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  
}
