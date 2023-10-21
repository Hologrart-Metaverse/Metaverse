using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ArtifactSO))]
public class ArtifactSOEditor : Editor
{
    private ArtifactSO artifact;

    private void OnEnable()
    {
        artifact = (ArtifactSO)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Texture2D texture = AssetPreview.GetAssetPreview(artifact.artifactSprite);
        GUILayout.Label("", GUILayout.Height(80), GUILayout.Width(80));
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
    }
}
