using UnityEngine;

public static class Utils
{
    public static bool IsUIScreenOpen;
    private static Vector3 vector = Vector3.zero;
    public static Vector3 GetRandomPositionAtHangar()
    {
        return new Vector3(Random.Range(-2, 2), -2, Random.Range(-15, -20));
    }
    public static Vector3 GetRandomPositionAtCertainPoint(Vector3 point, float range = 2f)
    {
        return new Vector3(Random.Range(point.x - range, point.x + range), point.y, Random.Range(point.z - range, point.z + range));
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
    public static Vector3 BuildVectorWithSameFloat(float value)
    {
        vector.x = vector.y = vector.z = value;
        return vector;
    }
    public static void SetParent(Transform child, Transform parent, bool isFirstSibling = false)
    {
        child.SetParent(parent);

        if (isFirstSibling)
        {
            child.SetSiblingIndex(0);
        }
    }
}
