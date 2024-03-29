using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ReportController : MonoBehaviour
{
    public UIDocument document;
    private List<ClassificationData> _classifications;
    public VisualElement ReportContentRoot;
    public GameObject LineParent;
    public GameObject LinePrefab;
    public UnityEngine.UI.RawImage ReportImage;
    private ImageStateController imgStateController;
    private Button _backButton;
    private Button _previousClassification;
    private Button _nextClassification;
    private Label _title;
    private VisualElement _classificationSelectedContainer;
    private VisualElement _dropsideArea;
    private Toggle _dropside;
    private int _currentClassificationIndex;
    private Screens _backToScreen;
    private string _backToText;
    private List<TinyClassification> _classificationsToShow = new List<TinyClassification>();

    private void Start() {
        
    }

    public void StartReport()
    {
        _currentClassificationIndex = 0;
        _classifications = FindObjectOfType<Classifications>()[0];
        imgStateController = FindObjectOfType<ImageStateController>();
        ReportContentRoot = document.rootVisualElement.Q("root");
        _title = document.rootVisualElement.Q<Label>("title");
        _nextClassification = document.rootVisualElement.Q<Button>("button-side-forward");
        _previousClassification = document.rootVisualElement.Q<Button>("button-side-back");
        _backButton = document.rootVisualElement.Q<Button>("back-button");
        _classificationSelectedContainer = document.rootVisualElement.Q("steps-area");
        _dropsideArea = document.rootVisualElement.Q("dropside-area");
        _dropside = document.rootVisualElement.Q<Toggle>("dropside");
        

        _backButton.text = _backToText;

        InitializeClassificationsToShow();

        imgStateController.SetStateImage(ReportImage);

        UpdateClassificationReport();
        UpdateTitle();

        _nextClassification.RegisterCallback<ClickEvent>(NextClassificationImage);
        _nextClassification.RegisterCallback<FocusInEvent>(OnVisualElementFocus);
        _nextClassification.RegisterCallback<FocusOutEvent>(OnVisualElementFocusOut);

        _previousClassification.RegisterCallback<ClickEvent>(PreviousClassificationImage);
        _previousClassification.RegisterCallback<FocusInEvent>(OnVisualElementFocus);
        _previousClassification.RegisterCallback<FocusOutEvent>(OnVisualElementFocusOut);


        _backButton.RegisterCallback<ClickEvent>(BackButtonAction);
        _dropside.RegisterCallback<ChangeEvent<bool>>(Dropside);
    }

    public void Dropside(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            _dropsideArea.AddToClassList("dropside-expanded");
        }
        else
        {
            _dropsideArea.RemoveFromClassList("dropside-expanded");
        }
    }

    private void OnVisualElementFocus(FocusInEvent evt)
    {
        VisualElementInteraction.IsVisualElementFocus = true;
    }

    private void OnVisualElementFocusOut(FocusOutEvent evt)
    {
        VisualElementInteraction.IsVisualElementFocus = false;
    }

    public void SetBackToButton(string text, Screens screen)
    {
        _backToScreen = screen;
        _backToText = text;
    }

    private void BackButtonAction(ClickEvent evt)
    {
        Exit();
        FindObjectOfType<OptionController>().ChangeScreen(_backToScreen);
    }

    public void Exit()
    {
        ResetReport();
        _nextClassification.UnregisterCallback<ClickEvent>(NextClassificationImage);
        _nextClassification.UnregisterCallback<FocusInEvent>(OnVisualElementFocus);
        _nextClassification.UnregisterCallback<FocusOutEvent>(OnVisualElementFocusOut);
        
        _previousClassification.UnregisterCallback<ClickEvent>(PreviousClassificationImage);
        _previousClassification.UnregisterCallback<FocusInEvent>(OnVisualElementFocus);
        _previousClassification.UnregisterCallback<FocusOutEvent>(OnVisualElementFocusOut);

        _backButton.UnregisterCallback<ClickEvent>(BackButtonAction);
        _dropside.UnregisterCallback<ChangeEvent<bool>>(Dropside);
        
        _classificationsToShow.Clear();
    }

    private void NextClassificationImage(ClickEvent evt)
    {
        _currentClassificationIndex = (_currentClassificationIndex+1) % _classificationsToShow.Count;
        UpdateClassificationReport();
    }

    private void PreviousClassificationImage(ClickEvent evt)
    {
        _currentClassificationIndex -= 1;
        if(_currentClassificationIndex < 0)
            _currentClassificationIndex = _classificationsToShow.Count - 1;
        UpdateClassificationReport();
    }

    private void InitializeClassificationsToShow()
    {
        foreach(var cls in _classifications)
        {
            if(cls.classification.SubState == SubStates.Sacro && cls.classification is ClassificationWithSacro clsSacro)
            {
                TinyClassification tinySacro = new TinyClassification()
                {
                    Image = imgStateController.GetStateImage(cls.State),
                    Lines = clsSacro.SubLines,
                    PositionImage = ImageManipulation.DefaultPositionImage,
                    ScaleImage = ImageManipulation.DefaultScaleImage,
                    UseLocalPosition = true,
                    Degrees = new List<float>()
                };
                _classificationsToShow.Add(tinySacro);
            }
            TinyClassification tinyCls = new TinyClassification()
            {
                Image = imgStateController.GetStateImage(cls.State),
                Lines = cls.classification.Lines,
                PositionImage = cls.classification.PositionImage,
                ScaleImage = cls.classification.ScaleImage,
                Degrees = cls.classification.Degrees
            };
            _classificationsToShow.Add(tinyCls);
        }
    }

    private void UpdateClassificationReport()
    {
        TinyClassification classification = _classificationsToShow[_currentClassificationIndex];
        imgStateController.UpdateImageToState(classification.Image);
        imgStateController.UpdateWidthAndHeight();
        imgStateController.UpdatePositionAndScale(classification.PositionImage, classification.ScaleImage, classification.UseLocalPosition);
        DrawLine(classification.Lines);
        foreach(var child in _classificationSelectedContainer.Children())
        {
            child.RemoveFromClassList("classification-selected");
            if(_classificationSelectedContainer.IndexOf(child) == _currentClassificationIndex)
            {
                child.AddToClassList("classification-selected");
            }
        }
        _dropsideArea.Clear();
        foreach(float degree in classification.Degrees)
        {
            Label degreeLabel = new Label(System.Math.Round(degree, 2).ToString());
            degreeLabel.AddToClassList("degree-label");
            _dropsideArea.Add(degreeLabel);
        }
        imgStateController.UpdatePosition(Vector3.zero, classification.UseLocalPosition);
    }

    private void UpdateTitle()
    {
        _title.text = ClassificatorController.CodeClassifications[ClassificationFolder.FolderName];
    }

    public void ExportClassification()
    {
        _classifications = FindObjectOfType<Classifications>()[0];
        foreach(var classification in _classifications)
        {
            classification.classification.ExportJson();
        }
    }

    public void ResetReport()
    {
        ReportImage.gameObject.SetActive(false);
        imgStateController.SetToDefaultPositionAndScale();
        ClearLines();
    }

    public void DrawLine(List<Line> lines)
    {
        ClearLines();
        foreach(Line line in lines)
        {
            DrawLine(line.Point1, line.Point2);
        }
        PointController.DisableMove = true;
    }

    public void ClearLines()
    {
        foreach(Transform child in LineParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void DrawLine(Vector3 position1, Vector3 position2)
    {
        var _auxLine = Instantiate(LinePrefab, Vector3.back, Quaternion.identity, LineParent.transform).GetComponent<LineController>();
        _auxLine.transform.localPosition = Vector3.back;
        _auxLine.Point1.transform.position = position1;
        _auxLine.Point2.transform.position = position2;
    }
}
