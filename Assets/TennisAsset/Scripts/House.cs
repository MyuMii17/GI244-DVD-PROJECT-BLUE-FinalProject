using UnityEngine;

public class House : BuildingClass
{
   
    override public void Output()
    {
        base.Output();
        if (this.IsBuilt)
        {
            Resource.GetInstance().Citizen += ProduceAmount;
        }
        
    }
}
