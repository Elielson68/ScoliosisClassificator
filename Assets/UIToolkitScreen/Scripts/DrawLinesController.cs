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

[RequireComponent(typeof(BoxCollider2D))]
public class DrawLinesController : MonoBehaviour
{
    [System.Serializable]
    public struct ClassificationData
    {
        public States State;
        public Classification Classification;
    }
    public StateController StateControll;
    public GameObject lineParent;
    public GameObject Line;
    public static bool IsLineCompleted;
    public static bool BlockCreationLineGlobal { get; set; }
    public static System.Action OnCompleteRule;
    public static System.Action OnAllRulesDone;
    public List<ClassificationData> Classifications;
    public int CurrentRule;

    private LineController _auxLine;
    private List<float> _degrees = new();
    private BoxCollider2D _collider;
    private bool _firstPointCreated = false;
    private bool _isClickOnUIElement;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.size = new Vector2(Screen.width, Screen.height);
        Classifications.ForEach(c => c.Classification.CurrentRule = 0);
        //StateController.OnChangeState += () => { CurrentRule = 0; BlockCreationLineGlobal = false; ClearLines(); };
        StateController.OnFowardButtonClick += ClearLines;
        StateController.OnFowardButtonClick += () => BlockCreationLineGlobal = false;
        Debug.Log($"width: {Screen.width} height: {Screen.height}");
    }

    private void Update()
    {
        if (BlockCreationLineGlobal)
            if (_auxLine is not null && IsLineCompleted is false)
                _auxLine = null;
    }

    private void ClearLines()
    {
        Debug.LogWarning("Limpou as linhas");
        foreach(Transform child in lineParent.transform)
        {
            Destroy(child.gameObject);
        }
        _degrees.Clear();
    }

    private void OnDisable()
    {
        BlockCreationLineGlobal = false;
    }

    private void OnMouseDown()
    {
        if (BlockCreationLineGlobal || _isClickOnUIElement) return;
        if (_firstPointCreated)
        {
            CreatePoint();
        }
    }

    private void OnMouseDrag()
    {
        if (BlockCreationLineGlobal || _isClickOnUIElement) return;
        if (_firstPointCreated && _auxLine is not null)
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            _auxLine.Point2.transform.position = pos;
        }
    }

    private void OnMouseUp()
    {
        if (BlockCreationLineGlobal || _isClickOnUIElement) return;

        if (_firstPointCreated is false)
        {
            CreatePoint();
        }

        if (_firstPointCreated)
        {
            CreateDegrees();
            _auxLine = null;
            IsLineCompleted = true;
            _firstPointCreated = false;
        }
        if (_auxLine is not null)
            _firstPointCreated = true;
    }

    private void CreatePoint()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        if (_auxLine is null)
        {
            CreateLine(pos);
            IsLineCompleted = false;
        }
    }

    private void CreateDegrees()
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
        UpdateRule();
    }
    

    private void UpdateRule()
    {
        Classifications.ForEach(cd =>
        {
            if(cd.State == StateController.CurrentState)
            {
                if(cd.Classification.CurrentRule > cd.Classification.Rules.Count - 1 && cd.State == States.Lateral)
                {
                    OnAllRulesDone?.Invoke();
                    BlockCreationLineGlobal = true;
                    Debug.LogWarning($"Todas as regras foram feitas!");
                    return;
                }
                Classification.Rule rule = cd.Classification.Rules[cd.Classification.CurrentRule];
                if(rule.TotalLines == lineParent.transform.childCount)
                {
                    cd.Classification.Lines.Add(new Classification.Line(){ Point1 = Vector3.left, Point2 = Vector3.back  });
                    cd.Classification.CurrentRule++;
                    if(cd.Classification.CurrentRule > cd.Classification.Rules.Count - 1)
                    {
                        StateControll.ShowFowardButton();
                        BlockCreationLineGlobal = true;
                        Debug.LogWarning($"Mostrou Foward");
                        return;
                    }
                    OnCompleteRule?.Invoke();
                    Debug.LogWarning($"Passou pra próxima regra! - cd.State: {cd.State} - CurrentRule: {cd.Classification.CurrentRule} - TotalLines: {rule.TotalLines}");
                }
            }
        });
    }


    private void CreateLine(Vector3 pos)
    {
        _auxLine = Instantiate(Line, Vector3.back, Quaternion.identity, lineParent.transform).GetComponent<LineController>();
        _auxLine.transform.localPosition = Vector3.back;
        _auxLine.Point1.transform.position = pos;
        _auxLine.Point2.transform.position = pos;
    }

    public void ChangeLinesParent(bool isDrawLine)
    {
        //(Transform removeChildFrom, Transform addChild) = isDrawLine ? (imgStateController.StateImage.transform, lineParent.transform) : (lineParent.transform, imgStateController.StateImage.transform);

        // if (addChild.childCount == 0 && removeChildFrom.childCount != 0)
        // {
        //     foreach (Transform child in removeChildFrom)
        //     {
        //         child.SetParent(addChild);
        //     }
        // }
    }


}
