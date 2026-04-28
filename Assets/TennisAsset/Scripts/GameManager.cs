using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    InputAction inputAction;

    [field: SerializeField] protected List<BuildingClass> buildingList = new List<BuildingClass>();


    private void Awake()
    {
        InitAllBuilding();
        inputAction = InputSystem.actions.FindAction("Jump");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inputAction.triggered)
        {
            Debug.Log("House: " + buildingList[0].IsBuilt  + "Gold: " + Resource.GetInstance().Gold + "Citizen: " + Resource.GetInstance().Citizen);
            Debug.Log("Produce: " + buildingList[0].ProduceAmount);
            buildingList[0].Output();
        }
    }


    void InitAllBuilding()
    {
        buildingList[0].Init(100, 50, 10, false);
        buildingList[1].Init(150, 100, 20, false);
        buildingList[2].Init(200, 150, 30, false);
        buildingList[3].Init(250, 200, 40, false);
        buildingList[4].Init(300, 250, 50, false);
    }


    public void HouseBuildButton()
    {
        buildingList[0].Build();
    }

    public void BarrackBuildButton()
    {
        buildingList[1].Build();
    }

    public void FarmBuildButton()
    {
        buildingList[2].Build();
    }

    public void MineBuildButton()
    {
        buildingList[3].Build();
    }

    public void ApothecaryBuildButton()
    {
        buildingList[4].Build();
    }
}
