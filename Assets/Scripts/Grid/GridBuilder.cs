using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    private Grid grid;
    void Awake()
    {
        grid = new Grid(64, 64, .1f, transform.position + new Vector3(.1f, -3.2f, -3.2f));
    }
    public Grid GetGrid() { return grid; }
}
