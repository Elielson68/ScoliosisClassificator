using System.Collections.Generic;
using UnityEngine;

public class StateInfo
{
    public string title;
    public List<string> contents;
}

public class ReportInfo
{
    public string title;
    public List<string> contents;
    public object lines;
    public object data;
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