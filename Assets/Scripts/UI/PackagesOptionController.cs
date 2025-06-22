using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PackagesOptionController : MonoBehaviour
{
    [SerializeField] private Transform lstPackageContent;
    [SerializeField] private GameObject packagePrefab;
   
    List<PackagesBuy> pack = new List<PackagesBuy>();
    private void Start()
    {
       
    }
    public void UpdateListPackage(List<PackagesBuy> lst)
    {
        foreach(Transform child in lstPackageContent)
            Destroy(child.gameObject);
        pack.Clear();
        pack.AddRange(lst);
        foreach(PackagesBuy p in pack)
        {
            GameObject go =Instantiate(packagePrefab, lstPackageContent);
            go.GetComponent<PackageBuy>().InnitPackages(p);
        }    
    }
}
