using UnityEngine;

public static class Utils
{
    public static Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-2, 2), -3, Random.Range(-17, -22));
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
}
