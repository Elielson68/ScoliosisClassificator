using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour
{
    public static bool IsMouseOnPoint;

    private void OnMouseDown() {
        if(CalculadorDeReta.IsLineCompleted)
            IsMouseOnPoint = true;
    }
    private void OnMouseUp() {
        if(CalculadorDeReta.IsLineCompleted)
            IsMouseOnPoint = false;
    }
    private void OnMouseDrag() {
        if(CalculadorDeReta.IsLineCompleted)
        {
            IsMouseOnPoint = true;
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            transform.position = pos;
        }
        
    }
}
