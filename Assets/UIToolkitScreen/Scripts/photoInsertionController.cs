using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class photoInsertionController : MonoBehaviour
{
    public UIDocument document;
    public StateController StateControll;
    private Button uploadButton;
    
    public static event System.Action OnUploadButtonClick;
    private Label content;
    

    public List<ClassificationData> classifications;
    void Start()
    {
        content = document.rootVisualElement.Q<Label>("content");
        uploadButton = document.rootVisualElement.Q<Button>("upload-button");

        uploadButton.RegisterCallback<ClickEvent>(UploadButton);
        
        StateController.OnUpdateState += UpdateTextOnChangeState;

        UploadImage.OnCompletedUploadImage +=  StateControll.ShowFowardButton;
        UploadImage.OnChangeImage += WriteImage;
    }

    public void HiddenButtons()
    {
        uploadButton.AddToClassList("element-hidden");
        document.rootVisualElement.Q<Button>("picture-button").AddToClassList("element-hidden");
    }

    private void UploadButton(ClickEvent evt)
    {
        OnUploadButtonClick?.Invoke();
    }

    private void UpdateTextOnChangeState(States state, string contentState)
    {
        content.text = contentState;
        foreach(ClassificationData cd in classifications)
        {
            if(cd.State == state)
            {
                UploadImage.OnChangeImage += cd.classification.SetImage;
            }
            else
            {
                UploadImage.OnChangeImage -= cd.classification.SetImage;
            }
        }
    }

    private void WriteImage(byte[] txt)
    {
        foreach(ClassificationData cd in classifications)
        {
            if(cd.State == StateController.CurrentState)
            {
                cd.classification.SetImage(txt);
                break;
            }
        }
    }
}
