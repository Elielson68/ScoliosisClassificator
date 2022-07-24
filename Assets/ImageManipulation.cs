using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageManipulation : MonoBehaviour
{
    public float DistanciaMatriz;
    public float Zoom;
    public RawImage imagem;
    int FrameCount;
    public Vector3 OneTouchPosition;
    public bool MovingImage;
    public static bool DisableImageManipulation {get; set;} = true;
    private void Update()
    {
        if(DisableImageManipulation) return;

        if (Input.touchCount > 1 && FrameCount > 5)
        {
            var pos_touch_1 = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            var pos_touch_2 = Camera.main.ScreenToWorldPoint(Input.GetTouch(1).position);
            if(Input.GetTouch(1).phase == TouchPhase.Began)
                DistanciaMatriz = Vector3.Distance(pos_touch_1, pos_touch_2);
            else if(Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved && imagem.transform.localScale.x > 0)
            {
                Zoom = Vector3.Distance(pos_touch_1, pos_touch_2) - DistanciaMatriz;
                imagem.transform.localScale += ((Vector3.one * Zoom)/10);
            }

            if(imagem.transform.localScale.x < 0)
                imagem.transform.localScale = Vector3.one/10;
            FrameCount = 0;
            MovingImage = false;
            
        } 
        else if(Input.touchCount == 1)
        {
            
            var pos_touch_1 = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            pos_touch_1.z = 90;
            float distance = 0;
            if(Input.GetTouch(0).phase is TouchPhase.Began || Input.GetTouch(0).phase is TouchPhase.Ended)
                OneTouchPosition = pos_touch_1;
            else
                distance = Vector3.Distance(OneTouchPosition, pos_touch_1);
            if(distance > 1)
                MovingImage = true;
            if(MovingImage)
            {
                imagem.transform.position = pos_touch_1;
            }
            FrameCount = 0;
        }
        else
        {
            MovingImage = false;
        }
        FrameCount++;   
    }
}
