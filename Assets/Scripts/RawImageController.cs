using UnityEngine;
using UnityEngine.UI;

public class RawImageController
{
    public RawImage Image;

    public void UpdateTexturePanel(Texture2D text)
    {
        Image.texture = text;
        Image.material.mainTexture = text;
        Image.gameObject.SetActive(false);
        Image.gameObject.SetActive(true);
    }

    public Texture2D GetTexture2D(ClassificationData cls)
    {
        string imageFolder = $"{ClassificationFolder.DefinedFolderFilePath}/{cls.State}.jpg";
        return NativeGallery.LoadImageAtPath(imageFolder);
    }

    public void UpdatePositionAndScale(Vector3 pos, Vector3 scal, bool useLocalPosition=false)
    {
        UpdatePosition(pos, useLocalPosition);
        Image.transform.localScale = scal;
    }

    public void UpdatePosition(Vector3 pos, bool useLocalPosition=false)
    {
        if(useLocalPosition)
            Image.transform.localPosition = pos;
        else
            Image.transform.position = pos;
    }

    public void SetToDefaultPositionAndScale()
    {
        Image.transform.localPosition = ImageManipulation.DefaultPositionImage;
        Image.transform.localScale = ImageManipulation.DefaultScaleImage;
    }
}