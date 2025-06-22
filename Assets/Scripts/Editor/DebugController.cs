using pbdson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    public static DebugController instance;
    private void Awake()
    {
        instance = this;
    }

    public void LogLstVector(ListCommonVector lcv, string mess="")
    {
        if(Debug.isDebugBuild)
        {
            Debug.Log(mess + "_______________________________");
            foreach (CommonVector cv in lcv.aVector)
            {
                Debug.Log(string.Join(",", cv.aLong));
                Debug.Log(string.Join(",", cv.aString));
                Debug.Log("-------");
            }
        }
    }
    public void LogVector(CommonVector cv,string mess="")
    {
        if(Debug.isDebugBuild)
        {
            Debug.Log(mess + ": _______________________________");
            Debug.Log(string.Join(",", cv.aLong));
            Debug.Log(string.Join(",", cv.aString));
        }
        
    }
    public void Log(string mess="", string txt ="")
    {
        Debug.Log(mess + ":______________"+ txt);
        
    }

}
