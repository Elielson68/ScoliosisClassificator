using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePicture : MonoBehaviour
{
    public StepsController stepPicture;
    public ProgressController statePicture;
    public RawImage image;
    public static System.Action OnUploadImage;

    private void OnMouseDown() {
        OnUploadImage?.Invoke();
        stepPicture.UpdateStep();
        statePicture.SetState(statePicture.ProximoEstado);
        StartCoroutine(UploadImage());
    }

    IEnumerator UploadImage()
    {
        image.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        image.texture = null;
        image.material.mainTexture = null;
        yield return new WaitForEndOfFrame();
        var a = NativeGallery.GetImageFromGallery(path => {
            var text = NativeGallery.LoadImageAtPath(path);
            image.texture = text;
            image.material.mainTexture = text;
        });
        yield return new WaitUntil(() => a == NativeGallery.Permission.Granted);
        image.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        image.gameObject.SetActive(true);
    }
    
}
