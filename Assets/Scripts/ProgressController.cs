using System;
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
        InclinacaoEsquerda = 1,
        InclinacaoDireita = 2,
        Lateral = 3
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
        public DegreeData data;
    }
    public List<StepsForState> PassosPorEstado;
    public Dictionary<string, StepsController> StepsForStateDic = new();
    public TextMeshProUGUI StepText;
    public static System.Action<string> OnInitChangeState;
    public static System.Action OnFinishChangeState;

    public UnityEvent OnCompleteAllStates;
    public UnityEvent OnCompleteAllSteps;

    private void Start() {
        foreach(var stepState in PassosPorEstado)
        {
            StepsController steps = new();
            steps.Steps = stepState.Steps;
            steps.actualStep = StepText;
            
            StepsForStateDic.Add(stepState.StateName, steps);
        }
        StepsForStateDic[EstadoAtual.ToString()].UpdateStep();
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
            OnCompleteAllSteps?.Invoke();
        else if(StepsForStateDic[EstadoAtual.ToString()].IsAllStepCompleted() && IsLastState())
            OnCompleteAllStates?.Invoke();

        StepsForStateDic[EstadoAtual.ToString()].UpdateStep();
    }

    public void UpdateState()
    {
        if(StepsForStateDic[EstadoAtual.ToString()].IsAllStepCompleted())
        {
            SetState(ProximoEstado);
            StepsForStateDic[EstadoAtual.ToString()].UpdateStep();
        } 
    }

    public void SetData(string name, float degree, List<GameObject> lines)
    {
        DegreeData data = PassosPorEstado.Find(d => d.StateName == EstadoAtual.ToString()).data;
        data.Degrees.Add(new DegreeData.DegreeCalculateData(){
            name = name,
            degree = degree,
            lines = lines
        });
    }

    public void SetData(int sacroOption)
    {
        DegreeData data = PassosPorEstado.Find(d => d.StateName == EstadoAtual.ToString()).data;
        data.sacro = DegreeData.SacroTypes.None + sacroOption;
    }



}
