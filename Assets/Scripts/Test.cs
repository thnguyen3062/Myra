using DG.Tweening;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    List<string> alltxt = new List<string>();
    Text[] txt;
    TextMeshProUGUI[] tmpro;
    private void Start()
    {
        txt = FindObjectsOfType<Text>();
        tmpro = FindObjectsOfType<TextMeshProUGUI>();

        foreach (Text t in txt)
        {
            alltxt.Add(t.text);
        }
        foreach (TextMeshProUGUI tmp in tmpro)
        {
            Debug.Log(tmpro.Length);
            Debug.Log(tmp.gameObject.name);
            Debug.Log(tmp.text);
            alltxt.Add(tmp.text);
        }

    }
    public void AddToFile()
    {  
        alltxt.Sort();
        string dataAsJson = JsonUtility.ToJson(alltxt);
        string filePath = Application.dataPath + "HienTxt"  + ".json";
        File.WriteAllText(filePath, dataAsJson);
    }    
}
