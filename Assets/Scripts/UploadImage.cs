using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;
using Newtonsoft.Json;

public class UploadImage : MonoBehaviour
{
    public RawImage StateImage;
    public static System.Action OnCompletedUploadImage;
    public static System.Action<byte[]> OnChangeImage;
    private Coroutine _uploadRoutine;
    
    void Start()
    {
        PhotoInsertionController.OnUploadButtonClick += Init;
    }

    private IEnumerator Upload()
    {
        StateImage.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();

        if(VisualElementInteraction.IsVisualElementFocus)
        {
            _uploadRoutine = null;
            yield break;
        }

        var a = NativeGallery.GetImageFromGallery(path =>
        {
            if (path is not null)
            {
                var text = NativeGallery.LoadImageAtPath(path, markTextureNonReadable: false);
                StateImage.texture = text;
                StateImage.material.mainTexture = text;
                OnChangeImage?.Invoke(text.EncodeToJPG());
                StateImage.gameObject.SetActive(false);
                StateImage.gameObject.SetActive(true);
                OnCompletedUploadImage?.Invoke();
            }
        });
        _uploadRoutine = null;
    }

    public void Init()
    {
        if(VisualElementInteraction.IsVisualElementFocus || _uploadRoutine is not null) 
        {
            return;
        }
        _uploadRoutine = StartCoroutine(Upload());
    }
}
