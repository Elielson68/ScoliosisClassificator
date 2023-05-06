using System.Collections.Generic;
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
    public static System.Action OnCompleteRule;
    public static System.Action OnAllRulesDone;
    public List<ClassificationData> Classifications;
    public int CurrentRule;
    public RawImage Image;
    
    private LineController _auxLine;
    private List<float> _degrees = new();
    private BoxCollider2D _collider;
    private bool _firstPointCreated = false;
    private bool _isClickOnUIElement;
    private RawImageController _rawImageController;
    private void Start()
    {
        _rawImageController = new RawImageController();
        _collider = GetComponent<BoxCollider2D>();
        _collider.size = new Vector2(Screen.width, Screen.height);
        _rawImageController.Image = Image;

        Classifications.ForEach(c => { c.classification.CurrentRule = 0; });
        //StateController.OnChangeState += () => { CurrentRule = 0; BlockCreationLineGlobal = false; ClearLines(); };
        StateController.OnFowardButtonClick += ClearLines;
        StateController.OnFowardButtonClick += () => BlockCreationLineGlobal = false;
        StateController.OnFowardButtonClick += UpdateImageOnChangeState;
        Debug.Log($"width: {Screen.width} height: {Screen.height}");
    }

    private void Update()
    {
        if (BlockCreationLineGlobal)
            if (_auxLine is not null && IsLineCompleted is false)
                _auxLine = null;
    }

    private void UpdateImageOnChangeState()
    {
        ClassificationData cls = Classifications[(int) StateController.CurrentState];
        Texture2D text = _rawImageController.GetTexture2D(cls);
        _rawImageController.UpdateTexturePanel(text);
    }

    private void ClearLines()
    {
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
        Vector3 point1, point2 = point1 =  Vector3.zero;
        for (var i = 0; i < ls.Count; i++)
        {
            int ii = i + 1;
            if(i == ls.Count - 1)
            {
                point1 = ls[i].GetPosition(0);
                point2 = ls[i].GetPosition(1);
            }
            if (ii < ls.Count)
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
        UpdateRule(point1, point2);
    }
    

    private void UpdateRule(Vector3 point1, Vector3 point2)
    {
        Classifications.ForEach(cd =>
        {
            if(cd.State == StateController.CurrentState)
            {
                if(cd.classification.CurrentRule > cd.classification.Rules.Count - 1 && cd.State == States.Lateral)
                {
                    OnAllRulesDone?.Invoke();
                    BlockCreationLineGlobal = true;
                    Debug.LogWarning($"Todas as regras foram feitas!");
                    return;
                }
                Classification.Rule rule = cd.classification.Rules[cd.classification.CurrentRule];
                cd.classification.Lines.Add(new Line(point1, point2));
                if(rule.TotalLines == lineParent.transform.childCount)
                {
                    cd.classification.CurrentRule++;
                    if(cd.classification.CurrentRule > cd.classification.Rules.Count - 1)
                    {
                        StateControll.ShowFowardButton();
                        BlockCreationLineGlobal = true;
                        return;
                    }
                    StateControll.UpdateState();
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
}
