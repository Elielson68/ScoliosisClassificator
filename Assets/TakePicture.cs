using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePicture : MonoBehaviour
{
    public Image image;
    public string PathImage;
    private void OnMouseDown() {
        print("Meu cu fede");
        var a = NativeGallery.GetImageFromGallery(path => image.material.mainTexture = NativeGallery.LoadImageAtPath(path));
        image.mainTexture.IncrementUpdateCount();
    }
    
}
