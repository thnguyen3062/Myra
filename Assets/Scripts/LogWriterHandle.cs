using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class LogWriterHandle
{
    private static bool canLog =true;

    public static void WriteLog(string content)
    {
        if (!canLog) 
            return;

        Debug.Log(content);
    }

    public static void WriteLog(long content)
    {
        if (!canLog)
            return;

        Debug.Log(content);
    }
    public static void WriteLogError(string content)
    {
        if (!canLog)
            return;

        Debug.LogError(content);
    }

    public static void WriteLogError(long content)
    {
        if (!canLog)
            return;

        Debug.LogError(content);
    }
}
