using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendliesPool : MonoBehaviour
{
    [SerializeField] private GameObject friendlyPrefab;
    [SerializeField] private GameObject rangeFriendlyPrefab;
    [SerializeField] private int poolSize = 10;

    private readonly List<GameObject> friendlyPool = new();
    private readonly List<GameObject> rangeFriendlyPool = new();
    public static FriendliesPool friendInstance = null;

    public static FriendliesPool GetInstance()
    {
        return friendInstance;
    }

    private void Awake()
    {
        if (friendInstance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        friendInstance = this;
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewFriend();
            if (i % 5 == 0)
            {
                yield return null;
            }

            CreateNewRangeFriend();
            if (i % 5 == 0)
            {
                yield return null;
            }
        }


    }


    private void CreateNewFriend()
    {
        var newFriend = Instantiate(friendlyPrefab);
        newFriend.SetActive(false);
        friendlyPool.Add(newFriend);
    }


    private void CreateNewRangeFriend()
    {
        var newRangeFriend = Instantiate(rangeFriendlyPrefab);
        newRangeFriend.SetActive(false);
        rangeFriendlyPool.Add(newRangeFriend);
    }

    public GameObject GetRangeFriend()
    {
        if (rangeFriendlyPool.Count == 0 && rangeFriendlyPool.Count !> poolSize)
        {
            CreateNewRangeFriend();
        }
        var rangeFriend = rangeFriendlyPool[0];
        rangeFriendlyPool.RemoveAt(0);

        rangeFriend.SetActive(true);
        return rangeFriend;
    }

    public GameObject GetFriend()
    {
       if (friendlyPool.Count == 0 && friendlyPool.Count !> poolSize)
        {
            CreateNewFriend();
        }

        var friend = friendlyPool[0];
        friendlyPool.RemoveAt(0);

        friend.SetActive(true);
        return friend;
    }

    

    public void ReturnFriend(GameObject friend)
    {
        Resource.GetInstance().MaxCitizen += 3;
        
        friendlyPool.Add(friend);
        friend.SetActive(false);
        
    }

    public void ReturnRangeFriend(GameObject rangeFriend)
    {
        Resource.GetInstance().MaxCitizen += 10;
        
        rangeFriendlyPool.Add(rangeFriend);
        rangeFriend.SetActive(false);
    }
}
