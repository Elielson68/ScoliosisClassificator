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
    private const string BackHistoryButton = "back-history";
    private const string BackInitialButton = "back-initial";
    private const string ReportTitleStyle = "report-title-label";

    private ReportController _reportController;
    private UIDocument _document;
    private VisualElement HistoryContentVE;
    private Button _backToHistoryButton;
    private Button _backToInitialButton;

    private void Start()
    {
        
    }

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

        CreateStoryItens();
    }

    private void CreateStoryItens()
    {
        foreach(string dir in Directory.GetDirectories(ClassificationFolder.SaveDataFolder+"/Reports/"))
        {
            VisualElement baseVE = ItemStory.Instantiate();
            HistoryContentVE.Add(baseVE);

            Label reportFolder = baseVE.Q<Label>("item");
            reportFolder.text = (new DirectoryInfo(dir)).Name;
            reportFolder.RegisterCallback<ClickEvent>(SetReportData);

            ConfigureEditButton(baseVE, reportFolder.text);
            ConfigureDeleteButton(baseVE, reportFolder.text);
        }
    }

    private void ConfigureEditButton(VisualElement baseVE, string reportFolderName)
    {
        Button edit = baseVE.Q<Button>("edit");
        edit.RegisterCallback<ClickEvent>(evt => {
            Debug.Log("Clicou no Edit");
            VisualElement popup = EditNamePopup.Instantiate();
            _document.rootVisualElement.Add(popup);
            ConfigureEditPopup(popup, reportFolderName);
        });
    }

    private void ConfigureDeleteButton(VisualElement baseVE, string reportFolderName)
    {
        Button delete = baseVE.Q<Button>("delete");
        delete.RegisterCallback<ClickEvent>(evt => {
            Debug.Log("Clicou no Delete");
            VisualElement popup = DeleteItemPopup.Instantiate();
            _document.rootVisualElement.Add(popup);
            ConfigureDeletePopup(popup, reportFolderName);
        });
    }

    private void ConfigureEditPopup(VisualElement popup, string oldName)
    {
        TextField name = popup.Q<TextField>("name");
        name.SetValueWithoutNotify(oldName);
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
            ChangeNameItem(oldName, name.value);
            _document.rootVisualElement.Remove(popup);
        });
    }

    private void ConfigureDeletePopup(VisualElement popup, string name)
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
            Debug.Log($"Vai tentar deletar o {name}");
            DeleteItem(name);
            _document.rootVisualElement.Remove(popup);
        });
    }

    private void ChangeNameItem(string oldName, string newName)
    {
        try
        {
            Directory.Move(string.Format(ClassificationFolder.PathFolderFile, oldName), string.Format(ClassificationFolder.PathFolderFile, newName));
            HistoryContentVE.Clear();
            CreateStoryItens();
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
