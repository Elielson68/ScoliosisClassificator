using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageStateController : MonoBehaviour
{
    public RawImage StateImage;
    public RawImage GetActualStateImage(){
        RawImage imgState = null;
        foreach (Transform obj in transform){
            print($"Obj: {obj}");
            if (ProgressController.EstadoAtual.ToString() == obj.gameObject.name)
            {
                imgState = obj.GetComponent<RawImage>();
            }
            else if (obj.gameObject.tag == "StateImage"){
                obj.gameObject.SetActive(false);
            }
        }
        return imgState;
    }
    public void UpdateStateImage(){
       
        StateImage = GetActualStateImage();
    }

}
