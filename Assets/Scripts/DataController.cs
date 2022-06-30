using System.Collections.Generic;
using UnityEditor;

public static class DataController
{
    public static DegreeData CreateDegreeData(Dictionary<string, float> valuesDict)
    {
        DegreeData data = new();
        foreach(var val in valuesDict)
        {
            var values = new DegreeData.DegreeCalculateData()
            {
                name = val.Key,
                degree = val.Value
            };
            data.Degrees.Add(values);
        }
        return data;
    }

    public static void CreateDegreeDataAsset(DegreeData data, string name)
    {
        string path = $"Assets/ScriptableObjects/{name}.asset";
        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
    }
}
