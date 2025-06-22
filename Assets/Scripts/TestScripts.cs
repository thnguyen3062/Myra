using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections;

public class TestScripts : MonoBehaviour
{
    string txtlst = "";
    private void Start()
    {
        //var tmp = new List<Text>();
        //var textMeshProList = new List<TextMeshProUGUI>();
        //var textMeshList = new List<TextMeshPro>();
        //Debug.Log(SceneManager.GetSceneAt(0).name);
        //foreach (var root in SceneManager.GetSceneAt(0).GetRootGameObjects())
        //{
        //    tmp.AddRange(root.GetComponentsInChildren<Text>(true));
        //    textMeshProList.AddRange(root.GetComponentsInChildren<TextMeshProUGUI>(true));
        //    textMeshList.AddRange(root.GetComponentsInChildren<TextMeshPro>(true));
        //}
        //foreach (var c in textMeshProList)
        //    txtlst += c.text + "\n";
        //Debug.Log(txtlst);
        bool a = true;
        bool b = false;
        bool c = false;
        bool d = true;
        bool e = false;
        bool f = true ;
        int count = 0;
        if (a)
        {
            StartCoroutine(Delay("a",count));
            count++;
        }
        if (b)
        {
            StartCoroutine(Delay("b",count));
            count++;
        }
        if (c)
        {
            StartCoroutine(Delay("c",count));
            count++;
        }
        if (d)
        {
            StartCoroutine(Delay("d",count));
            count++;
        }
        if (e)
        {
            StartCoroutine(Delay("e", count));
            count++;
        }
        if (f)
        {
            StartCoroutine(Delay("f", count));
            count++;
        }

    }
     private IEnumerator Delay(string log , float count)
    {
        yield return new WaitForSeconds(count);
    }    
}
