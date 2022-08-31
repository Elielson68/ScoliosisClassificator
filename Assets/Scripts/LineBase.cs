using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LineBase : MonoBehaviour
{
    public string nameLine;
    [HideInInspector] public Transform Point1, Point2;
    
}
