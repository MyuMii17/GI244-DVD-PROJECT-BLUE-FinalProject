using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendliesPool : MonoBehaviour
{
    [SerializeField] private GameObject friendlyPrefab;
    [SerializeField] private int poolSize = 10;

    private readonly List<GameObject> friendlyPool = new();

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
        }
    }


    private void CreateNewFriend()
    {
        var newFriend = Instantiate(friendlyPrefab);
        newFriend.SetActive(false);
        friendlyPool.Add(newFriend);
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
        friendlyPool.Add(friend);
        friend.SetActive(false);
        
    }
}
