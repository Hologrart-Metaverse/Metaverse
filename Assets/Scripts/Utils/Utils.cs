using UnityEngine;

public static class Utils
{
    public static bool IsChooseScreenOn;
    public static Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-2, 2), -2, Random.Range(-15, -20));
    }
    public static Vector3 GetRandomPositionAtCertainPoint(Vector3 point)
    {
        return new Vector3(Random.Range(point.x - 2, point.x + 2), point.y, Random.Range(point.z - 2, point.z + 2));
    }
    public static void SetMouseLockedState(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public static void SetRenderLayerInChildren(Transform transform, int layerNumber)
    {
        foreach (Transform trans in transform.GetComponentsInChildren<Transform>(true))
        {
            if (trans.CompareTag("IgnoreLayerChange"))
                continue;
            trans.gameObject.layer = layerNumber;
        }
    }
}
