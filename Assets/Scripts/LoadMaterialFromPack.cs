using GIKCore.Bundle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMaterialFromPack : MonoBehaviour
{
    [SerializeField] private Renderer m_Renderer;
    [SerializeField] private string[] m_References;
    [SerializeField] private string[] m_AssetNames;
    [SerializeField] private string[] m_AssetPaths;

    private void Start()
    {
        SetAssets(m_References, m_AssetNames, m_AssetPaths);
    }

    public void SetAssets(string[] tex, string[] aName, string[] aPath)
    {
        int rendererCount = m_Renderer.materials.Length;
        for (int j = 0; j < rendererCount; j++)
        {
            int length = tex.Length;
            for (int i = 0; i < length; i++)
            {
                Texture texture = GetTexture(aName[i], aPath[i]);
                if (texture != null)
                    m_Renderer.materials[j].SetTexture(tex[i], texture);
            }
        }
    }

    private Texture GetTexture(string aName, string aPath)
    {
        Texture target = BundleHandler.main.GetTexture(aPath, aName);
        if (target == null)
            target = Resources.Load<Texture>("Pack/" + aPath + "/" + aName);
        return target;
    }
}