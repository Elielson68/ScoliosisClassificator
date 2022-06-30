using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DegreeData", menuName = "EscolioseClassificator/DegreeData", order = 0)]
public class DegreeData : ScriptableObject {

    [System.Serializable]
    public struct DegreeCalculateData
    {
        public string name;
        public float degree;
        public List<GameObject> lines;
    }
    [Space]
    public List<DegreeCalculateData> Degrees = new();
}