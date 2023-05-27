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
        InsertImageMode.OnClickImage += UploadButton;

        UploadImage.OnCompletedUploadImage +=  StateControll.ShowFowardButton;
        UploadImage.OnChangeImage += WriteImage;
        VisualElementInteraction.IsVisualElementFocus = false;
        ClassificationFolder.GenerateFolder();
    }

    public void HiddenButtons()
    {

        UploadImage.OnCompletedUploadImage -=  StateControll.ShowFowardButton;
        UploadImage.OnChangeImage -= WriteImage;
        InsertImageMode.OnClickImage -= UploadButton;
    }

    private void UploadButton(ClickEvent evt)
    {
        UploadButton();
    }

    private void UploadButton()
    {
        OnUploadButtonClick?.Invoke();
    }

    private void WriteImage(byte[] txt)
    {
        _classifications[(int) StateController.CurrentState].classification.SetImage(txt);
    }
}
