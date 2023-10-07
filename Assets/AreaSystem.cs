using System.Collections.Generic;
using UnityEngine;
public enum Area
{
    Hangar,
    Culture_Planet,
    Social_Planet,
    Art_Planet,
}
public class AreaSystem : MonoBehaviour
{
    public static AreaSystem Instance { get; private set; }
    [SerializeField] private List<AreaSO> Areas = new List<AreaSO>();
    public Area currentArea { get; set; }
    private void Awake()
    {
        Instance = this;
    }
    public AreaSO GetArea(Area area)
    {
        foreach(AreaSO areaSO in Areas)
        {
            if(areaSO.Area == area) return areaSO;
        }
        return null;
    }
    public List<Area> GetAllAreasExceptCurrent()
    {
        List<Area> tempAreaList = new List<Area>();
        foreach(AreaSO areaSO in Areas)
        {
            if (areaSO.Area == currentArea)
                continue;
            tempAreaList.Add(areaSO.Area);
        }
        return tempAreaList; 
    }
    public bool IsAreaSuitable(Area area)
    {
        //Check area
        return true;
    }
    public List<AreaSO> GetAllAreaSOs()
    {
        return Areas;
    }
    public AreaSO GetCurrentArea()
    {
        foreach(AreaSO areaSO in Areas)
            if(areaSO.Area == currentArea)
                return areaSO;

        return null;
    }
}
