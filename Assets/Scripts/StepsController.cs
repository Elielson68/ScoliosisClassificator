using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StepsController : MonoBehaviour
{
    public List<string> Steps = new();
    private int _indexActualStep = 0;

    public int IndexActualStep {get => _indexActualStep;}

    public TextMeshProUGUI actualStep;
    
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
    }

    public bool IsAllStepCompleted()
    {
        return _indexActualStep == (Steps.Count-1);
    }
}
