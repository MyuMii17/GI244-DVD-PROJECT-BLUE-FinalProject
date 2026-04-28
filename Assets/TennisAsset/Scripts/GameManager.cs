using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    BuildingClass building;

    [field: SerializeField] protected List<BuildingClass> buildingList = new List<BuildingClass>();


    private void Awake()
    {
        buildingList[0].Init(100, 50, false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HouseBuildButton()
    {
        buildingList[0].Build();
    }
}
