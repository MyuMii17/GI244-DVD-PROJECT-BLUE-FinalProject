using UnityEngine;

public class House : BuildingClass
{
   private int citizenProduceAmount = 6;
    override public void Output()
    {
        base.Output();
        if (this.IsBuilt)
        {
            Resource.GetInstance().Citizen += citizenProduceAmount;
        }
        
    }
}
