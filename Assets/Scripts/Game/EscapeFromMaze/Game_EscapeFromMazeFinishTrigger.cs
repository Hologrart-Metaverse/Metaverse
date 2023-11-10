using UnityEngine;

public class Game_EscapeFromMazeFinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GetComponentInParent<Game_EscapeFromMaze>().OnFinish();
    }
}
