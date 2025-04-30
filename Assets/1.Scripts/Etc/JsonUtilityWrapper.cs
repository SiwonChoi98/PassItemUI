using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class JsonUtilityWrapper
{
    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }

    public static List<T> FromJsonList<T>(string json)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{\"Items\":");
        sb.Append(json);
        sb.Append("}");

        string newJson = sb.ToString();
        return JsonUtility.FromJson<Wrapper<T>>(newJson).Items;
    }
}