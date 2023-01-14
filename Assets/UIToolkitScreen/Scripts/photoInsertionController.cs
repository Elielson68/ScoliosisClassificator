using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class photoInsertionController : MonoBehaviour
{
    public UIDocument document;
    private Button uploadButton;
    private Button fowardButton;
    public static event System.Action OnFowardButtonClick;
    public static event System.Action OnUploadButtonClick;
    private Label content;
    [System.Serializable]
    public struct ClassificationData
    {
        public States State;
        public Classification classification;
    }
    public List<ClassificationData> classifications;

    void Start()
    {
        fowardButton = document.rootVisualElement.Q<Button>("foward-button");
        content = document.rootVisualElement.Q<Label>("content");
        uploadButton = document.rootVisualElement.Q<Button>("upload-button");

        uploadButton.RegisterCallback<ClickEvent>(UploadButton);
        fowardButton.RegisterCallback<ClickEvent>(FowardButton);
        StateController.OnChangeState += UpdateTextOnChangeState;

        UploadImage.OnCompletedUploadImage += () => fowardButton.RemoveFromClassList("element-hidden");
        UploadImage.OnChangeImage += classifications[0].classification.SetImage;
        
    }

    void FowardButton(ClickEvent evt)
    {
        OnFowardButtonClick?.Invoke();
        fowardButton.AddToClassList("element-hidden");

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



}
