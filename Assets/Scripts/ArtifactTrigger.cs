using UnityEngine;

public class ArtifactTrigger : MonoBehaviour, I_Interactable
{
    private Artifact artifact;
    private void Start()
    {
        artifact = GetComponentInParent<Artifact>();
    }
    public void Interact()
    {
        artifact.OnInteracted();
    }
    public void OnFaced()
    {
        InteractionUI.Instance.ShowText("Artifact (" + artifact.artifactSO.artifactName + ")");
    }

    public void OnInteractEnded()
    {
        InteractionUI.Instance.HideText();
    }
}
