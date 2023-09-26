using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomizeModelPosition
{
    public CustomizeModelPositionHandler.CustomizeState customizeState;
    public Vector3 position;
}
public class CustomizeModelPositionHandler : MonoBehaviour
{
    public enum CustomizeState
    {
        Menu,
        BodyCustomize,
        FaceCustomize,
    }
    [SerializeField] private List<CustomizeModelPosition> positionList = new List<CustomizeModelPosition>();
    [SerializeField] private float changingValueSmoothness;
    [SerializeField] private Transform model;
    [SerializeField] private float rotationSensitivity;
    private Vector3 targetPos;
    private float firstRotY;
    private Vector3 firstInputPosition;
    private Vector3 lastInputPosition;
    private void Awake()
    {
        targetPos = model.localPosition;
        firstRotY = 180;
        model.rotation = Quaternion.Euler(new Vector3(model.rotation.eulerAngles.x, firstRotY, model.rotation.eulerAngles.z));
    }

    public void SetPosition(CustomizeState state)
    {
        model.rotation = Quaternion.Euler(new Vector3(model.rotation.eulerAngles.x, firstRotY, model.rotation.eulerAngles.z));
        targetPos = GetPosition(state);
    }
    private Vector3 GetPosition(CustomizeState state)
    {
        foreach (CustomizeModelPosition modelPos in positionList)
        {
            if (modelPos.customizeState == state)
                return modelPos.position;
        }
        return Vector3.zero;
    }
    private void Update()
    {
        model.localPosition = Vector3.Lerp(model.localPosition, targetPos, Time.deltaTime * changingValueSmoothness);

        if (Input.GetMouseButtonDown(0) && !Mouse3D.IsPointerOverUIElement())
        {
            firstInputPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0) && firstInputPosition != Vector3.zero)
        {
            lastInputPosition = Input.mousePosition;
            CalculateRotation();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            firstInputPosition = Vector3.zero;
        }
    }

    private void CalculateRotation()
    {
        float distance = lastInputPosition.x - firstInputPosition.x;
        if (Math.Abs(distance) > rotationSensitivity)
        {
            float rotAmount = distance > 0 ? -10 : 10;
            model.Rotate(0, rotAmount, 0, Space.Self);
            firstInputPosition = lastInputPosition;
        }
    }
}
