using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateInfo
{
    public string title;
    public List<string> contents;
}

[System.Serializable]
public struct Line
{
    public Vector3 Point1;
    public Vector3 Point2;

    public Line(Vector3 p1, Vector3 p2)
    {
        Point1 = p1;
        Point2 = p2;
    }
}

[System.Serializable]
public struct EnterNewFileExecute
{
    public string FileName;
    public UnityEvent Execute;
}