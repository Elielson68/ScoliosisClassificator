using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ProgressController : MonoBehaviour
{
    [Serializable]
    public enum Estados
    {
        Frontal = 0,
        InclinacaoEsquerda = 1,
        InclinacaoDireita = 2,
        Lateral = 3
    }
    [SerializeField] private Estados _estadoAtual; 

    public Estados EstadoAtual {get => _estadoAtual; private set => _estadoAtual = value;}

    public int ActualStep;

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

    private void OnEnable() 
    {
        StepsForStateDic.Clear();
        foreach(var stepState in PassosPorEstado)
        {
            StepsController steps = new();
            steps.Steps = stepState.Steps;
            steps.actualStep = StepText;
            
            StepsForStateDic.Add(stepState.StateName, steps);
            stepState.data?.Reset();
        }
        ResetProgress();
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
        ActualStep = StepsForStateDic[EstadoAtual.ToString()].IndexActualStep;
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
        ActualStep = StepsForStateDic[EstadoAtual.ToString()].IndexActualStep;
    }

    public void UpdateState()
    {
        if(StepsForStateDic[EstadoAtual.ToString()].IsAllStepCompleted())
        {
            SetState(ProximoEstado);
            StepsForStateDic[EstadoAtual.ToString()].UpdateStep();
        } 
        ActualStep = StepsForStateDic[EstadoAtual.ToString()].IndexActualStep;
    }

    public void SetData(string name, float degree, List<DegreeData.Line> lines)
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

    public void ResetProgress()
    {
        EstadoAtual = Estados.Frontal;
        foreach(var step in StepsForStateDic)
        {
            step.Value.ResetStep();
        }
        ActualStep = StepsForStateDic[EstadoAtual.ToString()].IndexActualStep;
    }


}
