using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class ClassificationFolder
{
    public static string SaveDataFolder = Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.streamingAssetsPath;
    public static string StateName = "";
    public static string PathFolderFile => $"{SaveDataFolder}/Reports/{"{0}"}";
    public static string FileName => $"{PathFolderFile}/{StateName}.json";
    public static string FolderName = "";
    public static string DefinedFolderFilePath => string.Format(PathFolderFile, FolderName);
    public static string DefinedFilePath => string.Format(FileName, FolderName);  
    private static string[] _filesStates = {"DrawImageInfo.json", "InsetImageInfo.json", "Report.json", "SacroType.json"};
    
    public static void GenerateFolder()
    {
        FolderName = DateTime.Now.ToString("dd-MM-yyyy_HH.mm");
        if(Directory.Exists(ClassificationFolder.DefinedFolderFilePath) is false)
        {
            Directory.CreateDirectory(ClassificationFolder.DefinedFolderFilePath);
        }
    }

    public static void ConfigureFoldersOnAndroid()
    {
        if(Application.platform != RuntimePlatform.Android || Directory.Exists(Application.persistentDataPath+"/StatesFiles/")) return;
        
        string path = $"{Application.streamingAssetsPath}/StatesFiles/";
        Directory.CreateDirectory(Application.persistentDataPath+"/StatesFiles/");
        foreach(var file in _filesStates)
        {
            UnityWebRequest www = UnityWebRequest.Get(path+file);
            www.SendWebRequest();
            while (!www.isDone) ;
            File.WriteAllText(Application.persistentDataPath+"/StatesFiles/"+file, www.downloadHandler.text);
            Debug.Log($"Carregou o JSON!: \n{www.downloadHandler.text}");
        }
        Debug.Log("Moveu os arquivos!");
        
    }
}
