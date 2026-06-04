using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonUtilityManager
{
    // ========== Single Object ==========
    public static string ToJson(object obj, bool prettyPrint = false)
    {
        return JsonUtility.ToJson(obj, prettyPrint);
    }

    public static T FromJson<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }

    public static object FromJson(string json, Type type)
    {
        return JsonUtility.FromJson(json, type);
    }

    // ========== Array or List Support ==========
    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }

    public static string ToJsonArray<T>(T[] array, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T> { array = array };
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    public static T[] FromJsonArray<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.array;
    }

    public static string ToJsonList<T>(List<T> list, bool prettyPrint = false)
    {
        return ToJsonArray(list.ToArray(), prettyPrint);
    }

    public static List<T> FromJsonList<T>(string json)
    {
        return new List<T>(FromJsonArray<T>(json));
    }
}
