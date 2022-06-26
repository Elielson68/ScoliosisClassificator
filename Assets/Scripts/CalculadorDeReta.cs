using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class CalculadorDeReta : MonoBehaviour
{
    public GameObject linhas;
    public TextMeshProUGUI anguloText;
    private LineController auxLine;
    public StepsController steps;
    public GameObject Line;
    public static bool IsLineCompleted;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !PointController.IsMouseOnPoint)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            if(auxLine is null)
            {
                CreateLine(pos);
                IsLineCompleted = false;
            }
                
            else
            {
                auxLine = null;
                UpdateStep();
                IsLineCompleted = true;
            }
        }
        else if(auxLine is not null)
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            auxLine.Point2.transform.position = pos;
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
        auxLine = Instantiate(Line, Vector3.zero, Quaternion.identity, linhas.transform).GetComponent<LineController>();
        auxLine.Point1.transform.position = pos;
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
