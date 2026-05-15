using UnityEngine;

public class Farm : BuildingClass
{
    override public void Output()
    {
        if (this.IsBuilt)
        {
            Resource.GetInstance().Food += ProduceAmount;
        }
    }
}
