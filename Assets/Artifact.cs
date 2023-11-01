using ActionCode.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ArtifactType
{
    Static,
    Dynamic,
}
public class Artifact : MonoBehaviour, I_UIModeChanger
{
    public ArtifactSO artifactSO;
    private Cinemachine.CinemachineVirtualCamera detailsCam;
    [SerializeField] private Transform artifactObject;
    private void Start()
    {
        detailsCam = GlobalCameraManager.Instance.GetVirtualCamera(GlobalCameraManager.CameraType.Details);
        GameInput.Instance.OnBackPressed += GameInput_OnBackPressed;
    }
    private void GameInput_OnBackPressed(object sender, System.EventArgs e)
    {
        if (ZoomSystem.Instance.isZooming && ZoomSystem.Instance.currentZoomSurface == ZoomSystem.ZoomSurface.Artifact)
            ChangeMode(UIMode.None);
    }
    public void ChangeMode(UIMode mode)
    {
        switch (mode)
        {
            case UIMode.StaticEdit:
                detailsCam.Follow = artifactObject;
                detailsCam.transform.rotation = artifactObject.rotation;
                detailsCam.enabled = true;
                Utils.SetMouseLockedState(false);
                StateHandler.Instance.SetState(State.ArtPlanet_StaticUI);
                ArtifactDetailsUI.Instance.Show(artifactSO.artifactName, artifactSO.description);
                ZoomSystem.Instance.StartZooming(ZoomSystem.ZoomMode.Static, ZoomSystem.ZoomSurface.Artifact);
                break;
            case UIMode.DynamicEdit:
                StateHandler.Instance.SetState(State.ArtPlanet_DynamicUI);
                ArtifactDetailsUI.Instance.Show(artifactSO.artifactName, artifactSO.description);
                ZoomSystem.Instance.StartZooming(ZoomSystem.ZoomMode.Dynamic, ZoomSystem.ZoomSurface.Artifact, artifactObject);
                break;
            case UIMode.None:
                if (detailsCam)
                {
                    detailsCam.enabled = false;
                    detailsCam.Follow = null;
                }
                Utils.SetMouseLockedState(true);
                StateHandler.Instance.SetState(State.None);
                ArtifactDetailsUI.Instance.Hide();
                EventSystem.current.SetSelectedGameObject(null);
                ZoomSystem.Instance.EndZooming();
                break;
        }
    }

    public void OnInteracted()
    {
        switch (artifactSO.ArtifactType)
        {
            case ArtifactType.Static:
                ChangeMode(UIMode.StaticEdit);
                break;
            case ArtifactType.Dynamic:
                ChangeMode(UIMode.DynamicEdit);
                break;
        }
    }

}
