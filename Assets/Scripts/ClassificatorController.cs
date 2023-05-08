using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class ClassificatorController
{
    /// <summary>
    /// 0 = NÃO POSSUI ÂNGULO ACIMA DE 25
    /// 1 = POSSUI ÂNGULO ACIMA DE 25
    /// 2 = CURVA PRINCIPAL
    /// </summary>
    /// <returns>Tipo de curva no formato Tipo de curva (1-6)</returns>
    public static List<TipoCurva> tipoCurvas = new(){
        new(){Tipo = 1, ToracicaProximal=0, ToraricaPrincipal=2, ToracoLombar=0},
        new(){Tipo = 2, ToracicaProximal=1, ToraricaPrincipal=2, ToracoLombar=0},
        new(){Tipo = 3, ToracicaProximal=0, ToraricaPrincipal=2, ToracoLombar=1},
        new(){Tipo = 4, ToracicaProximal=1, ToraricaPrincipal=2, ToracoLombar=1},
        new(){Tipo = 5, ToracicaProximal=0, ToraricaPrincipal=0, ToracoLombar=2},
        new(){Tipo = 6, ToracicaProximal=0, ToraricaPrincipal=1, ToracoLombar=2}
    };

    public static void Classificate(List<ClassificationData> classifications)
    {
        int[] classification = { 0, 0, 0 };
        TipoCurva tipoCurva = new();

        // CALCULANDO FRONTAL
        for (int i = 0; i < classifications[0].classification.Degrees.Count; i++)
        {
            if (classifications[0].classification.Degrees[i] >= 25f)
                classification[i] = 1;
        }

        // CALCULANDO INCLINAÇÕES
        float grauPrincipalEsquerda = classifications[(int)States.LeftInclination].classification.Degrees[0];

        float grauProximalDireita = classifications[(int)States.RightInclination].classification.Degrees[0];
        float grauLombarDireita = classifications[(int)States.RightInclination].classification.Degrees[1];

        if (grauPrincipalEsquerda >= 25f)
            classification[(int)Toracica.Principal] = 1;

        if (grauProximalDireita >= 25f)
            classification[(int)Toracica.Proximal] = 1;

        if (grauLombarDireita >= 25f)
            classification[(int)Toracica.Lombar] = 1;

        if (classifications[(int)States.Front].classification.Degrees[(int)Toracica.Principal] > classifications[(int)States.Front].classification.Degrees[(int)Toracica.Lombar])
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
        float modificadorToracicoSagital = classifications[(int)States.Lateral].classification.Degrees[0];

        // VERIFICANDO MODIFICADOR TORACICO SAGITAL
        string modificadorSagitalToracico = modificadorToracicoSagital < 10f ? "-" : (modificadorToracicoSagital >= 10f && modificadorToracicoSagital <= 40f) ? "N" : "+";
        ClassificationWithSacro clsSacro = classifications[(int)States.Front].classification as ClassificationWithSacro;
        string sacro = clsSacro.Sacro.ToString();
        clsSacro.ClassificationCode = $"{tipoCurva.Tipo}{sacro}{modificadorSagitalToracico}";
        StateInfo stateInfo = new StateInfo()
        {
            title = "Classificação",
            contents = new List<string>(){ clsSacro.ClassificationCode }
        };
        List<StateInfo> serializeInfo = new List<StateInfo>() { stateInfo };
        string serialized = JsonConvert.SerializeObject(serializeInfo, Formatting.Indented);
        File.WriteAllText($"{Application.streamingAssetsPath}/StatesFiles/Report.json", serialized);
        Debug.Log($"classifição: {tipoCurva.Tipo}{sacro}{modificadorSagitalToracico}");
    }

}
