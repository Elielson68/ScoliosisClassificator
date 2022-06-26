using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class CalculadorDeReta : MonoBehaviour
{
    public Vector3 r1, r2;
    public Material LineColor;
    public GameObject linhas;
    public TextMeshProUGUI anguloText;
    private LineRenderer auxLine;
    public StepsController steps;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            if(auxLine is null)
                CreateLine(pos);
            else
            {
                DefinePosition(pos);
                UpdateStep();
            }
        }
        else if(auxLine is not null)
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            auxLine.SetPosition(1, pos);
        }
        WriteTextAngles();
    }

    void WriteTextAngles()
    {
        if(linhas.transform.childCount > 1)
        {
            List<LineRenderer> ls = new List<LineRenderer>();
            foreach(Transform child in linhas.transform)
            {
                ls.Add(child.GetComponent<LineRenderer>());
            }
            string angulos = "Ângulos: \n";
            for(var i=0; i < ls.Count; i++)
            {
                if(i+1 < ls.Count)
                {
                    float angulo = Vector3.Angle(ls[i].GetPosition(0) - ls[i].GetPosition(1), ls[i+1].GetPosition(0) - ls[i+1].GetPosition(1));
                    angulos += $"L{i+1} e L{i+2} = {(int)angulo}°\n";
                }
            }
            
            anguloText.text = angulos;
        }
    }

    void CreateLine(Vector3 pos)
    {
        GameObject g = new();
        auxLine = g.AddComponent<LineRenderer>();
        auxLine.positionCount = 2;
        auxLine.widthMultiplier = 0.05f;
        auxLine.material = LineColor;
        r1 = pos;
        auxLine.SetPosition(0, pos);
        g.transform.SetParent(linhas.transform);
    }

    void DefinePosition(Vector3 pos)
    {
        if(r1 != Vector3.zero)
        {
            auxLine.SetPosition(1, pos);
            r1 = Vector3.zero;
            auxLine = null;
        }
    }

    void UpdateStep()
    {
        switch(linhas.transform.childCount)
        {
            case 2:
                steps.UpdateStep();
                break;
            case 3:
                steps.UpdateStep();
                break;
            case 4:
                steps.UpdateStep();
                break;
        }
    }

}
