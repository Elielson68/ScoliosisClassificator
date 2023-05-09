using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class DrawLinesController : MonoBehaviour
{
    public StateController StateControll;
    public GameObject lineParent;
    public GameObject Line;
    public static bool IsLineCompleted;
    public static bool BlockCreationLineGlobal { get; set; }
    public RawImage Image;
    
    private LineController _auxLine;
    private BoxCollider2D _collider;
    private bool _firstPointCreated = false;
    private bool _isClickOnUIElement;
    private List<ClassificationData> _classifications;
    private ImageStateController _imgStateController;
    private LineRenderer _lastPointCreated;
    private Dictionary<LineRenderer, LinePair> _lineDegrees = new Dictionary<LineRenderer, LinePair>();

    private void OnEnable()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.size = new Vector2(Screen.width, Screen.height);
        _classifications = FindObjectOfType<Classifications>()[0];
        _imgStateController = FindObjectOfType<ImageStateController>();

        _classifications.ForEach(c => { c.classification.CurrentRule = 0; });

        StateController.OnFowardButtonClick += () => BlockCreationLineGlobal = false;
        StateController.OnFowardButtonClick += _imgStateController.UpdateImageOnChangeState;
        
        StateController.OnBeforeUpdateState += AddLinesToStateOnFinishState;

        _imgStateController.SetStateImage(Image);
        _imgStateController.UpdateImageOnChangeState();

        PointController.OnDragPoint += UpdateDegreeExtreme;
    }

    private void OnDisable()
    {
        PointController.OnDragPoint -= UpdateDegreeExtreme;
        StateController.OnBeforeUpdateState -= AddLinesToStateOnFinishState;
        StateController.OnFowardButtonClick -= _imgStateController.UpdateImageOnChangeState;
        StateController.OnFowardButtonClick -= ClearLines;
        StateController.OnFowardButtonClick -= () => BlockCreationLineGlobal = false;
        BlockCreationLineGlobal = false;
    }

    private void Update()
    {
        if (BlockCreationLineGlobal)
            if (_auxLine is not null && IsLineCompleted is false)
                _auxLine = null;
    }

    private void ClearLines()
    {
        foreach(Transform child in lineParent.transform)
        {
            Destroy(child.gameObject);
        }
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
            UpdateRule();
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

    private void CreateLine(Vector3 pos)
    {
        _auxLine = Instantiate(Line, Vector3.back, Quaternion.identity, lineParent.transform).GetComponent<LineController>();
        _auxLine.transform.localPosition = Vector3.back;
        _auxLine.Point1.transform.position = pos;
        _auxLine.Point2.transform.position = pos;
        SaveCurrentLine(_auxLine.GetComponent<LineRenderer>());
    }

    private void SaveCurrentLine(LineRenderer actualLine)
    {
        if(_lastPointCreated == null)
        {
            var line = new LineRenderer();
            _lastPointCreated = actualLine;
            LinePair newLine = new LinePair()
            {
                ActualLine = actualLine
            };
            _lineDegrees.Add(actualLine, newLine);
        }
        else if (_lineDegrees[_lastPointCreated].DownLine is null)
        {
            LinePair aux = _lineDegrees[_lastPointCreated];
            aux.DownLine = actualLine;
            _lineDegrees[_lastPointCreated] = aux;

            LinePair newLine = new LinePair()
            {
                ActualLine = actualLine,
                UpLine = _lastPointCreated
            };
            
            _lineDegrees.Add(actualLine, newLine);
            _lastPointCreated = actualLine;
            
        }
    }

    private void UpdateDegreeExtreme(LineRenderer movedLine)
    {
        var moveLineStruct = _lineDegrees[movedLine];
        LineRenderer Up = _lineDegrees[movedLine].UpLine;
        LineRenderer Down = _lineDegrees[movedLine].DownLine;
        LineRenderer ActualMoved = _lineDegrees[movedLine].ActualLine;

        if(Up is not null)
        {
            float degree = GetDegreeBetweenLines(ActualMoved, Up);
            moveLineStruct.DegreeBetweenActualAndUp = degree;
        }
        if(Down is not null)
        {
            float degree = GetDegreeBetweenLines(ActualMoved, Down);
            moveLineStruct.DegreeBetweenActualAndDown = degree;
        }
        _lineDegrees[movedLine] = moveLineStruct;
    }

    private float GetDegreeBetweenLines(LineRenderer l1, LineRenderer l2)
    {
        var firstLinePointOne = l1.GetPosition(0);
        var firstLinePointTwo = l1.GetPosition(1);

        var secondLinePointOne = l2.GetPosition(0);
        var secondLinePointTwo = l2.GetPosition(1);

        var v1 = (firstLinePointOne - firstLinePointTwo);
        var v2 = (secondLinePointOne - secondLinePointTwo);

        var directionPointOne = Vector3.ProjectOnPlane(v1, Vector3.zero).x;
        var directionPointTwo = Vector3.ProjectOnPlane(v2, Vector3.zero).x;

        if ((directionPointOne < 0 && directionPointTwo > 0) || (directionPointOne > 0 && directionPointTwo < 0))
            v2 = secondLinePointTwo - secondLinePointOne;

        return Vector3.Angle(v1, v2);
    }

    private void UpdateRule()
    {
        Classification cd = _classifications[(int) StateController.CurrentState].classification;
        Classification.Rule rule = cd.Rules[cd.CurrentRule];
        if(rule.TotalLines == _lineDegrees.Keys.Count)
        {
            cd.CurrentRule++;
            
            if(cd.CurrentRule > cd.Rules.Count - 1)
            {
                StateControll.ShowFowardButton();
                BlockCreationLineGlobal = true;
                return;
            }
            StateControll.UpdateState();
        }
    }

    private void AddLinesToStateOnFinishState()
    {
        List<LineRenderer> lineKeys = _lineDegrees.Keys.ToList();
        Classification cd = _classifications[(int) StateController.CurrentState].classification;
        for(int i=0; i < lineKeys.Count; i++)
        {
            cd.Lines.Add(new Line(lineKeys[i].GetPosition(0), lineKeys[i].GetPosition(1)));
            foreach(Classification.Rule rule in cd.Rules)
            {
                if(i == rule.TotalLines - 1)
                {
                    
                    UpdateDegreeExtreme(lineKeys[i]);
                    cd.Degrees.Add(_lineDegrees[lineKeys[i]].DegreeBetweenActualAndUp);
                }
            }
        }
        _lineDegrees.Clear();
        ClearLines();
        CheckFinalStateAndGenerateClassification();
    }

    private void CheckFinalStateAndGenerateClassification()
    {
        if(StateController.CurrentState == States.Lateral)
        {
            ClassificatorController.Classificate(_classifications);
        }
    }
}