using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomizationSOPart
{
    public Transform customizationPart;
    public Sprite customizationPartSprite;
}
[CreateAssetMenu()]
public class CustomizationSO : ScriptableObject
{
    public CustomizationContent content;
    public CustomizationPart part;
    public List<CustomizationSOPart> customizationParts = new List<CustomizationSOPart>();
}
