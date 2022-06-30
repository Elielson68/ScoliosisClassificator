using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePicture : MonoBehaviour
{
    public ImageStateController imgState;
    public StepsController stepPicture;
    public ProgressController statePicture;
    public static System.Action OnUploadImage;

    private void Start(){
        imgState.UpdateStateImage();
    }

    private void OnMouseDown() {
        OnUploadImage?.Invoke();
        stepPicture.UpdateStep();
        statePicture.SetState(statePicture.ProximoEstado);
        StartCoroutine(UploadImage());
        imgState.UpdateStateImage();
    }

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
    }
    
}
