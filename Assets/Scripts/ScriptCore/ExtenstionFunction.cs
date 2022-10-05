using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class ExtenstionFunction
{
    public static string GetString<K, V>(this IDictionary<K, V> dict)
    {
        var items = dict.Select(kvp => kvp.ToString());
        return string.Join(", ", items);
    }

    public static string GetString<T>(this IList<T> list)
    {
        var items = list.Select(kvp => kvp.ToString());
        return string.Join(", ", items);
    }

    public static string GetJson<K, V>(this IDictionary<K, V> dict)
    {
        var items = dict.Select(kvp => { 
            return kvp.ToString();
        });
        return string.Join(", ", items);
    }

    public static float GetAngle(Vector3 vStart, Vector3 vEnd)
    {
        Vector3 v = vEnd - vStart;

        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }
}
