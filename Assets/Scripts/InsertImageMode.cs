using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InsertImageMode : MonoBehaviour
{
    public static bool s_InsertImageModeActive = true;
    /// <summary>
    /// Call by event
    /// </summary>
    /// <value></value>
    public bool InsertImageModeActive {get => s_InsertImageModeActive; set => s_InsertImageModeActive = value;}
    public static System.Action OnClickImage;
    public Color InsertImageColor;
    public Color WithImageColor;
    public Texture2D BlankTexture;
    private TextMeshProUGUI _text;
    private RawImage _image;
    
    public void Init()
    {
        _text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _image = GetComponent<RawImage>();
        StateController.OnUpdateState += ResetImage;
        UploadImage.OnCompletedUploadImage += OnImageInserted;
        ResetImage(States.Front);
    }

    public void Exit()
    {
        StateController.OnUpdateState -= ResetImage;
        UploadImage.OnCompletedUploadImage -= OnImageInserted;
    }

    private void OnMouseDown()
    {
        if(s_InsertImageModeActive)
        {
            OnClickImage?.Invoke();
        }
    }

    private void OnImageInserted()
    {
        _image.color = WithImageColor;
        _text.gameObject.SetActive(false);
    }

    private void ResetImage(States state)
    {
        _image.color = InsertImageColor;
        _text.gameObject.SetActive(true);
        _image.texture = BlankTexture;
        _image.material.mainTexture = BlankTexture;
        
        var size = _image.rectTransform.sizeDelta;
        size.x = 810;
        size.y = 1440;
        _image.rectTransform.sizeDelta = size;
        _image.transform.localPosition = Vector3.zero;
    }
    
}
