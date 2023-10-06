using UnityEngine;

public class NFTScreenTrigger : MonoBehaviour, I_Interactable
{
    private NFTScreen nftScreen;
    private void Awake()
    {
        nftScreen = GetComponentInParent<NFTScreen>();
    }
    public void Interact()
    {
        nftScreen.ChangeMod(NFTScreen.Mode.Edit);
    }

    public void OnFaced()
    {
        Debug.Log("iþte yüzleþtik");
    }

    public void OnInteractEnded()
    {
    }

}
