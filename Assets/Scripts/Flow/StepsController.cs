using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StepsController : MonoBehaviour
{
    public List<string> Steps = new();
    private int _indexActualStep = 0;

    public int IndexActualStep => _indexActualStep;

    public TextMeshProUGUI actualStep;
    
    public void UpdateStep()
    {
        if (_indexActualStep < Steps.Count)
        {
            actualStep.text = Steps[_indexActualStep];
            _indexActualStep++;
        }
    }

    public bool IsAllStepCompleted()
    {
        return _indexActualStep == Steps.Count;
    }

    public void ResetStep()
    {
        _indexActualStep = 0;
    }
}
