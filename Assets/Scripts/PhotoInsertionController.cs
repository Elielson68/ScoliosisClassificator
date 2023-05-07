using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PhotoInsertionController : MonoBehaviour
{
    public UIDocument document;
    public StateController StateControll;
    private Button uploadButton;
    
    public static event System.Action OnUploadButtonClick;
    private Label content;
    

    private List<ClassificationData> _classifications;
    public void StartClass()
    {
        _classifications = FindObjectOfType<Classifications>()[0];
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

    public void ShowButtons()
    {
        uploadButton.RemoveFromClassList("element-hidden");
        document.rootVisualElement.Q<Button>("picture-button").RemoveFromClassList("element-hidden");

        uploadButton.style.display = DisplayStyle.Flex;
        document.rootVisualElement.Q<Button>("picture-button").style.display = DisplayStyle.Flex;
    }

    private void UploadButton(ClickEvent evt)
    {
        OnUploadButtonClick?.Invoke();
    }

    private void UpdateTextOnChangeState(States state, string contentState)
    {
        content.text = contentState;
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
