using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class ClassificatorController
{
    public static Dictionary<string, string> CodeClassifications = new Dictionary<string, string>();
    /// <summary>
    /// 0 = NÃO POSSUI ÂNGULO ACIMA DE 25
    /// 1 = POSSUI ÂNGULO ACIMA DE 25
    /// </summary>
    /// <returns>Tipo de curva no formato Tipo de curva (1-6)</returns>
    public static List<TipoCurva> tipoCurvas = new(){
        new(){Tipo = 1, ToracicaProximal=0, ToraricaPrincipal=1, ToracoLombar=0, PrincipalIsPrincipal=true},
        new(){Tipo = 2, ToracicaProximal=1, ToraricaPrincipal=1, ToracoLombar=0, PrincipalIsPrincipal=true},
        new(){Tipo = 3, ToracicaProximal=0, ToraricaPrincipal=1, ToracoLombar=1, PrincipalIsPrincipal=true},
        new(){Tipo = 4, ToracicaProximal=1, ToraricaPrincipal=1, ToracoLombar=1, PrincipalIsPrincipal=true},
        new(){Tipo = 5, ToracicaProximal=0, ToraricaPrincipal=0, ToracoLombar=1, PrincipalIsPrincipal=false},
        new(){Tipo = 6, ToracicaProximal=0, ToraricaPrincipal=1, ToracoLombar=1, PrincipalIsPrincipal=false}
    };
    // Proximal = 0
    // Principal = 2
    // Lombar = 1

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
        else
            classification[(int)Toracica.Principal] = 0;

        if (grauProximalDireita >= 25f)
            classification[(int)Toracica.Proximal] = 1;
        else
            classification[(int)Toracica.Proximal] = 0;

        if (grauLombarDireita >= 25f)
            classification[(int)Toracica.Lombar] = 1;
        else
            classification[(int)Toracica.Lombar] = 0;


        bool PrincipalIsPrincipal = true;

        if (classifications[(int)States.Front].classification.Degrees[(int)Toracica.Principal] < classifications[(int)States.Front].classification.Degrees[(int)Toracica.Lombar])
        {
            PrincipalIsPrincipal = false;
            classification[(int)Toracica.Lombar] = 1;
        }
        else
        {
            classification[(int)Toracica.Principal] = 1;
        }


        tipoCurva = tipoCurvas.Find(curva =>
                (
                    curva.ToracicaProximal == classification[0] &&
                    curva.ToraricaPrincipal == classification[1] &&
                    curva.ToracoLombar == classification[2] &&
                    curva.PrincipalIsPrincipal == PrincipalIsPrincipal
                )
            );
        float modificadorToracicoSagital = classifications[(int)States.Lateral].classification.Degrees[0];

        // VERIFICANDO MODIFICADOR TORACICO SAGITAL
        string modificadorSagitalToracico = modificadorToracicoSagital < 10f ? "-" : (modificadorToracicoSagital >= 10f && modificadorToracicoSagital <= 40f) ? "N" : "+";
        ClassificationWithSacro clsSacro = classifications[(int)States.Front].classification as ClassificationWithSacro;
        string sacro = clsSacro.Sacro.ToString();

        string code = $"{tipoCurva.Tipo}{sacro}{modificadorSagitalToracico}";
        SaveClassificationCode(code);
        Debug.Log($"classifição: {tipoCurva.Tipo}{sacro}{modificadorSagitalToracico}");
    }

    private static void SaveClassificationCode(string code)
    {
        File.WriteAllText($"{ClassificationFolder.DefinedFolderFilePath}/classification_code", code, System.Text.Encoding.UTF8);
        ClassificatorController.CodeClassifications.Add(ClassificationFolder.FolderName, code);
    }

    public static void LoadCodeClassifications()
    {
        foreach(string dir in Directory.GetDirectories(ClassificationFolder.SaveDataFolder+"/Reports/"))
        {
            string file = $"{dir}/classification_code";
            if(File.Exists(file))
            {
                string code = File.ReadAllText(file);
                CodeClassifications.Add((new DirectoryInfo(dir)).Name, code);
            }
        }
    }
}
