using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ProgressController : MonoBehaviour
{
    public enum Estados
    {
        Frontal = 0,
        Curvada = 1,
        Lateral = 2
    }

    public Estados EstadoAtual { get; private set;}

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
    public static System.Action<string> OnInitChangeState;
    public static System.Action OnFinishChangeState;

    public UnityEvent OnCompleteAllStates;

    private void Start() {
        foreach(var stepState in PassosPorEstado)
        {
            StepsController steps = new();
            steps.Steps = stepState.Steps;
            steps.actualStep = StepText;
            
            StepsForStateDic.Add(stepState.StateName, steps);
        }
        StepsForStateDic[EstadoAtual.ToString()].UpdateText();
    }

    private void SetState(Estados novoEstado)
    {
        OnInitChangeState?.Invoke(EstadoAtual.ToString());
        foreach(var est in EstadosAnim)
        {
            if(est.name == EstadoAtual.ToString())
                est.SetBool("EstadoCompleto", true);
        }
        EstadoAtual = novoEstado;
        OnFinishChangeState?.Invoke();
    }

    public bool IsLastState()
    {
        return EstadoAtual == Estados.Lateral;
    }

    public void UpdateStepForActualState()
    {
        if(StepsForStateDic[EstadoAtual.ToString()].IsAllStepCompleted() && !IsLastState())
        {
            SetState(ProximoEstado);
            StepsForStateDic[EstadoAtual.ToString()].UpdateText();
            return;
        }
        StepsForStateDic[EstadoAtual.ToString()].UpdateStep();
        if(StepsForStateDic[EstadoAtual.ToString()].IsAllStepCompleted() && IsLastState())
            OnCompleteAllStates?.Invoke();
    }



}
