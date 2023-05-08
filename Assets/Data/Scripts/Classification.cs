using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "Classification", menuName = "EscolioseClassificator/Classification", order = 0)]
public class Classification : ScriptableObject
{
    public States State;
    public SubStates SubState = SubStates.None;

    [Serializable]
    public struct Rule
    {
        public string Name;
        public int TotalLines;
    }

    public List<Line> Lines;
    public byte[] Image;
    public List<float> Degrees;

    public List<Rule> Rules;
    public int CurrentRule;
    public void SetImage(byte[] image)
    {
        Image = image;
    }
    
    public virtual void ExportJson() 
    {
        ClassificationFolder.StateName = State.ToString();
        string serialized = JsonUtility.ToJson(this, prettyPrint: true);
        if(Directory.Exists(ClassificationFolder.DefinedFolderFilePath) is false)
        {
            Directory.CreateDirectory(ClassificationFolder.DefinedFolderFilePath);
        }
        File.WriteAllText(ClassificationFolder.DefinedFilePath, serialized, System.Text.Encoding.UTF8);
    }

    public virtual void ImportJson(string folderName)
    {
        ClassificationFolder.StateName = State.ToString();
        string file = File.ReadAllText(string.Format(ClassificationFolder.FileName, folderName));
        Classification obj = JsonConvert.DeserializeObject<Classification>(file);
        State = obj.State;
        Lines = obj.Lines;
        Image = obj.Image;
    }

    [ContextMenu("Reset Values")]
    public virtual void Reset()
    {
        Lines = new List<Line>();
        Image = new byte[0];
        Degrees = new List<float>();
    }
}