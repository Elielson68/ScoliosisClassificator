using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePicture : MonoBehaviour
{
    public StepsController stepPicture;
    public ProgressController statePicture;
    public Image image;
    public string PathImage;
    private void OnMouseDown() {
        var a = NativeGallery.GetImageFromGallery(path => image.material.mainTexture = NativeGallery.LoadImageAtPath(path));
        image.mainTexture.IncrementUpdateCount();
        stepPicture.UpdateStep();
        statePicture.SetState(statePicture.ProximoEstado);
    }
    
}
