using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class InsertImageState : MonoBehaviour
{
    public string InsertImageInfoFileName;
    public States CurrentState = States.Front;
    public List<StateInfo> Data = new List<StateInfo>();
    public static event System.Action<string, List<string>> OnChangeState;
    private void Start()
    {
        string text = File.ReadAllText("./Assets/UIToolkitScreen/Scripts/Data/" + InsertImageInfoFileName + ".json");
        Data = JsonConvert.DeserializeObject<List<StateInfo>>(text);
        photoInsertionController.OnFowardButtonClick += NextState;
        OnChangeState?.Invoke(Data[(int)CurrentState].title, Data[(int)CurrentState].content);
    }

    public void NextState()
    {
        CurrentState++;
        OnChangeState?.Invoke(Data[(int)CurrentState].title, Data[(int)CurrentState].content);
    }


}
