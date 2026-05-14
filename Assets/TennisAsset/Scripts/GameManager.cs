using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    InputAction inputAction;
    bool isGameEnd;
    [field: SerializeField] protected List<BuildingClass> buildingList = new List<BuildingClass>();

    public TMP_Text GoldText;
    public TMP_Text FoodText;
    public TMP_Text CitizenText;
    private void Awake()
    {
        inputAction = InputSystem.actions.FindAction("Jump");
        InitAllBuilding();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Produce());
        Resource.GetInstance().Gold = 0;
        Resource.GetInstance().Food = 50;
        Resource.GetInstance().Citizen = 0;
        buildingList[5].Build();
        
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
        if (GoldText != null && FoodText != null)
        {
            GoldText.text = Resource.GetInstance().Gold.ToString();
            FoodText.text = Resource.GetInstance().Food.ToString();
            CitizenText.text = Resource.GetInstance().Citizen.ToString();
        }
    }


    void InitAllBuilding()
    {
        //House
        buildingList[0].Init(50, 10, 5, false, 80f);

        //Barrack
        buildingList[1].Init(150, 20, 0, false, 110f);

        //Farm
        buildingList[2].Init(20, 10, 10, false, 40f);

        //Mine
        buildingList[3].Init(100, 30, 20, false, 60f);

        //Apothecary
        buildingList[4].Init(150, 20, 0, false, 80f);

        //Castle
        buildingList[5].Init(0, 0, 10, false, 180f);
    }

    void BuildingsOuput()
    {
       foreach(var building in buildingList)
        {
            building.Output();
        }
    }

    IEnumerator Produce()
    {
        while (!isGameEnd)
        {
            yield return new WaitForSeconds(2f);
            BuildingsOuput();
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
