using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ReportController : MonoBehaviour
{
    public UIDocument document;
    public List<ClassificationData> Classifications;
    public VisualElement ReportButtonsContent;
    public Dictionary<States, RadioButton> radioButtons = new Dictionary<States, RadioButton>();
    public GameObject LineParent;
    public GameObject LinePrefab;
    public bool IsReportState {get; set;}
    public UnityEngine.UI.RawImage ReportImage;
    private void Start()
    {
        ReportButtonsContent = document.rootVisualElement.Q("report-content");
        radioButtons.Add(States.Front, ReportButtonsContent.Q<RadioButton>("Frontal"));
        radioButtons.Add(States.LeftInclination, ReportButtonsContent.Q<RadioButton>("LeftInclination"));
        radioButtons.Add(States.RightInclination, ReportButtonsContent.Q<RadioButton>("RightInclination"));
        radioButtons.Add(States.Lateral, ReportButtonsContent.Q<RadioButton>("Lateral"));
    }

    public void ShowReportButtons()
    {
        ReportButtonsContent.style.display = DisplayStyle.Flex;
        foreach(var classification in Classifications)
        {
            radioButtons[classification.State].style.backgroundImage = new StyleBackground(GetTexture2D(classification));
            radioButtons[classification.State].RegisterCallback<ClickEvent>(UpdateTexturePanel);
            radioButtons[classification.State].RegisterCallback<ClickEvent, List<Line>>(DrawLine, classification.classification.Lines);
            radioButtons[classification.State].SendEvent(new ClickEvent());
            classification.classification.ExportJson();
        }
    }

    public Texture2D GetTexture2D(ClassificationData cls)
    {
        Texture2D text = new Texture2D(2, 2);
        text.LoadImage(cls.classification.Image);
        text.Apply();
        return text;
    }

    public void UpdateTexturePanel(ClickEvent evt)
    {
        RadioButton button = evt.currentTarget as RadioButton;
        ReportImage.texture = button.style.backgroundImage.value.texture;
        ReportImage.material.mainTexture = button.style.backgroundImage.value.texture;
        ReportImage.gameObject.SetActive(false);
        ReportImage.gameObject.SetActive(true);
    }

    public void DrawLine(ClickEvent evt, List<Line> lines)
    {
        foreach(Transform child in LineParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        foreach(Line line in lines)
        {
            DrawLine(line.Point1, line.Point2);
        }
        PointController.DisableMove = true;
    }

    public void DrawLine(Vector3 position1, Vector3 position2)
    {
        var _auxLine = Instantiate(LinePrefab, Vector3.back, Quaternion.identity, LineParent.transform).GetComponent<LineController>();
        _auxLine.transform.localPosition = Vector3.back;
        _auxLine.Point1.transform.position = position1;
        _auxLine.Point2.transform.position = position2;
    }
}
