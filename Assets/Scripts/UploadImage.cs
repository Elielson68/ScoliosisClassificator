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
    void Start()
    {
        PhotoInsertionController.OnUploadButtonClick += Init;
    }

    IEnumerator Upload()
    {
        StateImage.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        yield return new WaitForEndOfFrame();
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
    }

    public void Init()
    {
        StartCoroutine(Upload());
    }
}
