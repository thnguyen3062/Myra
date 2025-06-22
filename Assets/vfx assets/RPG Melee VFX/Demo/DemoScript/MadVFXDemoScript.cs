using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MadVFXDemoScript: MonoBehaviour
{
    string[] allArray = null;

    public int i = 0;
    private GameObject currObj;
    public Transform allParent;
    public Transform anim;
    public Transform rotationParent;
    public UnityEngine.UI.Text nameText;
    private float speed = 50; 

    public void Awake()
    {
        allArray = new string[allParent.childCount];
        for (int j = 0; j < allParent.childCount; j++) 
        {
            allArray[j] = allParent.GetChild(j).gameObject.name;
        }

        currObj = GameObject.Instantiate(allParent.transform.Find(allArray[i]).gameObject);
        currObj.transform.parent = null;
        currObj.transform.localPosition = Vector3.zero;

        nameText.text = allParent.transform.Find(allArray[i]).gameObject.name;

        IsRotation(currObj);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2)) 
        {
            var s = allArray[i];
            UnityEngine.GUIUtility.systemCopyBuffer = s;
        }
        anim.Rotate(Vector3.up * Time.deltaTime * speed);

        if (Input.GetKeyDown(KeyCode.Space))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;
#endif
        }
    }

    public void RePlay() 
    {
        if (currObj != null)
        {
            currObj.SetActive(false);
            currObj.SetActive(true);
        }
    }

    public void OnLeftBtClick() 
    {
        i--;
        if (i < 0)
        {
            i = allArray.Length - 1;
        }
        if (currObj != null)
        {
            GameObject.DestroyImmediate(currObj);
        }
        currObj = GameObject.Instantiate(allParent.transform.Find(allArray[i]).gameObject);
        currObj.transform.parent = null;
        currObj.transform.localPosition = Vector3.zero;
        nameText.text = allParent.transform.Find(allArray[i]).gameObject.name;
        IsRotation(currObj);
    }

    public void OnRightBtClick()
    {
        i++;
        if (i >= allArray.Length)
        {
            i = 0;
        }
        if (currObj != null)
        {
            GameObject.DestroyImmediate(currObj);
        }
        currObj = GameObject.Instantiate(allParent.transform.Find(allArray[i]).gameObject);
        currObj.transform.parent = null;
        currObj.transform.localPosition = Vector3.zero;
        nameText.text = allParent.transform.Find(allArray[i]).gameObject.name;
        IsRotation(currObj);
    }

    //need rotation
    public void IsRotation(GameObject obj) 
    {
        if (obj.name.IndexOf("_R_Fly") > -1) 
        {
            obj.transform.parent = rotationParent.transform;
            obj.transform.localPosition = Vector3.zero;
            try
            {
                var sp = obj.name.Replace("(Clone)", "").Split('_');
                speed = System.Convert.ToInt32(sp[sp.Length - 1]);
            }
            catch (System.Exception x) 
            {
                Debug.LogError(obj.name + x.ToString());
            }
            obj.transform.gameObject.SetActive(false);
            obj.transform.gameObject.SetActive(true);
        }
    }
}