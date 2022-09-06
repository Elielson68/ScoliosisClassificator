using System.Collections;
using System.Collections.Generic;
using SFB;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using TMPro;

public class UploadImage : MonoBehaviour, IPointerDownHandler
{
    public ImageStateController imgState;
    public ProgressController statePicture;
    public static System.Action OnUploadImage;
    public UnityEvent OnClick;
    public TMP_Text texto;

    private void Start()
    {
        imgState.UpdateStateImage();
    }

    IEnumerator Upload()
    {
        RawImage StateImage = imgState.StateImage;
        StateImage.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        yield return new WaitForEndOfFrame();
        bool imageChoose = false;
        var a = NativeGallery.GetImageFromGallery(path =>
        {
            if (path is not null)
            {
                var text = NativeGallery.LoadImageAtPath(path);
                StateImage.texture = text;
                StateImage.material.mainTexture = text;
                imageChoose = true;
            }
        });
        yield return new WaitUntil(() => a == NativeGallery.Permission.Granted);
        StateImage.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        StateImage.gameObject.SetActive(true);
        if (imageChoose)
            statePicture.UpdateStepForActualState();
    }

    IEnumerator Upload(string url)
    {
        RawImage StateImage = imgState.StateImage;
        StateImage.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        StateImage.texture = null;
        StateImage.material.mainTexture = null;
        yield return new WaitForEndOfFrame();
        var loader = new WWW(url);
        yield return loader;
        StateImage.texture = loader.texture;
        StateImage.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        StateImage.gameObject.SetActive(true);
        statePicture.UpdateStepForActualState();
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData) {
        texto.text += "\nClicou no OnPointerDown\n";
        UploadFile(gameObject.name, "OnFileUpload", ".png, .jpg, .jpeg", false);
    }

    public void OnFileUpload(string url) {
        texto.text = $"\n{url}\n";
        StartCoroutine(Upload(url));
    }
#else
    public void OnPointerDown(PointerEventData eventData)
    {
        OnUploadImage?.Invoke();
        StartCoroutine(Upload());
    }
#endif
}
