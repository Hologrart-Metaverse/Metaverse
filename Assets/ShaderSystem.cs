using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShaderSystem : MonoBehaviour
{
    public static ShaderSystem Instance { get; private set; }
    public delegate void Callback();
    public Callback callback;
    private WaitForSecondsRealtime wfsRealtime;
    private PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        Instance = this;
        wfsRealtime = new WaitForSecondsRealtime(.01f);
    }

    public void ChangeMaterialsFloatProperty(Transform materialsParent, string variableName, float variableValue, Callback callback = default)
    {
        ShaderSystemArgs args = new ShaderSystemArgs();
        foreach (var renderer in materialsParent.GetComponentsInChildren<Renderer>())
        {
            if (renderer.material.HasFloat(variableName))
            {
                args.materials.Add(renderer.material);
            }
        }
        
        if (args.materials.Count < 1) { return; }

        args.variableName = variableName;
        args.variableCurrentValue = args.materials[0].GetFloat(variableName);
        args.variableTargetValue = variableValue;
        StartCoroutine(ChangeValueContinuously(args, callback));
        //string s = JsonHelper<ShaderSystemArgs>.Serialize(args, new JsonSerializerSettings
        //{
        //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //});
        //Debug.Log(s.Length);
        //Debug.Log(s);
        //PV.RPC(nameof(ChangeValueRPC), RpcTarget.Others, s);
    }
    private IEnumerator ChangeValueContinuously(ShaderSystemArgs args, Callback callback = default)
    {
        Debug.Log("Baslangýç: " + args.variableCurrentValue + " Bitiþ: " + args.variableTargetValue);
        while ((float)Math.Round(args.variableCurrentValue, 1) != (float)Math.Round(args.variableTargetValue, 1))
        {
            args.variableCurrentValue = Mathf.Lerp(args.variableCurrentValue, args.variableTargetValue, Time.deltaTime * 4);
            foreach (var mat in args.materials)
            {
                mat.SetFloat(args.variableName, args.variableCurrentValue);
            }
            yield return wfsRealtime;
        }
        callback?.Invoke();
    }

}
[Serializable]
public class ShaderSystemArgs
{
    public List<Material> materials = new();
    public string variableName;
    public float variableCurrentValue;
    public float variableTargetValue;
}
