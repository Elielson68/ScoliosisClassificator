using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour
{
    public static bool IsMouseOnPoint;
    public static bool DisableMove {get; set;}
    private RectTransform rectTransform;
    public bool IsSacralPoint;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();    
    }

    void Update()
    {
        if(IsSacralPoint is false)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = DisableMove is false;
            gameObject.GetComponent<BoxCollider2D>().enabled = DisableMove is false;
        }
            
    }

    private void OnEnable() {
        DisableMove = IsSacralPoint;    
    }

    private void OnMouseDown() {
        if(CalculadorDeReta.IsLineCompleted || IsSacralPoint)
            IsMouseOnPoint = true;
    }
    private void OnMouseUp() {
        if(CalculadorDeReta.IsLineCompleted || IsSacralPoint)
            IsMouseOnPoint = false;
    }
    private void OnMouseDrag() {
        if(CalculadorDeReta.IsLineCompleted || IsSacralPoint)
        {
            IsMouseOnPoint = true;
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = rectTransform.position.z;

            if(IsSacralPoint)
            { 
                pos.y = 0;
            }

            rectTransform.SetPositionAndRotation(pos, Quaternion.identity);          
        }
    }
}
