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
    public List<Line> Lines;
    public RawImage image;


    public void ExportJson() { }


    public void ImportJson() { }

}