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
    public static System.Action<States> OnUpdateState;
    public static System.Action OnChangeState;
    public UnityEvent ExecuteOnStart;
    public List<EnterNewFileExecute> ExecuteOnChangeFile;
    private Button fowardButton;
    public static System.Action OnFowardButtonClick;
    public static System.Action OnBeforeUpdateState;
    private Label content;

    private void OnEnable()
    {
        CurrentStep = 0;
        CurrentStateFile = 0;
        fowardButton = document.rootVisualElement.Q<Button>("foward-button");
        content = document.rootVisualElement.Q<Label>("content");
        OnFowardButtonClick += UpdateState;
        fowardButton.RegisterCallback<ClickEvent>(FowardButton);

        ResetStateController();

        ExecuteOnStart?.Invoke();
    }

    private void OnDisable()
    {
        OnFowardButtonClick -= UpdateState;
        fowardButton.UnregisterCallback<ClickEvent>(FowardButton);
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
                OnBeforeUpdateState?.Invoke();
                UpdateStateFile();
                ExecuteOnChangeFile.ForEach(actionEx => {
                    if(actionEx.FileName == StateFileList[CurrentStateFile])
                    {
                        actionEx.Execute?.Invoke();
                    }
                });
                Debug.Log("Executou as actions do "+StateFileList[CurrentStateFile]);
            }
            else
            {
                OnBeforeUpdateState?.Invoke();
                CurrentState++;
                OnChangeState?.Invoke();
            }
        }

        OnUpdateState?.Invoke(CurrentState);
        content.text = Data[(int)CurrentState].contents[CurrentStep];
        CurrentStep++;
    }

    public void ResetStateController()
    {
        CurrentState = States.Front;
        CurrentStep = 0;
        CurrentStateFile = 0;
        UpdateFile();
        OnUpdateState?.Invoke(CurrentState);
        content.text = Data[(int)CurrentState].contents[CurrentStep];
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
        string text = File.ReadAllText($"{ClassificationFolder.SaveDataFolder}/StatesFiles/{StateFileList[CurrentStateFile]}.json");
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
