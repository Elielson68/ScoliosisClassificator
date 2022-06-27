using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePictureReal : MonoBehaviour
{
    private WebCamTexture backCam;
    public Image imgCam;
    public Vector3 originalAngle;
    private Texture texture;
    public StepsController stepPicture;
    public ProgressController statePicture;
    void Start()
    {
        var devices = WebCamTexture.devices;
        originalAngle = imgCam.rectTransform.localEulerAngles;
        foreach(var cam in devices)
        {
            if(!cam.isFrontFacing)
            {
                // (int) imgCam.preferredHeight, (int) imgCam.preferredWidth   Screen.height, Screen.width
                backCam = new WebCamTexture(cam.name, (int) imgCam.preferredWidth, (int) imgCam.preferredHeight);
            }
        }        
        imgCam.material.mainTexture = backCam;
    }


    private void OnMouseDown()
    {
        if(backCam.isPlaying){
            backCam.Pause();
            stepPicture.UpdateStep();
            statePicture.SetState(statePicture.ProximoEstado);
        }
           

        else
        {
            backCam.Play();
            imgCam.material.SetTextureScale("_BaseMap", new Vector2(1, -1));

        }  
    }

}
