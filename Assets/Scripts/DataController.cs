using System.Collections.Generic;

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
}
