using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ClassificatorController : MonoBehaviour
{

    public enum Estados
    {
        Frontal = 0,
        InclinacaoEsquerda = 1,
        InclinacaoDireita = 2,
        Lateral = 3
    }
    private enum Toracica
    {
        Proximal = 0,
        Principal = 1,
        Lombar = 2
    }
    public List<DegreeData> degreeDatas;

    public TextMeshProUGUI classificationText;

    public struct TipoCurva
    {
        public int Tipo;
        public int ToracicaProximal;
        public int ToraricaPrincipal;
        public int ToracoLombar;
    }
    public List<TipoCurva> tipoCurvas = new(){
        new(){Tipo = 1, ToracicaProximal=0, ToraricaPrincipal=2, ToracoLombar=0},
        new(){Tipo = 2, ToracicaProximal=1, ToraricaPrincipal=2, ToracoLombar=0},
        new(){Tipo = 3, ToracicaProximal=0, ToraricaPrincipal=2, ToracoLombar=1},
        new(){Tipo = 4, ToracicaProximal=1, ToraricaPrincipal=2, ToracoLombar=1},
        new(){Tipo = 5, ToracicaProximal=0, ToraricaPrincipal=0, ToracoLombar=2},
        new(){Tipo = 6, ToracicaProximal=0, ToraricaPrincipal=1, ToracoLombar=2}
    };

    private void Start()
    {
        Classificate();
    }

    private void Classificate()
    {
        int[] classification = {0,0,0};
        TipoCurva tipoCurva = new();
        for(int i=0; i < (int) Estados.InclinacaoDireita + 1; i++)
        {
            for(int ii=0; ii < (int) Toracica.Lombar + 1; ii++)
            {
                if(degreeDatas[i].Degrees[ii].degree >= 25f)
                    classification[ii] = 1;
            }
        }

        if(degreeDatas[(int) Estados.Frontal].Degrees[(int) Toracica.Principal].degree > degreeDatas[(int) Estados.Frontal].Degrees[(int) Toracica.Lombar].degree)
            classification[1] += 1;
        else
            classification[2] += 1;

        tipoCurva = tipoCurvas.Find(curva => 
                (
                    curva.ToracicaProximal == classification[0] &&
                    curva.ToraricaPrincipal == classification[1] &&
                    curva.ToracoLombar == classification[2]
                )
            );
        float modificadorToracicoSagital = degreeDatas[(int) Estados.Lateral].Degrees[0].degree;
        string modificadorSagitalToracico =  modificadorToracicoSagital < 10f ? "-" : (modificadorToracicoSagital >= 10f && modificadorToracicoSagital <= 40f) ? "N" : "+";
        string sacro = degreeDatas[(int) Estados.Frontal].sacro.ToString();
        classificationText.text = $"A classificação é do tipo:\n\n{tipoCurva.Tipo}{sacro}{modificadorSagitalToracico} | classificaiton: {classification[0]} {classification[1]} {classification[2]}";
        foreach(DegreeData data in degreeDatas)
            data.Reset();
    }

}
