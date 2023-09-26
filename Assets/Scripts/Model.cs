using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ModelPartInitializer
{
    public Model.ModelPart part;
    public Transform modelPart;
}
public class Model : MonoBehaviour
{
    [SerializeField] private List<ModelPartInitializer> modelPartsList = new List<ModelPartInitializer>();
    public enum ModelPart
    {
        Top, Bottom, Head, Eye, Hair, Beard, Ear
    }
    public Transform GetModelPart(ModelPart part)
    {
        foreach (var mp in modelPartsList)
        {
            if (mp.part == part)
            {
                return mp.modelPart;
            }
        }
        return null;
    }

}
