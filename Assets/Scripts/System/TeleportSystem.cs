using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TeleportPoint
{
    public Areas area;
    public Vector3 point;
}
public class TeleportSystem : MonoBehaviour
{
    public static TeleportSystem Instance;
    [SerializeField] private List<TeleportPoint> teleportPoints = new List<TeleportPoint>();
    private void Awake()
    {
        Instance = this;
        Area.currentArea = Areas.Hangar;
    }

    public void Teleport(Areas area)
    {
        if (!IsAreaSuitable(area))
            return;

        Vector3 pos = GetPositionByArea(area);
        PlayerController.Local.Teleport(pos);
        Area.currentArea = area;
    }
 
    public List<Areas> GetSuitableAreas()
    {
        List<Areas> areaList = new List<Areas>();
        foreach(var teleportPoint in teleportPoints)
        {
            if (teleportPoint.area == Area.currentArea)
                continue;
            areaList.Add(teleportPoint.area);
        }
        return areaList;
    }
    private bool IsAreaSuitable(Areas area)
    {
        //Check area
        return true;
    }

    private Vector3 GetPositionByArea(Areas area)
    {
        foreach (TeleportPoint teleportPoint in teleportPoints)
        {
            if (teleportPoint.area == area)
            {
                return Utils.GetRandomPositionAtCertainPoint(teleportPoint.point);
            }
        }
        return Vector3.zero;
    }
}
