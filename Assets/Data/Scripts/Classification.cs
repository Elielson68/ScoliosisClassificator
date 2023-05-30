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
    public List<float> Degrees;

    public List<Rule> Rules;
    public int CurrentRule;
    public Vector3 PositionImage;
    public Vector3 ScaleImage;

    public void SetImage(byte[] image)
    {
        File.WriteAllBytes($"{ClassificationFolder.DefinedFolderFilePath}/{State.ToString()}.jpg", image);
    }
    
    public virtual void ExportJson() 
    {
        ClassificationFolder.StateName = State.ToString();
        string serialized = JsonUtility.ToJson(this, prettyPrint: true);
        File.WriteAllText(ClassificationFolder.DefinedFilePath, serialized, System.Text.Encoding.UTF8);
    }

    public virtual void ImportJson(string folderName)
    {
        ClassificationFolder.StateName = State.ToString();
        string file = File.ReadAllText(string.Format(ClassificationFolder.FileName, folderName));
        Classification obj = JsonConvert.DeserializeObject<Classification>(file);
        State = obj.State;
        Lines = obj.Lines;
        PositionImage = obj.PositionImage;
        ScaleImage = obj.ScaleImage;
    }

    [ContextMenu("Reset Values")]
    public virtual void Reset()
    {
        CurrentRule = 0;
        Lines = new List<Line>();
        Degrees = new List<float>();
        PositionImage = Vector3.zero;
        ScaleImage = Vector3.zero;
    }
}