using System;
using System.Collections.Generic;
using UnityEngine;
public class Grid
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private int[,] gridArray;
    private Dictionary<int, Color> gridColors = new Dictionary<int, Color>();
    public Grid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[width, height];

        bool showDebug = true;
        if (showDebug)
        {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
            {
                //debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z].ToString();
            };
        }
        int v = 0;
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = v;
                v++;
            }
        }
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(0, x, y) * cellSize + originPosition;
    }

    internal void GetXZ(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void SetGridColor(int x, int y, Color color)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            int gridValue = gridArray[x, y];
            if (gridColors.ContainsKey(gridValue))
            {
                gridColors[gridValue] = color;
            }
            else
            {
                gridColors.Add(gridValue, color);
            }

            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
        }
    }

    public void SetGridColor(Vector3 worldPosition, Color color)
    {
        int x, y;
        GetXZ(worldPosition, out x, out y);
        SetGridColor(x, y, color);
    }

    public Color GetGridColor(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            var gridValue = gridArray[x, y];
            if (gridColors.ContainsKey(gridValue))
            {
                return gridColors[gridValue];
            }
            return default;
        }
        else
        {
            return default;
        }
    }

    public Color GetBuilding(Vector3 worldPosition)
    {
        int x, y;
        GetXZ(worldPosition, out x, out y);
        return GetGridColor(x, y);
    }

}
