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
        Resource.GetInstance().Gold = 500;
        Resource.GetInstance().Food = 50;
        Resource.GetInstance().Citizen = 20;
    }

    // Update is called once per frame
    void Update()
    {
        if (inputAction.triggered)
        {
            Debug.Log("House: " + buildingList[0].IsBuilt  + "Gold: " + Resource.GetInstance().Gold + "Citizen: " + Resource.GetInstance().Citizen);
            Debug.Log("Produce: " + buildingList[0].ProduceAmount);
            BuildingsOuput();
        }
    }


    void InitAllBuilding()
    {
        buildingList[0].Init(50, 10, 10, false);
        buildingList[1].Init(150, 20, 20, false);
        buildingList[2].Init(20, 10, 30, false);
        buildingList[3].Init(100, 30, 40, false);
        buildingList[4].Init(150, 20, 50, false);
    }

    void BuildingsOuput()
    {
       foreach(var building in buildingList)
        {
            building.Output();
        }
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
