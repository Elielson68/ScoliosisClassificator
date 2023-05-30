using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageStateController : MonoBehaviour
{
    private RawImageController _rawImageController;
    private List<ClassificationData> _classifications;

    public void Awake()
    {
        _rawImageController = new RawImageController();
        _classifications = FindObjectOfType<Classifications>()[0];
    }

    public void UpdateImageOnChangeState()
    {
        UpdateImageToState(StateController.CurrentState);
        UpdateWidthAndHeight();
    }

    public void UpdateImageToState(States state)
    {
        ClassificationData cls = _classifications[(int) state];
        Texture2D text = _rawImageController.GetTexture2D(cls);
        _rawImageController.UpdateTexturePanel(text);
    }

    public void UpdateImageToState(Texture2D img)
    {
        _rawImageController.UpdateTexturePanel(img);
    }

    public void UpdateWidthAndHeight()
    {
        _rawImageController.Image.SetNativeSize();
    }

    public void UpdatePositionAndScale(Vector3 pos, Vector3 scal, bool useLocalPosition=false)
    {
        _rawImageController.UpdatePositionAndScale(pos, scal, useLocalPosition);
    }

     public void UpdatePosition(Vector3 pos, bool useLocalPosition=false)
    {
        _rawImageController.UpdatePosition(pos, useLocalPosition);
    }
    
    public void SetToDefaultPositionAndScale()
    {
        _rawImageController.SetToDefaultPositionAndScale();
    }

    public Texture2D GetStateImage(States state)
    {
        ClassificationData cls = _classifications[(int) state];
        return _rawImageController.GetTexture2D(cls);
    }

    public void SetStateImage(RawImage image)
    {
        _rawImageController.Image = image;
    }
}
