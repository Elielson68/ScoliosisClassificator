using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class HistoryController : MonoBehaviour
{
    public VisualTreeAsset ItemStory;
    public VisualTreeAsset EditNamePopup;
    public VisualTreeAsset DeleteItemPopup;
    private List<ClassificationData> _classifications;

    private const string HistoryContent = "history-content";
    private const string BackInitialButton = "back-initial";

    private ReportController _reportController;
    private UIDocument _document;
    private VisualElement HistoryContentVE;
    private Button _backToInitialButton;
    private TextField _searchReport;
    private void Start()
    {
        
    }

    public void StartHistory()
    {
        _classifications = FindObjectOfType<Classifications>()[0];
        _document = FindObjectOfType<UIDocument>();
        _reportController = GetComponent<ReportController>();
        HistoryContentVE = _document.rootVisualElement.Q(HistoryContent);
        _backToInitialButton = _document.rootVisualElement.Q<Button>(BackInitialButton);
        _searchReport = _document.rootVisualElement.Q<TextField>("search-report");

        _backToInitialButton.RegisterCallback<ClickEvent>(evt =>
        {
            FindObjectOfType<OptionController>().ChangeScreen(Screens.Initial);
        });

        _searchReport.RegisterValueChangedCallback(FindReport);

        _reportController.SetBackToButton("BACK", Screens.History);
        CreateStoryItens();
    }

    private void CreateStoryItens(string find = "")
    {
        int counter = 1;
        foreach(string name in ClassificatorController.CodeClassifications.Keys)
        {
            string code = ClassificatorController.CodeClassifications[name];

            if(string.IsNullOrEmpty(find) is false && name.ToLower().Contains(find.ToLower()) is false && code.ToLower().Contains(find.ToLower()) is false) continue;

            VisualElement baseVE = ItemStory.Instantiate();
            

            HistoryContentVE.Add(baseVE);

            Label reportFolder = baseVE.Q<Label>("item");
            reportFolder.text = name;
            reportFolder.RegisterCallback<ClickEvent>(SetReportData);

            baseVE.Q<Label>("classification").text = code;
            baseVE.Q<Label>("number").text = counter.ToString("00");

            ConfigureEditButton(baseVE, reportFolder);
            ConfigureDeleteButton(baseVE, reportFolder);
            counter++;
        }
    }

    private void FindReport(ChangeEvent<string> evt)
    {
        HistoryContentVE.Clear();
        CreateStoryItens(evt.newValue);
    }

    private void ConfigureEditButton(VisualElement baseVE, Label currentLabel)
    {
        Button edit = baseVE.Q<Button>("edit");
        edit.RegisterCallback<ClickEvent>(evt => {
            Debug.Log("Clicou no Edit");
            VisualElement popup = EditNamePopup.Instantiate().Q("root");
            _document.rootVisualElement.Add(popup);
            ConfigureEditPopup(popup, currentLabel);
        });
    }

    private void ConfigureDeleteButton(VisualElement baseVE, Label currentLabel)
    {
        Button delete = baseVE.Q<Button>("delete");
        delete.RegisterCallback<ClickEvent>(evt => {
            Debug.Log("Clicou no Delete");
            VisualElement popup = DeleteItemPopup.Instantiate().Q("root");
            _document.rootVisualElement.Add(popup);
            ConfigureDeletePopup(popup, currentLabel);
        });
    }

    private void ConfigureEditPopup(VisualElement popup, Label oldNameLabel)
    {
        TextField name = popup.Q<TextField>("name");
        name.SetValueWithoutNotify(oldNameLabel.text);
        Button cancel = popup.Q<Button>("cancel");
        cancel.RegisterCallback<ClickEvent>(evt => 
        {
            Debug.Log("Cancelou a renomeação");
            _document.rootVisualElement.Remove(popup);
        });

        Button save = popup.Q<Button>("save");
        save.RegisterCallback<ClickEvent>(evt =>
        {  
            Debug.Log($"Salvou o novo nome! {name.value}");
            string code = ClassificatorController.CodeClassifications[oldNameLabel.text];
            ClassificatorController.CodeClassifications.Remove(oldNameLabel.text);
            ClassificatorController.CodeClassifications.Add(name.value, code);
            _document.rootVisualElement.Remove(popup);
            ChangeNameItem(oldNameLabel.text, name.value);
            oldNameLabel.text = name.value;
        });
    }

    private void ConfigureDeletePopup(VisualElement popup, Label nameLabel)
    {
        Button cancel = popup.Q<Button>("cancel");
        cancel.RegisterCallback<ClickEvent>(evt => 
        {
            Debug.Log("Cancelou a renomeação");
            _document.rootVisualElement.Remove(popup);
        });

        Button save = popup.Q<Button>("save");
        save.RegisterCallback<ClickEvent>(evt =>
        {  
            Debug.Log($"Vai tentar deletar o {nameLabel.text}");
            ClassificatorController.CodeClassifications.Remove(nameLabel.text);
            DeleteItem(nameLabel.text);
            _document.rootVisualElement.Remove(popup);
        });
    }

    private void ChangeNameItem(string oldName, string newName)
    {
        try
        {
            Directory.Move(string.Format(ClassificationFolder.PathFolderFile, oldName), string.Format(ClassificationFolder.PathFolderFile, newName));
        }
        catch(System.Exception e)
        {
            Debug.Log($"Houve um erro ao tentar renomear! Message: {e.Message}\n{e.StackTrace}");
        }
    }

    private void DeleteItem(string name)
    {
        try
        {
            Directory.Delete(string.Format(ClassificationFolder.PathFolderFile, name), true);
            HistoryContentVE.Clear();
            CreateStoryItens();
        }
        catch(System.Exception e)
        {
            Debug.Log($"Houve um erro ao tentar deletar! Message: {e.Message}\n{e.StackTrace}");
        }
    }

    private void SetReportData(ClickEvent evt)
    {
        Label label = evt.target as Label;
        foreach(var cls in _classifications)
        {
            cls.classification.ImportJson(label.text);
        }
        _reportController.FolderClassification = label.text;
        FindObjectOfType<OptionController>().ChangeScreen(Screens.Report);
    }
}
