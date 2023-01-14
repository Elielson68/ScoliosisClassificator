using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Classification", menuName = "EscolioseClassificator/Classification", order = 0)]
public class Classification : ScriptableObject
{

    [System.Serializable]
    public struct Line
    {
        public Vector3 Point1;
        public Vector3 Point2;
    }

    [Serializable]
    public struct Rule
    {
        public string Name;
        public int[] Lines;
    }

    public List<Line> Lines;
    public Texture Image;


    public List<Rule> Rules;

    public void SetImage(Texture image)
    {
        Image = image;
    }

    public void ExportJson() { }


    public void ImportJson() { }

}