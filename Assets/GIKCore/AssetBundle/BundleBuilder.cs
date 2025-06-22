#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace GIKCore.Bundle
{
    public class BundleBuilder
    {
        [MenuItem("Bundle/Build/All")]
        private static void BuildAssetBundlesAll()
        {
            BuildAssetBundlesAndroid();
            BuildAssetBundlesIOS();
            BuildAssetBundlesWebGL();
            BuildAssetBundlesWindows();
        }

        [MenuItem("Bundle/Build/Android")]
        private static void BuildAssetBundlesAndroid()
        {
            string path = "AssetBundles/Android";
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.Android);
            Rename(path, manifest);
            Debug.Log("Builded Android Asset Bundles...");
        }

        [MenuItem("Bundle/Build/IOS")]
        private static void BuildAssetBundlesIOS()
        {
            string path = "AssetBundles/IOS";
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.iOS);
            Rename(path, manifest);
            Debug.Log("Builded IOS Asset Bundles...");
        }

        [MenuItem("Bundle/Build/WebGL")]
        private static void BuildAssetBundlesWebGL()
        {
            string path = "AssetBundles/WebGL";
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.WebGL);
            Rename(path, manifest);
            Debug.Log("Builded WebGL Asset Bundles...");
        }

        [MenuItem("Bundle/Build/Windows")]
        private static void BuildAssetBundlesWindows()
        {
            string path = "AssetBundles/Windows";
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
            Rename(path, manifest);
            Debug.Log("Builded Windows Asset Bundles...");
        }

        private static void Rename(string folder, AssetBundleManifest manifest)
        {
            DirectoryInfo d = new DirectoryInfo(folder);
            FileInfo[] infos = d.GetFiles();

            List<string> lstNode = new List<string>();
            string[] arrayAssetBundleName = AssetDatabase.GetAllAssetBundleNames();
            foreach (FileInfo f in infos)
            {
                if (!f.Name.EndsWith(".manifest") && arrayAssetBundleName.Contains(f.Name))
                {
                    Hash128 hash = manifest.GetAssetBundleHash(f.Name);
                    string suffix = BundleHandler.SPLIT + hash.ToString();

                    lstNode.Add('"' + f.Name + suffix + '"');
                    File.Move(f.FullName, f.FullName + suffix);
                }
                //else if (f.Name.EndsWith(".manifest") || f.Name.EndsWith("Android") || f.Name.EndsWith("IOS") || f.Name.EndsWith("WebGL") || f.Name.EndsWith("Windows"))
                //{
                //    File.Delete(f.FullName);
                //}
            }

            string aJSON = "[" + string.Join(",", lstNode) + "]";
            string file = folder + "/category.txt";
            if (File.Exists(file)) File.Delete(file);
            File.WriteAllText(file, aJSON);
        }

        [MenuItem("Bundle/Clear cache")]
        static void ClearCaching()
        {
            if (Caching.ClearCache())
            {
                Debug.Log("Successfully cleaned the cache.");
            }
            else
            {
                Debug.Log("Unable to clear cache.");
            }
        }
    }
}
#endif
