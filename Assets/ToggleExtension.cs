using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleExtension : Toggle
{
    
    public override void OnPointerDown(PointerEventData eventData)
    {
        ImageManipulation.DisableImageManipulation = true;
        CalculadorDeReta.BlockCreationLineGlobal = true;
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
    }

}
