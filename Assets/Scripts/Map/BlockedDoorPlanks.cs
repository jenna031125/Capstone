using UnityEngine;
using Yarn.Unity;

public class BlockedDoorPlanks : MonoBehaviour
{
    [YarnCommand("remove_wood_planks")]
    public static void RemovePlanks()
    {
        BlockedDoorPlanks planks = FindFirstObjectByType<BlockedDoorPlanks>(FindObjectsInactive.Include);
        if (planks != null)
        {
            planks.gameObject.SetActive(false); 
        }
    }
}