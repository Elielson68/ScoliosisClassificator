using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

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

[System.Serializable]
public enum Screens
{
    Initial,
    States,
    History,
    Report
}

[System.Serializable]
public struct ScreenAsset
{
    public VisualTreeAsset Screen;
    public UnityEvent OnOpenScreen;
}

public enum Toracica
{
    Proximal = 0,
    Principal = 1,
    Lombar = 2
}

public struct TipoCurva
{
    public int Tipo;
    public int ToracicaProximal;
    public int ToraricaPrincipal;
    public int ToracoLombar;
}

public struct LinePair
{
    public LineRenderer UpLine;
    public LineRenderer ActualLine;
    public LineRenderer DownLine;
    public Label ScreenDegreeUp;
    public Label ScreenDegreeDown;
    
    public float DegreeBetweenActualAndUp;
    public float DegreeBetweenActualAndDown;

    public LinePair(int degreeUp = -1, int degreeDown = -1)
    {
        DegreeBetweenActualAndUp = degreeUp;
        DegreeBetweenActualAndDown = degreeDown;
        UpLine = null;
        ActualLine = null;
        DownLine = null;
        ScreenDegreeDown = null;
        ScreenDegreeUp = null;
    }
} 

public struct TinyClassification
{
    public Texture2D Image;
    public List<Line> Lines;
    public Vector3 PositionImage;
    public Vector3 ScaleImage;
    public bool UseLocalPosition;
    public List<float> Degrees;
}