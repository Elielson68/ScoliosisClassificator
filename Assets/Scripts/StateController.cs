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
    public static event System.Action<States, string> OnUpdateState;
    public static event System.Action OnChangeState;
    public UnityEvent ExecuteOnStart;
    public List<EnterNewFileExecute> ExecuteOnChangeFile;
    private Button fowardButton;
    public static event System.Action OnFowardButtonClick;

    private ReportController _reportController;

    private void Start()
    {
        fowardButton = document.rootVisualElement.Q<Button>("foward-button");
        _reportController = GetComponent<ReportController>();

        OnFowardButtonClick += UpdateState;
        fowardButton.RegisterCallback<ClickEvent>(FowardButton);

        ResetStateController();

        ExecuteOnStart?.Invoke();
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

    private bool IsAllStatesDone => (int)CurrentState == Data.Count-1;

    private bool IsAllStepsDone => CurrentStep > Data[(int)CurrentState].contents.Count-1;

    private bool IsAllStatesFileReaded => IsAllStepsDone && IsAllStatesDone && CurrentStateFile == StateFileList.Count - 1;
}
