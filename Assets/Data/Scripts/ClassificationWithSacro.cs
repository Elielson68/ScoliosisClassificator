using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "ClassificationWithSacro", menuName = "EscolioseClassificator/ClassificationWithSacro", order = 0)]
public class ClassificationWithSacro : Classification
{
    public string ClassificationCode;
    public List<Line> SubLines;
    public Sacro Sacro;

    public override void ExportJson() 
    {
        base.ExportJson();
    }

    public override void ImportJson(string folderName)
    {
        ClassificationFolder.StateName = State.ToString();
        string file = File.ReadAllText(string.Format(ClassificationFolder.FileName, folderName));
        ClassificationWithSacro obj = JsonConvert.DeserializeObject<ClassificationWithSacro>(file);
        State = obj.State;
        Lines = obj.Lines;
        Image = obj.Image;
        SubLines = obj.SubLines;
        Sacro = obj.Sacro;
        PositionImage = obj.PositionImage;
        ScaleImage = obj.ScaleImage;
        ClassificationCode = obj.ClassificationCode;
    }

    [ContextMenu("Reset Values")]
    public override void Reset()
    {
        base.Reset();
        SubLines = new List<Line>();
    }
}