using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class StateController : MonoBehaviour
{
    private const string _stepIncomplete = "step-incomplete";
    private const string _stepComplete = "step-complete";

    public UIDocument document;
    public VisualTreeAsset StepAsset;
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
    private VisualElement _footerButtonsContainer;
    private VisualElement _stepContainer;
    
    private List<VisualElement> _StepsVE = new List<VisualElement>();

    private void OnEnable()
    {
        CurrentStep = 0;
        CurrentStateFile = 0;
        _footerButtonsContainer = document.rootVisualElement.Q("insert-image-flow");
        fowardButton = document.rootVisualElement.Q<Button>("foward-button");
        content = document.rootVisualElement.Q<Label>("title");
        _stepContainer = document.rootVisualElement.Q("steps-area");

        OnFowardButtonClick += UpdateState;
        fowardButton.RegisterCallback<ClickEvent>(FowardButton);

        ResetStateController();

        ExecuteOnStart?.Invoke();
        HideFowardButton();

        fowardButton.RegisterCallback<FocusInEvent>(evt => VisualElementInteraction.IsVisualElementFocus = true, TrickleDown.TrickleDown);
        fowardButton.RegisterCallback<FocusOutEvent>(evt => VisualElementInteraction.IsVisualElementFocus = false);
    }

    private void OnDisable()
    {
        OnFowardButtonClick -= UpdateState;
        fowardButton.UnregisterCallback<ClickEvent>(FowardButton);
        fowardButton.UnregisterCallback<FocusInEvent>(evt => VisualElementInteraction.IsVisualElementFocus = true);
        fowardButton.UnregisterCallback<FocusOutEvent>(evt => VisualElementInteraction.IsVisualElementFocus = false);
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
        UpdateStepVisualElements();
    }
    
    private void UpdateStateFile()
    {
        CurrentStateFile++;
        CurrentState = States.Front;
        UpdateFile();
        UpdateStepVisualElements();
    }

    private void UpdateStepVisualElements()
    {
        ClearStepsVisualElements();
        for(int i=0; i < Data.Count; i++)
        {
            VisualElement step = StepAsset.Instantiate().Q("step");
            _stepContainer.Add(step);
            _StepsVE.Add(step.Q(_stepComplete));
        }
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
    public void ClearStepsVisualElements()
    {
        _StepsVE.Clear();
        _stepContainer.Clear();
    }
    public void ShowFowardButton()
    {
        fowardButton.style.display = DisplayStyle.Flex;
        _StepsVE[(int)CurrentState].AddToClassList(_stepComplete);
    }

    public void HideFowardButton()
    {
        fowardButton.style.display = DisplayStyle.None;
    }

    private bool IsAllStatesDone => (int)CurrentState == Data.Count-1;

    private bool IsAllStepsDone => CurrentStep > Data[(int)CurrentState].contents.Count-1;

    private bool IsAllStatesFileReaded => IsAllStepsDone && IsAllStatesDone && CurrentStateFile == StateFileList.Count - 1;
}
