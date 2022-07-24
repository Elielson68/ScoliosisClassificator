using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour
{
    public static bool IsMouseOnPoint;
    public static bool DisableMove {get; set;}
    public bool IsSacralPoint;

    void Update()
    {
        if(IsSacralPoint is false)
            gameObject.SetActive(DisableMove is false);
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
            pos.z = 0;
            transform.position = IsSacralPoint ? Vector3.right * pos.x:pos;
        }
    }
}
