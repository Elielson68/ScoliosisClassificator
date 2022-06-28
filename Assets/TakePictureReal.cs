using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TakePictureReal : MonoBehaviour
{
    public static WebCamTexture backCam;
    public RawImage imgCam;
    public RawImage camDevice;
    Texture2D photo;
    
    private void Start()
    {
        TakePicture.OnUploadImage += Reset;
        SetCam();    
    }

    private void SetCam()
    {
        var devices = WebCamTexture.devices;
        foreach(var cam in devices)
        {
            if(!cam.isFrontFacing)
            {
                // (int) imgCam.preferredHeight, (int) imgCam.preferredWidth   Screen.height, Screen.width
                backCam = new WebCamTexture(cam.name, Screen.width, Screen.height);
            }
        }
        camDevice.texture = backCam;
        camDevice.material.mainTexture = backCam;
    }

    private void OnMouseDown()
    {
        if(backCam.isPlaying)
        {
            StartCoroutine(MakePhoto());
        }
            
        else
        {
            imgCam.gameObject.SetActive(false);
            camDevice.gameObject.SetActive(true);
            backCam.Play();
            camDevice.rectTransform.localEulerAngles = new Vector3(0, 0, -backCam.videoRotationAngle);
        }  
    }

    IEnumerator MakePhoto()
    {
        yield return new WaitForEndOfFrame();
        photo = new Texture2D(backCam.width, backCam.height);
        photo.SetPixels(backCam.GetPixels());
        photo.Apply();
        yield return new WaitForEndOfFrame();
        backCam.Stop();
        yield return new WaitForEndOfFrame();
        imgCam.material.mainTexture = photo;
        imgCam.texture = photo;
        imgCam.gameObject.SetActive(true);
        imgCam.rectTransform.localEulerAngles = new Vector3(0, 0, 270);
        camDevice.gameObject.SetActive(false);
    }
    public void Reset()
    {
        if(backCam is not null && backCam.isPlaying)
        {
            backCam.Stop();
            camDevice.gameObject.SetActive(false);
        }
            
    }

}
