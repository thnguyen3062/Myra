using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GIKCore.Bundle
{
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(Image))]
    public class BundleLoader : MonoBehaviour
    {
        // Fields
        [SerializeField] private Image m_ImageTarget;
        [SerializeField] private string m_AssetName;
        [SerializeField] [BundleSelector] private string m_AssetBundle = "";
        [SerializeField] private bool m_CheckSpriteExists = true;
        [SerializeField] private bool m_SetNativeSize = false;        

        // Methods
        private void DoLoad()
        {
            if (m_ImageTarget == null) 
                m_ImageTarget = GetComponent<Image>();

            if (string.IsNullOrEmpty(m_AssetBundle) || string.IsNullOrEmpty(m_AssetName) || m_ImageTarget == null) 
                return;
            if (m_CheckSpriteExists && m_ImageTarget.sprite != null && m_ImageTarget.sprite.name.Equals(m_AssetName)) 
                return;

            Sprite ret = BundleHandler.LoadSprite(m_AssetBundle, m_AssetName);
            if (ret != null)
            {
                m_ImageTarget.sprite = ret;
                if (m_SetNativeSize)
                    m_ImageTarget.SetNativeSize();
            }
        }

        // System
        void Awake()
        {
            DoLoad();
        }

        // Use this for initialization
        //void Start() { }

#if UNITY_EDITOR
        private Sprite lastSprite = null;

        private void Validate()
        {
            if (m_ImageTarget != null && m_ImageTarget.sprite != null)
            {
                m_AssetName = m_ImageTarget.sprite.name;
                m_AssetBundle = "";

                string targetPath = UnityEditor.AssetDatabase.GetAssetPath(m_ImageTarget.sprite);
                string[] allAssetBundleNames = UnityEditor.AssetDatabase.GetAllAssetBundleNames();
                foreach (string assetBundleName in allAssetBundleNames)
                {
                    string[] assetPathsFromAssetBundle = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
                    foreach (string assetPath in assetPathsFromAssetBundle)
                    {
                        if (assetPath.Equals(targetPath))
                        {
                            m_AssetBundle = assetBundleName;
                            return;
                        }
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (m_ImageTarget != null && m_ImageTarget.sprite != null)
            {
                if (m_ImageTarget.sprite != lastSprite)
                {
                    lastSprite = m_ImageTarget.sprite;
                    Validate();
                }
            }
        }
        void OnValidate()
        {
            Validate();
        }
#endif
    }
}