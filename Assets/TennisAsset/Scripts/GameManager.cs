using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private InputAction repair;
    InputAction inputAction;
    bool isGameEnd;
    [field: SerializeField] protected List<BuildingClass> buildingList = new List<BuildingClass>();
    public TMP_Text GoldText;
    public TMP_Text FoodText;
    public TMP_Text CitizenText;
    private bool isFixSetTime;
    private float nextFixTime;

    private void Awake()
    {
        repair = InputSystem.actions.FindAction("Interact");
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
        Vector2 screenPos = Mouse.current.position.ReadValue();
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            Debug.DrawRay(ray.origin, ray.direction * 5, Color.red, 5f);

            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null && hit.collider.TryGetComponent(out Barrack barrack))
            {
                barrack.ShowUnitBuyUi();
            }
        }

        //if (inputAction.triggered)
        //{
        //    Debug.Log("House: " + buildingList[0].IsBuilt  + "Gold: " + Resource.GetInstance().Gold + "Citizen: " + Resource.GetInstance().Citizen);
        //    Debug.Log("Produce: " + buildingList[0].ProduceAmount);
        //    BuildingsOuput();
        //}
        if (GoldText != null && FoodText != null)
        {
            GoldText.text = Resource.GetInstance().Gold.ToString();
            FoodText.text = Resource.GetInstance().Food.ToString();
            CitizenText.text = Resource.GetInstance().Citizen.ToString();
        }
        RepairCooldown();
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

    public void AllBuildingRepair()
    {
        foreach(var building in buildingList)
        {
            building.Repair();
        }
    }

    void RepairCooldown()
    {
        var fixCooldown = 10f;
        if (isFixSetTime == false)
        {
            isFixSetTime = true;
            nextFixTime = Time.time + fixCooldown;
        }

        if (repair.IsPressed() && Time.time >= nextFixTime)
        {
            Debug.Log("E");
            AllBuildingRepair();
            nextFixTime = Time.time + fixCooldown;
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
