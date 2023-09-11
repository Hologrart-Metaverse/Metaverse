using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-2, 2), -3, Random.Range(-17, -22));
    }
}
