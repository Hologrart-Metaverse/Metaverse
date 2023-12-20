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
    public void TeleportArea(Area area, bool playTeleportEffect = false)
    {
        if (!AreaSystem.Instance.IsAreaSuitable(area))
            return;

        Vector3 pos = GetRandomPositionAtCertainPoint(AreaSystem.Instance.GetArea(area).teleportPosition);
        PlayerController.Local.Teleport(pos, default, playTeleportEffect);
        AreaSystem.Instance.currentArea = area;
    }
    public void TeleportPosition(Vector3 pos, Quaternion rot = default)
    {
        PlayerController.Local.Teleport(GetRandomPositionAtCertainPoint(pos), rot);
    }
    private Vector3 GetRandomPositionAtCertainPoint(Vector3 point)
    {
        return Utils.GetRandomPositionAtCertainPoint(point);
    }
}
