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
        
        _backButton.text = _backToText;

        InitializeClassificationsToShow();

        imgStateController.SetStateImage(ReportImage);

        UpdateClassificationReport();
        UpdateTitle();

        _nextClassification.RegisterCallback<ClickEvent>(NextClassificationImage);
        _previousClassification.RegisterCallback<ClickEvent>(PreviousClassificationImage);
        _backButton.RegisterCallback<ClickEvent>(BackButtonAction);
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
        _previousClassification.UnregisterCallback<ClickEvent>(PreviousClassificationImage);
        _backButton.UnregisterCallback<ClickEvent>(BackButtonAction);
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
                    UseLocalPosition = true
                };
                _classificationsToShow.Add(tinySacro);
            }
            TinyClassification tinyCls = new TinyClassification()
            {
                Image = imgStateController.GetStateImage(cls.State),
                Lines = cls.classification.Lines,
                PositionImage = cls.classification.PositionImage,
                ScaleImage = cls.classification.ScaleImage
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
    }

    private void UpdateTitle()
    {
        ClassificationData classification = _classifications[_currentClassificationIndex];
        ClassificationWithSacro clsSub = classification.classification as ClassificationWithSacro;
        _title.text = clsSub.ClassificationCode;
    }

    public void ExportClassification()
    {
        ClassificationFolder.GenerateFolderName();
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
