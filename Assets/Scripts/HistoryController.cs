using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class HistoryController : MonoBehaviour
{
    private List<ClassificationData> _classifications;

    private const string HistoryContent = "history-content";
    private const string BackHistoryButton = "back-history";
    private const string BackInitialButton = "back-initial";
    private const string ReportTitleStyle = "report-title-label";

    private ReportController _reportController;
    private UIDocument _document;
    private VisualElement HistoryContentVE;
    private Button _backToHistoryButton;
    private Button _backToInitialButton;

    public void StartHistory()
    {
        _classifications = FindObjectOfType<Classifications>()[0];
        _document = FindObjectOfType<UIDocument>();
        _reportController = GetComponent<ReportController>();
        HistoryContentVE = _document.rootVisualElement.Q(HistoryContent);
        _backToHistoryButton = _document.rootVisualElement.Q<Button>(BackHistoryButton);
        _backToInitialButton = _document.rootVisualElement.Q<Button>(BackInitialButton);

        _backToHistoryButton.RegisterCallback<ClickEvent>(BackButtonAction);
        _backToInitialButton.RegisterCallback<ClickEvent>(evt =>
        {
            FindObjectOfType<OptionController>().ChangeScreen(Screens.Initial);
        });

        foreach(string dir in Directory.GetDirectories(Application.streamingAssetsPath+"/Reports/"))
        {
            Button reportFolder = new Button();
            reportFolder.text = (new DirectoryInfo(dir)).Name;
            HistoryContentVE.Add(reportFolder);
            reportFolder.focusable = true;
            reportFolder.RegisterCallback<ClickEvent>(SetReportData);
            reportFolder.AddToClassList(ReportTitleStyle);
        }
    }

    private void SetReportData(ClickEvent evt)
    {
        Button label = evt.target as Button;
        foreach(var cls in _classifications)
        {
            cls.classification.ImportJson(label.text);
        }
        _reportController.ShowReportButtons(exportJsonOnShowButtons: false);
        _reportController.ShowImageReport();
        HistoryContentVE.style.display = DisplayStyle.None; 
    }

    private void BackButtonAction(ClickEvent evt)
    {
        HistoryContentVE.style.display = DisplayStyle.Flex;
        _reportController.HideReportScreen();
    }
}
