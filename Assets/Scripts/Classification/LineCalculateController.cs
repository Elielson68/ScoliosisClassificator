using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static TMPro.TMP_Dropdown;
using UnityEngine.EventSystems;
public class LineCalculateController : MonoBehaviour
{
    public static bool SacroStep { get; set; }
    public TMP_Dropdown SacroOption;
    public GameObject lineParent;
    public TMP_Dropdown degreesDropDown;
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
        public UnityEvent OnStepComplete;
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
    public static bool BlockCreationLineGlobal { get; set; } = false;

    public bool isFinishedAllStepOrStates { get; set; }
    private bool isClickOnUIElement;
    private void Start()
    {
        colider = GetComponent<BoxCollider2D>();
        colider.size = new Vector2(Screen.width, Screen.height);
        Debug.Log($"width: {Screen.width} height: {Screen.height}");
        ProgressController.OnInitChangeState += state =>
        {
            foreach (Transform child in lineParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            _degrees.Clear();
            firstPointCreated = false;
            auxLine = null;
            BlockCreationLineGlobal = false;
        };
        UpdateStep();
        States.OnCompleteAllSteps.AddListener(() => isFinishedAllStepOrStates = true);
        States.OnCompleteAllStates.AddListener(() => isFinishedAllStepOrStates = true);
    }

    public void DisableBlockCreation()
    {
        BlockCreationLineGlobal = false;
    }

    private void OnEnable()
    {
        _stepData.Clear();
        _stepData = new();
        SacroStep = false;
    }
    void Update()
    {
        degreesDropDown.captionText.text = "Ângulos";
        if (BlockCreationLineGlobal)
            if (auxLine is not null && IsLineCompleted is false)
                auxLine = null;
        if (EventSystem.current.currentSelectedGameObject is not null)
            isClickOnUIElement = true;
        else
            isClickOnUIElement = false;
    }

    private void OnMouseDown()
    {

        if (BlockCreationLineGlobal || SacroStep || isFinishedAllStepOrStates || isClickOnUIElement) return;
        if (firstPointCreated)
        {
            CreatePoint();
        }

    }
    private void OnMouseDrag()
    {
        if (BlockCreationLineGlobal || SacroStep || isFinishedAllStepOrStates || isClickOnUIElement) return;
        if (firstPointCreated && auxLine is not null)
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            auxLine.Point2.transform.position = pos;
        }
    }

    private void OnMouseUp()
    {
        if (BlockCreationLineGlobal || SacroStep || isFinishedAllStepOrStates || isClickOnUIElement) return;
        if (firstPointCreated is false)
        {
            CreatePoint();
        }

        if (firstPointCreated)
        {
            CreateDegrees();
            auxLine = null;
            UpdateStep();
            IsLineCompleted = true;
            firstPointCreated = false;
        }
        if (auxLine is not null)
            firstPointCreated = true;
    }
    void CreatePoint()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        if (auxLine is null)
        {
            CreateLine(pos);
            IsLineCompleted = false;
        }
    }
    void WriteTextAngles()
    {

        degreesDropDown.ClearOptions();
        var dataState = States.PassosPorEstado.Find(state => state.StateName == States.EstadoAtual.ToString());
        List<OptionData> options = new();
        foreach (var value in dataState.data.Degrees)
        {
            options.Add(new OptionData($"{value.name}: {(int)value.degree}°\n"));
        }
        degreesDropDown.AddOptions(options);
        degreesDropDown.captionText.text = "Ângulos";
    }

    void CreateDegrees()
    {
        List<LineRenderer> ls = new List<LineRenderer>();
        _degrees.Clear();
        foreach (Transform child in lineParent.transform)
        {
            ls.Add(child.GetComponent<LineRenderer>());
        }
        for (var i = 0; i < ls.Count; i++)
        {
            if (i + 1 < ls.Count)
            {
                var firstLinePointOne = ls[i].GetPosition(0);

                var firstLinePointTwo = ls[i].GetPosition(1);

                var secondLinePointOne = ls[i + 1].GetPosition(0);

                var secondLinePointTwo = ls[i + 1].GetPosition(1);

                var v1 = (firstLinePointOne - firstLinePointTwo);
                var v2 = (secondLinePointOne - secondLinePointTwo);

                var directionPointOne = Vector3.ProjectOnPlane(v1, Vector3.zero).x;
                var directionPointTwo = Vector3.ProjectOnPlane(v2, Vector3.zero).x;

                if ((directionPointOne < 0 && directionPointTwo > 0) || (directionPointOne > 0 && directionPointTwo < 0))
                    v2 = secondLinePointTwo - secondLinePointOne;

                float angulo = Vector3.Angle(v1, v2);
                _degrees.Add(angulo);
            }
        }
    }

    void CreateLine(Vector3 pos)
    {
        auxLine = Instantiate(Line, Vector3.back, Quaternion.identity, lineParent.transform).GetComponent<LineController>();
        auxLine.transform.localPosition = Vector3.back;
        auxLine.Point1.transform.position = pos;
        auxLine.Point2.transform.position = pos;
    }

    public void UpdateStep()
    {

        foreach (var pair in PairsLinesOfStatesForSteps)
        {
            if (States.EstadoAtual.ToString() == pair.StateName)
            {
                foreach (var pairStep in pair.pairs)
                {
                    bool stepDataNotContainsKey = !_stepData.ContainsKey(pairStep.StepName);
                    bool pairsLinesEqualLinesChilds = pairStep.PairLines == lineParent.transform.childCount;
                    bool degreeIsNotEmpty = _degrees.Count > 0;
                    if (stepDataNotContainsKey && pairsLinesEqualLinesChilds && degreeIsNotEmpty)
                    {
                        _stepData.Add(pairStep.StepName, _degrees[_degrees.Count - 1]);
                        States.UpdateStepForActualState();
                        Debug.Log($"Atualizou o estado: {States.EstadoAtual.ToString()} está no passo: {States.StepsForStateDic[States.EstadoAtual.ToString()].IndexActualStep}");
                        List<DegreeData.Line> lines = new();
                        DegreeData.Line newLine = new()
                        {
                            Point1 = lineParent.transform.GetChild(lineParent.transform.childCount - 1).GetComponent<LineRenderer>().GetPosition(0),
                            Point2 = lineParent.transform.GetChild(lineParent.transform.childCount - 2).GetComponent<LineRenderer>().GetPosition(1)
                        };
                        lines.Add(newLine);
                        States.SetData(pairStep.StepName, _degrees[_degrees.Count - 1], lines);
                        pairStep.OnStepComplete?.Invoke();
                    }
                    if (pairStep.PairLines is -1 && SacroStep)
                    {
                        int index = SacroOption.value;
                        States.UpdateStepForActualState();
                        States.SetData(index);

                    }
                }
            }
        }
        WriteTextAngles();
    }

    public void UpdateDegrees()
    {
        foreach (var pair in PairsLinesOfStatesForSteps)
        {
            if (States.EstadoAtual.ToString() == pair.StateName)
            {
                foreach (var pairStep in pair.pairs)
                {
                    CreateDegrees();
                    var dataState = States.PassosPorEstado.Find(s => s.StateName == States.EstadoAtual.ToString()).data;
                    for (int i = 0; i < dataState.Degrees.Count; i++)
                    {
                        var degree = dataState.Degrees[i];
                        degree.degree = _degrees[i];
                        dataState.Degrees[i] = degree;
                    }
                }
            }
        }
        WriteTextAngles();
    }

}
