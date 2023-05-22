using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PhotoInsertionController : MonoBehaviour
{
    public UIDocument document;
    public StateController StateControll;
    private Button uploadButton;
    public static event System.Action OnUploadButtonClick;
    private List<ClassificationData> _classifications;

    private void Start()
    {
        
    }
    
    public void StartClass()
    {
        _classifications = FindObjectOfType<Classifications>()[0];
        //uploadButton = document.rootVisualElement.Q<Button>("upload-button");

        //uploadButton.RegisterCallback<ClickEvent>(UploadButton);
        InsertImageMode.OnClickImage += UploadButton;
        StateController.OnUpdateState += UpdateTextOnChangeState;

        UploadImage.OnCompletedUploadImage +=  StateControll.ShowFowardButton;
        UploadImage.OnChangeImage += WriteImage;
        
    }

    public void HiddenButtons()
    {
        //uploadButton.AddToClassList("element-hidden");
        //document.rootVisualElement.Q<Button>("picture-button").AddToClassList("element-hidden");

        //uploadButton.UnregisterCallback<ClickEvent>(UploadButton);
        StateController.OnUpdateState -= UpdateTextOnChangeState;
        UploadImage.OnCompletedUploadImage -=  StateControll.ShowFowardButton;
        UploadImage.OnChangeImage -= WriteImage;
        InsertImageMode.OnClickImage -= UploadButton;
    }

    // public void ShowButtons()
    // {
    //     uploadButton.RemoveFromClassList("element-hidden");
    //     document.rootVisualElement.Q<Button>("picture-button").RemoveFromClassList("element-hidden");

    //     uploadButton.style.display = DisplayStyle.Flex;
    //     document.rootVisualElement.Q<Button>("picture-button").style.display = DisplayStyle.Flex;
    // }

    private void UploadButton(ClickEvent evt)
    {
        UploadButton();
    }

    private void UploadButton()
    {
        OnUploadButtonClick?.Invoke();
    }

    private void UpdateTextOnChangeState(States state)
    {
        foreach(ClassificationData cd in _classifications)
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
        foreach(ClassificationData cd in _classifications)
        {
            if(cd.State == StateController.CurrentState)
            {
                cd.classification.SetImage(txt);
                break;
            }
        }
    }
}
