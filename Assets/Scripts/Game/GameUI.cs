using System.Collections.Generic;
using UnityEngine;

public abstract class GameUI : MonoBehaviour
{
    internal List<int> memberIds = new();
    internal bool isInitialized = false;
    public void Show()
    {
        Initialize();
        isInitialized = true;
    }
    public abstract void Initialize();
}
