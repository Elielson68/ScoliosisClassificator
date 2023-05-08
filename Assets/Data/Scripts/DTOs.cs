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

public enum Screens
{
    Initial,
    States,
    History
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