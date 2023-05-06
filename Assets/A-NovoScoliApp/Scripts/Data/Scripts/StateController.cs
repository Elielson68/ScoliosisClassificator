using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class StateController : MonoBehaviour
{
    public UIDocument document;
    public List<string> StateFileList;
    public static States CurrentState = States.Front;
    public int CurrentStep;
    public int CurrentStateFile;
    public List<StateInfo> Data = new List<StateInfo>();
    public List<ReportInfo> DataReport = new List<ReportInfo>();
    public static event System.Action<States, string> OnUpdateState;
    public static event System.Action OnChangeState;
    public UnityEngine.UI.RawImage ReportImage;
    [System.Serializable]
    public struct EnterNewFileExecute
    {
        public string FileName;
        public UnityEvent Execute;
    }
    
    public List<EnterNewFileExecute> ExecuteOnChangeFile;
    private Button fowardButton;
    public static event System.Action OnFowardButtonClick;

    public bool IsReportState {get; set;}
    public List<ClassificationData> Classifications;

    public VisualElement ReportButtonsContent;
    public RadioButton FrontalReportButton;
    public RadioButton LeftInclinationReportButton;
    public RadioButton RightInclinationReportButton;
    public RadioButton LateralReportButton;
    public Dictionary<States, RadioButton> radioButtons = new Dictionary<States, RadioButton>();

    public GameObject LineParent;
    public GameObject LinePrefab;
    private void Start()
    {
        fowardButton = document.rootVisualElement.Q<Button>("foward-button");

        OnFowardButtonClick += UpdateState;
        fowardButton.RegisterCallback<ClickEvent>(FowardButton);
        ResetStateController();

        ReportButtonsContent = document.rootVisualElement.Q("report-content");
        radioButtons.Add(States.Front, ReportButtonsContent.Q<RadioButton>("Frontal"));
        radioButtons.Add(States.LeftInclination, ReportButtonsContent.Q<RadioButton>("LeftInclination"));
        radioButtons.Add(States.RightInclination, ReportButtonsContent.Q<RadioButton>("RightInclination"));
        radioButtons.Add(States.Lateral, ReportButtonsContent.Q<RadioButton>("Lateral"));
    }

    public void UpdateState()
    {
        if(IsAllStatesFileReaded)
        {
            Debug.Log("Não há mais arquivos");
            return;
        }

        if(IsAllStepsDone)
        {
            CurrentStep = 0;
            if(IsAllStatesDone)
            {
                UpdateStateFile();
                ExecuteOnChangeFile.ForEach(actionEx => {
                    if(actionEx.FileName == StateFileList[CurrentStateFile])
                    {
                        actionEx.Execute?.Invoke();
                    }
                });
            }
            else
            {
                CurrentState++;
                OnChangeState?.Invoke();
            }
        }

        if(IsReportState)
        {
            UpdateFile();
            OnUpdateState?.Invoke(CurrentState, DataReport[(int)CurrentState].contents[CurrentStep]);
            return;
        }
        OnUpdateState?.Invoke(CurrentState, Data[(int)CurrentState].contents[CurrentStep]);
        CurrentStep++;
    }

    public void ResetStateController()
    {
        CurrentState = States.Front;
        CurrentStep = 0;
        CurrentStateFile = 0;
        UpdateFile();
        OnUpdateState?.Invoke(CurrentState, Data[(int)CurrentState].contents[CurrentStep]);
        CurrentStep++;
    }
    
    private void UpdateStateFile()
    {
        CurrentStateFile++;
        CurrentState = States.Front;
        UpdateFile();
    }

    private void UpdateFile()
    {
        string text = File.ReadAllText($"{Application.streamingAssetsPath}/StatesFiles/{StateFileList[CurrentStateFile]}.json");
        if(IsReportState)
        {
            Data = null;
            DataReport = JsonConvert.DeserializeObject<List<ReportInfo>>(text);
            return;
        }
        Data = JsonConvert.DeserializeObject<List<StateInfo>>(text);
    }

    void FowardButton(ClickEvent evt)
    {
        OnFowardButtonClick?.Invoke();
        HideFowardButton();
    }

    public void ShowFowardButton()
    {
        fowardButton.RemoveFromClassList("element-hidden");
    }

    public void HideFowardButton()
    {
        fowardButton.AddToClassList("element-hidden");
    }

    public void ShowReportButtons()
    {
        ReportButtonsContent.style.display = DisplayStyle.Flex;
        foreach(var classification in Classifications)
        {
            radioButtons[classification.State].style.backgroundImage = new StyleBackground(GetTexture2D(classification));
            radioButtons[classification.State].RegisterCallback<ClickEvent>(UpdateTexturePanel);
            radioButtons[classification.State].RegisterCallback<ClickEvent, List<Line>>(DrawLine, classification.classification.Lines);
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

    private bool IsAllStatesDone => (int)CurrentState == Data.Count-1;

    private bool IsAllStepsDone => CurrentStep > Data[(int)CurrentState].contents.Count-1;

    private bool IsAllStatesFileReaded => IsAllStepsDone && IsAllStatesDone && CurrentStateFile == StateFileList.Count - 1;
}
