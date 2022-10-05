using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class PathExtenstion
{
    static public string defaultPath = Application.persistentDataPath;
    public static string GetScriptPath(string fileName)
    {
        string path = Path.Combine(defaultPath, "Scripts");
        path = Path.Combine(path, fileName);

        return path;
    }
}
