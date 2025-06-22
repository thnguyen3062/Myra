using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDissolveHandle : MonoBehaviour
{
    [SerializeField] private BoardCard card;
    [SerializeField] private Material[] dissolveMaterial;
    private Material[] lastCardMaterial;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private bool isDissolve;
    private float currentDissolve;

    private void Start()
    {
        //card.onDissolve += OnDissolve;
        skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        lastCardMaterial = skinnedMeshRenderer.materials;
    }

    private void OnDissolve()
    {
        Material[] mat = skinnedMeshRenderer.materials;
        for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
        {
            mat[i] = dissolveMaterial[i];
            if (mat[i].HasProperty("_print_img"))
                mat[i].SetTexture("_print_img", mat[i].GetTexture("_print_img"));
        }
        skinnedMeshRenderer.materials = mat;
        isDissolve = true;
    }

    private void FixedUpdate()
    {
        if (isDissolve)
        {
            foreach(Material mat in skinnedMeshRenderer.materials)
            {
                currentDissolve += 0.01f;
                mat.SetFloat("_dissolve", currentDissolve);
            }
        }
    }

    private void OnDisable()
    {
        isDissolve = false;
        currentDissolve = 0;
        if(skinnedMeshRenderer != null)
        skinnedMeshRenderer.materials = lastCardMaterial;
    }
}
