using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageStateController : MonoBehaviour
{
    public Texture2D DefaultImage;
    public RawImage StateImage;
    public ProgressController progressController;

    public static event System.Action<RawImage> OnStateImageChange;

    private void Start()
    {
        ProgressController.OnFinishChangeState += UpdateStateImage;
    }

    private void OnEnable()
    {
        UpdateStateImage();
    }

    public RawImage GetActualStateImage()
    {
        RawImage imgState = null;
        foreach (Transform obj in transform)
        {
            if (progressController.EstadoAtual.ToString() == obj.gameObject.name)
            {
                imgState = obj.GetComponent<RawImage>();
                obj.gameObject.SetActive(true);
            }
            else if (obj.gameObject.tag == "StateImage")
            {
                obj.gameObject.SetActive(false);
            }
        }
        return imgState;
    }
    public void UpdateStateImage()
    {

        StateImage = GetActualStateImage();
        StateImage.SetNativeSize();
        OnStateImageChange?.Invoke(StateImage);
    }

    public void SetProgressController(ProgressController p)
    {
        progressController = p;
    }

    private void OnDisable()
    {
        foreach (Transform obj in transform)
        {
            if (obj.gameObject.tag == "StateImage")
            {
                obj.GetComponent<RawImage>().texture = DefaultImage;
            }
        }
    }

}
