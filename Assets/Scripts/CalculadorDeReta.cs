using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class CalculadorDeReta : MonoBehaviour
{
    public GameObject linhas;
    public TextMeshProUGUI anguloText;
    private LineController auxLine;
    public GameObject Line;
    public static bool IsLineCompleted;
    private List<float> _degrees = new();
    private Dictionary<string, float> _stepData = new();
    public ProgressController States;

    [System.Serializable]
    public struct PairLinesForStep
    {
        public string StepName;
        public int PairLines;
    }
    [System.Serializable]
    public struct PairLineStates
    {
        public string StateName;
        public List<PairLinesForStep> pairs;
    }

    public List<PairLineStates> PairsLinesOfStatesForSteps;
    private BoxCollider2D colider;
    private bool firstPointCreated = false;

    private void Start() {
        colider = GetComponent<BoxCollider2D>();
        colider.size = new Vector2(Screen.width, Screen.height);
        ProgressController.OnInitChangeState += CreateData;
        ProgressController.OnInitChangeState += state => {
            foreach(Transform child in linhas.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            _degrees.Clear();
        };
    }

    void Update()
    {        
        WriteTextAngles();
    }

    private void OnMouseDown() {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        if(auxLine is null)
        {
            CreateLine(pos);
            IsLineCompleted = false;
        }  
        else
        {
            CreateDegrees();
        }
    }
    private void OnMouseDrag() {
        if(firstPointCreated)
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            auxLine.Point2.transform.position = pos;
        }
    }

    private void OnMouseUp() {
        if(firstPointCreated)
        {
            auxLine = null;
            UpdateStep();
            IsLineCompleted = true;
            firstPointCreated = false;
        }
        if(auxLine is not null)
            firstPointCreated = true;
    }

    void WriteTextAngles()
    {
        if(_stepData.Count > 0)
        {
            string angulos = "Ângulos: \n";
            foreach(var value in _stepData)
            {
                angulos += $"{value.Key}: {(int)value.Value}°\n";
            }
            anguloText.text = angulos;
        }
    }

    void CreateDegrees()
    {
        List<LineRenderer> ls = new List<LineRenderer>();
        _degrees.Clear();
        foreach(Transform child in linhas.transform)
        {
            ls.Add(child.GetComponent<LineRenderer>());
        }
        for(var i=0; i < ls.Count; i++)
        {
            if(i+1 < ls.Count)
            {
                float angulo = Vector3.Angle(ls[i].GetPosition(0) - ls[i].GetPosition(1), ls[i+1].GetPosition(0) - ls[i+1].GetPosition(1));
                _degrees.Add(angulo);
            }
        }
    }
    void CreateLine(Vector3 pos)
    {
        auxLine = Instantiate(Line, Vector3.zero, Quaternion.identity, linhas.transform).GetComponent<LineController>();
        auxLine.Point1.transform.position = pos;
        auxLine.Point2.transform.position = pos;
    }

    void UpdateStep()
    {
        
        foreach(var pair in PairsLinesOfStatesForSteps)
        {
            if(States.EstadoAtual.ToString() == pair.StateName)
            {
                foreach(var pairStep in pair.pairs)
                {
                    bool stepDataNotContainsKey = !_stepData.ContainsKey(pairStep.StepName);
                    bool pairsLinesEqualLinesChilds = pairStep.PairLines==linhas.transform.childCount;
                    bool degreeIsNotEmpty = _degrees.Count > 0;
                    if(stepDataNotContainsKey && pairsLinesEqualLinesChilds && degreeIsNotEmpty)
                    {
                        int index = States.StepsForStateDic[States.EstadoAtual.ToString()].IndexActualStep;
                        _stepData.Add(pairStep.StepName, _degrees[index]);
                        States.UpdateStepForActualState();
                    } 
                }
            }
        }
    }

    void CreateData(string name)
    {
        var data = DataController.CreateDegreeData(_stepData);
        DataController.CreateDegreeDataAsset(data, name);
    }

}
