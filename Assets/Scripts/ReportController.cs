using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ReportController : MonoBehaviour
{
    private const string BackButton = "back-history";
    private const string BackToInitialButton = "back-initial";
    public UIDocument document;
    private List<ClassificationData> _classifications;
    public VisualElement ReportButtonsContent;
    public Dictionary<States, RadioButton> radioButtons = new Dictionary<States, RadioButton>();
    public GameObject LineParent;
    public GameObject LinePrefab;
    public UnityEngine.UI.RawImage ReportImage;
    private ImageStateController imgStateController;
    private Button _backButton;
    private Button _backToInitialButton;
    private void Start() {
        
    }

    public void StartReport()
    {
        _classifications = FindObjectOfType<Classifications>()[0];
        imgStateController = FindObjectOfType<ImageStateController>();
        ReportButtonsContent = document.rootVisualElement.Q("Report");
        _backButton = ReportButtonsContent.Q<Button>(BackButton);
        _backToInitialButton = document.rootVisualElement.Q<Button>(BackToInitialButton);

        if(radioButtons.Count == 0)
        {
            radioButtons.Add(States.Front, ReportButtonsContent.Q<RadioButton>("Frontal"));
            radioButtons.Add(States.LeftInclination, ReportButtonsContent.Q<RadioButton>("LeftInclination"));
            radioButtons.Add(States.RightInclination, ReportButtonsContent.Q<RadioButton>("RightInclination"));
            radioButtons.Add(States.Lateral, ReportButtonsContent.Q<RadioButton>("Lateral"));
        }
        
        imgStateController.SetStateImage(ReportImage);

        _backToInitialButton.RegisterCallback<ClickEvent>(evt =>
        {
            FindObjectOfType<OptionController>().ChangeScreen(Screens.Initial);
        });
    }

    public void ShowReportButtons(bool exportJsonOnShowButtons = true)
    {
        ReportButtonsContent.style.display = DisplayStyle.Flex;
        
        foreach(var classification in _classifications)
        {
            radioButtons[classification.State].style.backgroundImage = new StyleBackground(imgStateController.GetStateImage(classification.State));
            radioButtons[classification.State].RegisterCallback<ClickEvent>(UpdateTexturePanel);
            radioButtons[classification.State].RegisterCallback<ClickEvent, List<Line>>(DrawLine, classification.classification.Lines);
            
            
            if(exportJsonOnShowButtons)
            {
                classification.classification.ExportJson();
            }
        }
        imgStateController.UpdateImageToState(States.Front);
    }

    public void DisplayBackButton(bool show)
    {
        _backButton.style.display = show ? DisplayStyle.Flex:DisplayStyle.None;
        _backToInitialButton.style.display = show is false ? DisplayStyle.Flex:DisplayStyle.None;
    }

    public void ShowImageReport()
    {
        ReportImage.gameObject.SetActive(true);
    }

    public void HideReportScreen()
    {
        ReportButtonsContent.style.display = DisplayStyle.None;
        ReportImage.gameObject.SetActive(false);
        ClearLines();
    }

    public void UpdateTexturePanel(ClickEvent evt)
    {
        RadioButton button = evt.currentTarget as RadioButton;
        imgStateController.UpdateImageToState(button.style.backgroundImage.value.texture);
    }

    public void DrawLine(ClickEvent evt, List<Line> lines)
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
