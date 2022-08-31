using System.Collections;
using System.Collections.Generic;
using SFB;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using TMPro;

public class TakePicture : MonoBehaviour, IPointerDownHandler
{
    public ImageStateController imgState;
    public ProgressController statePicture;
    public static System.Action OnUploadImage;
    public UnityEvent OnClick;
    public TMP_Text texto;

    private void Start(){
        imgState.UpdateStateImage();
    }

    // private void OnMouseDown() {
    //     OnUploadImage?.Invoke();
    //     //StartCoroutine(UploadImage());
    //     //var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);
    //     //Debug.Log(paths[0]);
    //     OnClick?.Invoke();
    // }

    IEnumerator UploadImage()
    {
        RawImage StateImage = imgState.StateImage;
        StateImage.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        StateImage.texture = null;
        StateImage.material.mainTexture = null;
        yield return new WaitForEndOfFrame();
        var a = NativeGallery.GetImageFromGallery(path => {
            var text = NativeGallery.LoadImageAtPath(path);
            StateImage.texture = text;
            StateImage.material.mainTexture = text;
        });
        yield return new WaitUntil(() => a == NativeGallery.Permission.Granted);
        StateImage.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        StateImage.gameObject.SetActive(true);
        statePicture.UpdateStepForActualState();
    }

    IEnumerator UploadImage(string url)
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
        StartCoroutine(UploadImage(url));
    }
#else
    public void OnPointerDown(PointerEventData eventData)
    {
        OnUploadImage?.Invoke();
        StartCoroutine(UploadImage());
    }
#endif
}
