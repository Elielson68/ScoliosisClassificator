using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StepsController : MonoBehaviour
{
    public List<string> Steps;
    private int _indexActualStep = 0;

    public TextMeshProUGUI actualStep;
    public ProgressController progress;
    
    private void Start() 
    {
        actualStep.text = Steps[_indexActualStep];    
    }
    
    public void UpdateStep()
    {
        if (_indexActualStep < Steps.Count - 1)
        {
            actualStep.text = Steps[++_indexActualStep];
        }
        else
        {
            progress.SetState(progress.ProximoEstado);
        }
    }
}
