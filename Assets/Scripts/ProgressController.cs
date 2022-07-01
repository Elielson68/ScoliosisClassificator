using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressController : MonoBehaviour
{
    public enum Estados
    {
        Frontal = 0,
        Curvada = 1,
        Lateral = 2
    }

    public static Estados EstadoAtual { get; private set;}

    public List<Animator> EstadosAnim;
    public Estados ProximoEstado { 
        get {
            return EstadoAtual + 1;
        } 
    }
    [System.Serializable]

    public struct StepsForState{
        public string StateName;
        public List<string> Steps;
    }
    public List<StepsForState> PassosPorEstado;
    public Dictionary<string, StepsController> StepsForStateDic = new();
    public TextMeshProUGUI StepText;
    public static System.Action<string> OnChangeState;

    private void Start() {
        foreach(var stepState in PassosPorEstado)
        {
            StepsController steps = new();
            steps.Steps = stepState.Steps;
            steps.actualStep = StepText;
            StepsForStateDic.Add(stepState.StateName, steps);
        }
    }

    public void SetState(Estados novoEstado)
    {
        OnChangeState?.Invoke(_estadoAtual.ToString());
        foreach(var est in EstadosAnim)
        {
            if(est.name == EstadoAtual.ToString())
                est.SetBool("EstadoCompleto", true);
        }
        EstadoAtual = novoEstado;
    }

    public void UpdateStepForActualState()
    {
        if(StepsForStateDic[EstadoAtual.ToString()].IsAllStepCompleted())
            SetState(ProximoEstado);
        StepsForStateDic[EstadoAtual.ToString()].UpdateStep();
    }



}
