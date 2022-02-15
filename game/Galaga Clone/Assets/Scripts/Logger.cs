﻿using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger : MonoBehaviour
{
    private string fileName;
    private string filePath;

    void OnEnable()
    {
        int i = 1;

        top:
        fileName = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + i + ".txt";
        string directory = Application.dataPath + "/logs/";
        filePath = directory + fileName;

        DirectoryInfo directoryInfo = Directory.CreateDirectory(directory);

        if (File.Exists(@filePath))
        {
            i++;
            goto top;
        }

        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string message, string stackTrace, LogType logType)
    {
        TextWriter tw = new StreamWriter(filePath, true);
        string lineStart = "[" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "] [" + logType.ToString() + "]: ";

        if (logType != LogType.Log)
        {
            tw.WriteLine(lineStart + message + "\n" + stackTrace);
        }
        else
        {
            tw.WriteLine(lineStart + message);
        }
        tw.Close();
    }
}
