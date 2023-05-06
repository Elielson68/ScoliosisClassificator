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
        Texture2D text = new Texture2D(2, 2);
        text.LoadImage(cls.classification.Image);
        text.Apply();
        return text;
    }
}