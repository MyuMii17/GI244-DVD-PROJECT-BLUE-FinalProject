using UnityEngine;

public class Castle : BuildingClass
{
    private void OnDisable()
    {
        GameStateManager.GetStatic().isGameOver = true;
    }
}
