using UnityEngine;
using UnityEngine.InputSystem;

public class RepairArea : MonoBehaviour
{
    private bool isFixSetTime;
    private float nextFixTime;
    private InputAction repairAction;
    private BuildingClass buildingClass;
    public bool isPlayerInArea;
    public bool CanRepair;
    private void Awake()
    {
        repairAction = InputSystem.actions.FindAction("Interact");
    }

    private void Update()
    {

        //if (repairAction.triggered && CanRepair == true && Time.time >= nextFixTime && buildingClass != null)
        //{
        //    Debug.Log("E");
        //    buildingClass.Repair();
        //    nextFixTime = Time.time + 0.1f;
        //}

        //if (isPlayerInArea == true)
        //{
        //   RepairCooldown();
        //}
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        isPlayerInArea = true;
        var fixCooldown = 0.5f;
        if (isPlayerInArea == true)
        {
            if (collision.gameObject.TryGetComponent(out BuildingClass buildingClass))
            {
                CanRepair = true;
                buildingClass = collision.gameObject.GetComponent<BuildingClass>();
                if (isFixSetTime == false)
                {
                    isFixSetTime = true;
                    nextFixTime = Time.time + fixCooldown;
                }

                if (repairAction.IsPressed() && Time.time >= nextFixTime)
                {
                    Debug.Log("E");
                    buildingClass.Repair();
                    nextFixTime = Time.time + fixCooldown;
                }

            }
        }

    }

    //void RepairCooldown()
    //{
    //    var fixCooldown = 0.5f;
    //    if (isFixSetTime == false)
    //    {
    //        isFixSetTime = true;
    //        nextFixTime = Time.time + fixCooldown;
    //    }

    //    if (repairAction.IsPressed() && Time.time >= nextFixTime)
    //    {
    //        Debug.Log("E");
    //        buildingClass.Repair();
    //        nextFixTime = Time.time + fixCooldown;
    //    }
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerInArea = false;
        isFixSetTime = false;
        CanRepair = false;
    }

}
