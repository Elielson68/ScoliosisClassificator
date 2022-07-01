using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageStateController : MonoBehaviour
{
    public RawImage StateImage;
    public ProgressController progressController;

    private void Start()
    {
        ProgressController.OnFinishChangeState += UpdateStateImage;
    }

    public RawImage GetActualStateImage(){
        RawImage imgState = null;
        foreach (Transform obj in transform){
            if (progressController.EstadoAtual.ToString() == obj.gameObject.name)
            {
                imgState = obj.GetComponent<RawImage>();
                obj.gameObject.SetActive(true);
            }
            else if (obj.gameObject.tag == "StateImage"){
                Debug.LogWarning($"Estado: {obj.gameObject.name}");
                obj.gameObject.SetActive(false);
            }
        }
        return imgState;
    }
    public void UpdateStateImage(){
       
        StateImage = GetActualStateImage();
    }

    public void SetProgressController (ProgressController p)
    {
        progressController = p;
    }

}
