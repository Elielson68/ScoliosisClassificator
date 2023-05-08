using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClassificationFolder
{
    public static string StateName = "";
    public static string PathFolderFile => $"{Application.streamingAssetsPath}/Reports/{"{0}"}";
    public static string FileName => $"{PathFolderFile}/{StateName}.json";
    public static string FolderName = "";
    public static string DefinedFolderFilePath => string.Format(PathFolderFile, FolderName);
    public static string DefinedFilePath => string.Format(FileName, FolderName);  

    public static void GenerateFolderName()
    {
        FolderName = DateTime.Now.ToString("dd-MM-yyyy_HH.mm");
    }
}
