using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

// 저장을 위한 형식으로 변환하고, 저장된 데이터를 변환하는 Formatter
public class ScriptDataFormatter
{
    public static void SaveScriptData(ScriptData data, string path, string fileName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string finalPath = Path.Combine(path, fileName + ".scirptData");
        FileStream fs = new FileStream(finalPath, FileMode.Create, FileAccess.Write);

        formatter.Serialize(fs, data.ToJson().ToString());
        
        fs.Close();
    }

    public static void LoadScriptData(out JObject data, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        
        string tmp = (string)formatter.Deserialize(fs);
        data = JObject.Parse(tmp);

        fs.Close();
    }
    public static void LoadScriptData(out ScriptData data, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

        string tmp = (string)formatter.Deserialize(fs);
        JObject obj = JObject.Parse(tmp);
        data = new ScriptData();
        data.ToData(obj);

        fs.Close();
    }
}
