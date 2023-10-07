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
        InteractionUI.Instance.ShowText("Joint NFT");
    }

    public void OnInteractEnded()
    {
        InteractionUI.Instance.HideText();
    }

}
