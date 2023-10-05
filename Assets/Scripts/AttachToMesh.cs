using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This matches the cloth movement to a character, the clothing must be inside the character.
 **/
public class AttachToMesh : MonoBehaviour
{

    void Start()
    {
        int i = 0;
        Transform rootParent = transform.root.transform;
        GameObject target;
        Transform[] bodyBones = null;
        var skinnedMesh = transform.parent.GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMesh != null)
        {
            bodyBones = skinnedMesh.bones;
        }

        if (bodyBones == null)
        {
            Debug.LogError("Wrong parent body.");
            return;
        }

        GameObject Attachment;

        for (i = 0; i < transform.childCount; i++)
        {
            Attachment = transform.GetChild(i).gameObject;
            if (Attachment.GetComponent<SkinnedMeshRenderer>() != null)
            {
                Attachment.GetComponent<SkinnedMeshRenderer>().bones = bodyBones;
            }

        }
    }
}