using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePicture : MonoBehaviour
{
    public Material image;
    public string PathImage;
    private void OnMouseDown() {
        print("Meu cu fede");
        var a = NativeGallery.GetImageFromGallery(path => image.mainTexture = NativeGallery.LoadImageAtPath(path));
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
    
}
