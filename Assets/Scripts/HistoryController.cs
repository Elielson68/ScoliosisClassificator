using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class HistoryController : MonoBehaviour
{
    private List<ClassificationData> _classifications;

    private const string HistoryContent = "history-content";
    private const string BackButton = "back-history";
    private const string ReportTitleStyle = "report-title-label";

    private ReportController _reportController;
    private UIDocument _document;
    private VisualElement HistoryContentVE;
    private Button _backButton;
    private void Start()
    {
        _classifications = FindObjectOfType<Classifications>()[0];
        _document = FindObjectOfType<UIDocument>();
        _reportController = GetComponent<ReportController>();
        HistoryContentVE = _document.rootVisualElement.Q(HistoryContent);
        _backButton = _document.rootVisualElement.Q<Button>(BackButton);

        _backButton.RegisterCallback<ClickEvent>(BackButtonAction);

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
