using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class StateController : MonoBehaviour
{
    public List<string> StateFileList;
    public static States CurrentState = States.Front;
    public int CurrentStep;
    public int CurrentStateFile;
    public List<StateInfo> Data = new List<StateInfo>();
    public static event System.Action<States, string> OnUpdateState;
    public static event System.Action OnChangeState;

    public UnityEvent<string> OnChangeFile;

    private void Start()
    {
        photoInsertionController.OnFowardButtonClick += UpdateState;
        DrawLinesController.OnCompleteRule += UpdateState;
        ResetStateController();
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
                OnChangeFile?.Invoke(StateFileList[CurrentStateFile]);
            }
            else
            {
                CurrentState++;
                OnChangeState?.Invoke();
            }
        }

        OnUpdateState?.Invoke(CurrentState, Data[(int)CurrentState].content[CurrentStep]);
        CurrentStep++;
    }

    public void ResetStateController()
    {
        CurrentState = States.Front;
        CurrentStep = 0;
        CurrentStateFile = 0;
        UpdateFile();
        OnUpdateState?.Invoke(CurrentState, Data[(int)CurrentState].content[CurrentStep]);
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
        string text = File.ReadAllText("./Assets/UIToolkitScreen/Scripts/Data/Json/" + StateFileList[CurrentStateFile] + ".json");
        Data = JsonConvert.DeserializeObject<List<StateInfo>>(text);
    }

    

    private bool IsAllStatesDone => (int)CurrentState == Data.Count-1;

    private bool IsAllStepsDone => CurrentStep > Data[(int)CurrentState].content.Count-1;

    private bool IsAllStatesFileReaded => IsAllStepsDone && IsAllStatesDone && CurrentStateFile == StateFileList.Count - 1;
}
