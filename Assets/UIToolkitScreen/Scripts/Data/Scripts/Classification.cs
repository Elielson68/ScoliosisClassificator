using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "Classification", menuName = "EscolioseClassificator/Classification", order = 0)]
public class Classification : ScriptableObject
{
    public States State;
    

    [Serializable]
    public struct Rule
    {
        public string Name;
        public int TotalLines;
    }

    public List<Line> Lines;
    public byte[] Image;


    public List<Rule> Rules;
    public int CurrentRule;
    public void SetImage(byte[] image)
    {
        Image = image;
    }

    private string PathFolderFile => $"{Application.streamingAssetsPath}/{"{0}"}";
    private string FileName => $"{PathFolderFile}/{State.ToString()}.json";
    private string FolderName => DateTime.Now.ToString("dd-MM-yyyy_HH.mm");
    private string DefinedFolderFilePath => string.Format(PathFolderFile, FolderName);
    private string DefinedFilePath => string.Format(FileName, FolderName);
    public void ExportJson() 
    {
        string serialized = JsonUtility.ToJson(this, prettyPrint: true);
        Debug.LogWarning(serialized);
        if(Directory.Exists(DefinedFolderFilePath) is false)
        {
            Directory.CreateDirectory(DefinedFolderFilePath);
        }
        File.WriteAllText(DefinedFilePath, serialized, System.Text.Encoding.UTF8);
    }

    public void ImportJson(string folderName)
    {
        string file = File.ReadAllText(string.Format(FileName, folderName));
        Classification obj = JsonConvert.DeserializeObject<Classification>(file);
        State = obj.State;
        Lines = obj.Lines;
        Image = obj.Image;
    }
}