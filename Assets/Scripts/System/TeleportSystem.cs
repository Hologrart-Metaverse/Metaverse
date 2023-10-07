using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;


public class TeleportSystem : MonoBehaviour
{
    public static TeleportSystem Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        AreaSystem.Instance.currentArea = Area.Hangar;
    }
    public void Teleport(Area area)
    {
        if (!AreaSystem.Instance.IsAreaSuitable(area))
            return;

        Vector3 pos = GetRandomPositionAtCertainPoint(AreaSystem.Instance.GetArea(area).teleportPosition);
        PlayerController.Local.Teleport(pos);
        AreaSystem.Instance.currentArea = area;
    }

    private Vector3 GetRandomPositionAtCertainPoint(Vector3 point)
    {
        return Utils.GetRandomPositionAtCertainPoint(point);
    }
}
