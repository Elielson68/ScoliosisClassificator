using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour
{
    //public ImageStateController imgState;
    private WebCamTexture backCam;
    public RawImage camDevice;
    Texture2D photo;

    private void Start()
    {
        //imgState.UpdateStateImage();
        UploadImage.OnCompletedUploadImage += Reset;
        SetCam();
    }

    private void SetCam()
    {
        var devices = WebCamTexture.devices;
        foreach (var cam in devices)
        {
            if (!cam.isFrontFacing)
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
        if (backCam.isPlaying)
        {
            StartCoroutine(MakePhoto());
        }

        else
        {
            //RawImage StateImage = imgState.StateImage;
            //StateImage.gameObject.SetActive(false);
            camDevice.gameObject.SetActive(true);
            backCam.Play();
            camDevice.rectTransform.localEulerAngles = new Vector3(0, 0, -backCam.videoRotationAngle);
            var size = camDevice.rectTransform.sizeDelta;
            size.x = Screen.height;
            size.y = Screen.width;
            camDevice.rectTransform.sizeDelta = size;
        }
    }

    IEnumerator MakePhoto()
    {
        //RawImage StateImage = imgState.StateImage;
        yield return new WaitForEndOfFrame();
        photo = new Texture2D(backCam.width, backCam.height);
        photo.SetPixels(backCam.GetPixels());
        photo.Apply();
        yield return new WaitForEndOfFrame();
        backCam.Stop();
        yield return new WaitForEndOfFrame();
        //StateImage.material.mainTexture = photo;
        //StateImage.texture = photo;
        //StateImage.gameObject.SetActive(true);
        //StateImage.rectTransform.localEulerAngles = new Vector3(0, 0, 270);
        camDevice.gameObject.SetActive(false);
        //var size = StateImage.rectTransform.sizeDelta;
        //size.x = Screen.height;
        //size.y = Screen.width;
        //StateImage.rectTransform.sizeDelta = size;
    }
    public void Reset()
    {
        if (backCam is not null && backCam.isPlaying)
        {
            backCam.Stop();
            camDevice.gameObject.SetActive(false);
        }

    }

}
