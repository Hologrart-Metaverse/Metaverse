using UnityEngine;
[CreateAssetMenu()]

public class ArtifactSO : ScriptableObject
{
    public string artifactName;
    [TextArea(5, 10)]
    public string description;
    public ArtifactType ArtifactType;
    public Sprite artifactSprite;
}
