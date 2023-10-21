using UnityEngine;

public class JointNFTTrigger : MonoBehaviour, I_Interactable
{
    private JointNFT nftScreen;
    private void Awake()
    {
        nftScreen = GetComponentInParent<JointNFT>();
    }
    public void Interact()
    {
        nftScreen.ChangeMode(UIMode.StaticEdit);
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
