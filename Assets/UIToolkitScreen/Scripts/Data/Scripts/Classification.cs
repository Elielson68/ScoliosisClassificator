using System;
using System.Collections.Generic;
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

    public void ExportJson() { }


    public void ImportJson() { }

}