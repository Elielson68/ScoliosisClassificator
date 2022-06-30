using System.Collections;
using System.Collections.Generic;
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

    public void SetState(Estados novoEstado)
    {
        foreach(var est in EstadosAnim)
        {
            if(est.name == EstadoAtual.ToString())
                est.SetBool("EstadoCompleto", true);
        }
        EstadoAtual = novoEstado;
    }
}
