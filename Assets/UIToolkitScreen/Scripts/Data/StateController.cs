using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class StateController : MonoBehaviour
{
    public List<string> StateFileList;
    public States CurrentState = States.Front;
    public int CurrentStep;
    public int CurrentStateFile;
    public List<StateInfo> Data = new List<StateInfo>();
    public static event System.Action<string> OnChangeState;

    private void Start()
    {
        photoInsertionController.OnFowardButtonClick += UpdateState;
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
            }
            else
            {
                CurrentState++;
            }
        }

        OnChangeState?.Invoke(Data[(int)CurrentState].content[CurrentStep]);
        CurrentStep++;
    }

    public void ResetStateController()
    {
        CurrentState = States.Front;
        CurrentStep = 0;
        CurrentStateFile = 0;
        UpdateFile();
        OnChangeState?.Invoke(Data[(int)CurrentState].content[CurrentStep]);
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
        string text = File.ReadAllText("./Assets/UIToolkitScreen/Scripts/Data/" + StateFileList[CurrentStateFile] + ".json");
        Data = JsonConvert.DeserializeObject<List<StateInfo>>(text);
    }

    

    private bool IsAllStatesDone => (int)CurrentState == Data.Count-1;

    private bool IsAllStepsDone => CurrentStep > Data[(int)CurrentState].content.Count-1;

    private bool IsAllStatesFileReaded => IsAllStepsDone && IsAllStatesDone && CurrentStateFile == StateFileList.Count - 1;
}
