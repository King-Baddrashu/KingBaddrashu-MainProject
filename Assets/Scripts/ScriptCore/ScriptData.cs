using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ������ id�� ������ ����
// id�� ��� 1�� ������
// �ε��� �� id�� �غ���
// ����Ǿ��ִ� ���� ScriptNode�� Key���� ���� �� �� ����

[System.Serializable]
public class ScriptData
{
    public int lastKey { get; private set; }

    string scriptName;

    public int startNodeID;
    int currentNodeID;
    public Dictionary<int, ScriptNode> scripts;
    public Dictionary<string, int> parameter_Int;
    public Dictionary<string, float> parameter_Float;
    public Dictionary<string, string> parameter_String;
    public Dictionary<string, bool> parameter_Bool;

    public ScriptData()
    {
        lastKey = 0;
        scriptName = "Default - ScriptName";
        startNodeID = 0;
        currentNodeID = 0;

        scripts = new Dictionary<int, ScriptNode>();
        parameter_Int = new Dictionary<string, int>();
        parameter_Float = new Dictionary<string, float>();
        parameter_String = new Dictionary<string, string>();
        parameter_Bool = new Dictionary<string, bool>();
    }

    // ScriptData�� �⺻ ������ �����ϱ� ���� Method
    public void SetScriptName(string name) { scriptName = name; }
    public string GetScriptName() { return scriptName; }


    // �Ķ���� �߰� Int, Float, String, Bool... + �Ķ���� ���� ������Ʈ�� ������ 
    public void AddIntParameter(string name, int initValue = 0) { parameter_Int.Add(name, initValue); }
    public void AddFloatParameter(string name, float initValue = 0) { parameter_Float.Add(name, initValue); }
    public void AddStringParameter(string name, string initValue = "") { parameter_String.Add(name, initValue); }
    public void AddBoolParameter(string name, bool initValue = false) { parameter_Bool.Add(name, initValue); }

    // �Ķ���� �� ����
    public void SetInt(string name, int value) { 
        parameter_Int[name] = value;
    }
    public void SetFloat(string name, float value) { parameter_Float[name] = value; }
    public void SetString(string name, string value) { parameter_String[name] = value; }
    public void SetBool(string name, bool value) { parameter_Bool[name] = value; }

    // �Ķ���� ����
    public void RemoveIntParameter(string name) {
        if (parameter_Int.ContainsKey(name)) 
            parameter_Int.Remove(name);
        else 
            Debug.Log($"[ScriptNode] The key [{name}] does not exist.");
    }
    public void RemoveFloatParameter(string name) { 
        if (parameter_Float.ContainsKey(name)) 
            parameter_Float.Remove(name); 
        else Debug.Log($"[ScriptNode] The key [{name}] does not exist.");
    }
    public void RemoveStringParameter(string name) {
        if (parameter_String.ContainsKey(name)) 
            parameter_String.Remove(name); 
        else Debug.Log($"[ScriptNode] The key [{name}] does not exist."); 
    }
    public void RemoveBoolParameter(string name) { 
        if (parameter_Bool.ContainsKey(name)) 
            parameter_Bool.Remove(name); 
        else 
            Debug.Log($"[ScriptNode] The key [{name}] does not exist.");
    }

    // ���� ��带 �����ϴ� �޼���
    public void SetStartNode(int id)
    {
        if(scripts.ContainsKey(id))
            startNodeID = id;
        else
            Debug.Log($"[ScriptNode] The key [{id}] does not exist.");
    }

    // ScriptNode�� �߰��ϴ� �޼���
    public ScriptNode AddNode(string content, ScriptType type)
    {
        return AddNode(content, type, Vector3.zero);
    }
    public ScriptNode AddNode(string content, ScriptType type, Vector3 pos)
    {
        ScriptNode node = new ScriptNode(lastKey++);
        node.SetNodePosition(pos);
        node.SetScriptContent(content);
        node.SetScriptType(type);

        scripts.Add(node.GetNodeID(), node);

        return node;
    }

    // ��带 ��������ش�.
    // ���� �Ϲ����� �����, �ٷ� ���� ���� ��������ش�.
    public void ConnectNode(int fromNodeID, int toNodeID)
    {
        if (!scripts.ContainsKey(fromNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{fromNodeID}] dose not exist.");
            return;
        }
        if (!scripts.ContainsKey(toNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{toNodeID}] dose not exist.");
            return;
        }

        scripts[fromNodeID].AddConnection(toNodeID);
        if(scripts[fromNodeID].GetNextNodeID() != -1)
        {
            Debug.LogWarning($"[ScriptData] This node already has next node. The next node of this node is changed to the most recently connected node [ID : {toNodeID}]");
        }

        scripts[fromNodeID].SetNextNodeID(toNodeID);
    }

    // ����� ��忡 ������ �߰��Ѵ�. ���� ����Ǿ� ���� �ʴٸ� ��带 �����Ѵ�.
    public StateObject<int> AddConnectionIntState(int fromNodeID, int toNodeID, string param, RelationalOper oper, int targetValue)
    {
        if (!scripts.ContainsKey(fromNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{fromNodeID}] dose not exist.");
            return null;
        }
        if (!scripts.ContainsKey(toNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{toNodeID}] dose not exist.");
            return null;
        }

        ScriptNode fromNode = scripts[fromNodeID];

        if (!fromNode.CheckConnection(toNodeID))
        {
            ConnectNode(fromNodeID, toNodeID);
        }

        return fromNode.AddIntState(toNodeID, param, oper, targetValue);
    }
    public StateObject<float> AddConnectionFloatState(int fromNodeID, int toNodeID, string param, RelationalOper oper, float targetValue)
    {
        if (!scripts.ContainsKey(fromNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{fromNodeID}] dose not exist.");
            return null;
        }
        if (!scripts.ContainsKey(toNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{toNodeID}] dose not exist.");
            return null;
        }

        ScriptNode fromNode = scripts[fromNodeID];

        if (!fromNode.CheckConnection(toNodeID))
        {
            ConnectNode(fromNodeID, toNodeID);
        }

        return fromNode.AddFloatState(toNodeID, param, oper, targetValue);
    }
    public StateObject<string> AddConnectionStringState(int fromNodeID, int toNodeID, string param, RelationalOper oper, string targetValue)
    {
        if (!scripts.ContainsKey(fromNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{fromNodeID}] dose not exist.");
            return null;
        }
        if (!scripts.ContainsKey(toNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{toNodeID}] dose not exist.");
            return null;
        }

        ScriptNode fromNode = scripts[fromNodeID];

        if (!fromNode.CheckConnection(toNodeID))
        {
            ConnectNode(fromNodeID, toNodeID);
        }

        return fromNode.AddStringState(toNodeID, param, oper, targetValue);
    }
    public StateObject<bool> AddConnectionBoolState(int fromNodeID, int toNodeID, string param, RelationalOper oper, bool targetValue)
    {
        if (!scripts.ContainsKey(fromNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{fromNodeID}] dose not exist.");
            return null;
        }
        if (!scripts.ContainsKey(toNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{toNodeID}] dose not exist.");
            return null;
        }

        ScriptNode fromNode = scripts[fromNodeID];

        if (!fromNode.CheckConnection(toNodeID))
        {
            ConnectNode(fromNodeID, toNodeID);
        }

        if (oper != RelationalOper.EQUALS && oper != RelationalOper.EQUALS)
        {
            Debug.LogWarning($"[ScriptData] RelationOper : The Boolean type can not operate {oper}.");

            oper = RelationalOper.EQUALS;
        }

        return fromNode.AddBoolState(toNodeID, param, oper, targetValue);
    }
    public void ChangeConnectionIntState(int fromNodeID, int stateID, string param, RelationalOper oper, int targetValue)
    {
        if (!scripts.ContainsKey(fromNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{fromNodeID}] dose not exist.");
            return;
        }

        scripts[fromNodeID].ChangeState<int>(stateID, param, oper, targetValue);
    }
    public void ChangeConnectionFloatState(int fromNodeID, int stateID, string param, RelationalOper oper, float targetValue)
    {
        if (!scripts.ContainsKey(fromNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{fromNodeID}] dose not exist.");
            return;
        }

        scripts[fromNodeID].ChangeState<float>(stateID, param, oper, targetValue);
    }
    public void ChangeConnectionStringState(int fromNodeID, int stateID, string param, RelationalOper oper, string targetValue)
    {
        if (!scripts.ContainsKey(fromNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{fromNodeID}] dose not exist.");
            return;
        }

        scripts[fromNodeID].ChangeState<string>(stateID, param, oper, targetValue);
    }
    public void ChangeConnectionBoolState(int fromNodeID, int stateID, string param, RelationalOper oper, bool targetValue)
    {
        if (!scripts.ContainsKey(fromNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{fromNodeID}] dose not exist.");
            return;
        }

        scripts[fromNodeID].ChangeState<bool>(stateID, param, oper, targetValue);
    }
    // Status �ϳ��� �����ϴ� �ڵ�
    public void RemoveStatus(int nodeID, int statusID)
    {
        if (!scripts.ContainsKey(nodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{nodeID}] dose not exist.");
            return;
        }

        scripts[nodeID].RemoveState(statusID);
    }

    // form���� to�� ���� Connection�� ���´�.
    public void RemoveConnection(int fromNodeID, int toNodeID)
    {
        if (!scripts.ContainsKey(fromNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{fromNodeID}] dose not exist.");
            return;
        }
        if (!scripts.ContainsKey(toNodeID))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{toNodeID}] dose not exist.");
            return;
        }

        ScriptNode fromNode = scripts[fromNodeID];

        fromNode.RemoveNodeConnect(toNodeID);

        return;
    }
    // ScriptNode�� �����ϴ� �޼���, �ش� ���� ����� ��� Connection�� ������.
    public void RemoveScriptNode(int nodeID)
    {
        if (scripts.ContainsKey(nodeID))
        {
            foreach (var pair in scripts)
            {
                ScriptNode node = pair.Value;
                node.RemoveNodeConnect(nodeID);
            }

            scripts.Remove(nodeID);
        }
        else
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{nodeID}] dose not exist.");
        }
    }

    // -= ��Ÿ�ӽ� �ʿ��� �޼��� =-
    // ����� ID�� �ʱ�ȭ ���ִ� �޼���
    public void StartScript()
    {
        currentNodeID = startNodeID;
    }

    // ���� ScriptNode�� ���� ��带 ã�� �޼���
    public void CaculateNextNode()
    {
        ScriptNode currentNode = scripts[currentNodeID];
        if(currentNode.GetScriptType() != ScriptType.OPTIONAL)
        {
            Debug.LogWarning("This Function is only support 'ScriptType.OPTIONAL'. If you want use this function, then you must change this script node type to 'ScriptType.OPTIONAL'");
            return;
        }
        Dictionary<int, StateGroup> currntNodeGroups = currentNode.GetNodeConnector();

        bool check = false;
        foreach(var stateGroups in currntNodeGroups)
        {
            var stateGroup = stateGroups.Value;

            foreach (var param in parameter_Int)
            {
                ResultInfo resultInfo = stateGroup.ChecktIntStateObject(param.Key, param.Value);
                switch (resultInfo)
                {
                    case ResultInfo.OK:
                        check = true;
                        break;
                    case ResultInfo.FALSE:
                        check = false;
                        break;
                }

                if (resultInfo == ResultInfo.NOT_FOUND)
                {
                    check = false;
                    continue;
                }

                break;
            }

            foreach (var param in parameter_Float)
            {
                ResultInfo resultInfo = stateGroup.ChecktFloatStateObject(param.Key, param.Value);
                switch (resultInfo)
                {
                    case ResultInfo.OK:
                        check = true;
                        break;
                    case ResultInfo.FALSE:
                        check = false;
                        break;
                }

                if (resultInfo == ResultInfo.NOT_FOUND)
                {
                    check = false;
                    continue;
                }

                break;
            }

            foreach (var param in parameter_String)
            {
                ResultInfo resultInfo = stateGroup.ChecktStringStateObject(param.Key, param.Value);
                switch (resultInfo)
                {
                    case ResultInfo.OK:
                        check = true;
                        break;
                    case ResultInfo.FALSE:
                        check = false;
                        break;
                }

                if (resultInfo == ResultInfo.NOT_FOUND)
                {
                    check = false;
                    continue;
                }

                break;
            }

            foreach (var param in parameter_Bool)
            {
                ResultInfo resultInfo = stateGroup.ChecktBoolStateObject(param.Key, param.Value);
                switch (resultInfo)
                {
                    case ResultInfo.OK:
                        check = true;
                        break;
                    case ResultInfo.FALSE:
                        check = false;
                        break;
                }

                if (resultInfo == ResultInfo.NOT_FOUND)
                {
                    check = false;
                    continue;
                }

                break;
            }

            if(check)
            {
                currentNode.SetNextNodeID(stateGroups.Key);
                break;
            }
        }

        if(currentNode.GetNextNodeID() == -1)
        {
            Debug.LogWarning("[ScriptData] All Parameter Value is not satisfied for this node. Please Check this scriptdata parameter value.");
        }
    }

    // ���� ����� ���� ���� ������.
    public int GetCurrentNextNode()
    {
        ScriptNode currentNode = scripts[currentNodeID];

        if(currentNode.GetNextNodeID() == -1)
        {
            switch(currentNode.GetScriptType())
            {
                case ScriptType.OPTIONAL:
                    CaculateNextNode();

                    if (currentNode.GetNextNodeID() == -1)
                    {
                        Debug.LogWarning("[ScriptData] Can not get next script node.");
                        return -1;
                    }
                    else
                    {
                        return currentNode.GetNextNodeID();
                    }
                case ScriptType.RESULT:
                    Debug.LogWarning("[ScriptData] This node type is 'ScriptType.RESULT'.  ");
                    return -1;
                case ScriptType.NORMAL:
                    Debug.LogWarning("[ScriptData] This node is not connected other nodes. Check your Script Data or Script Node logic");
                    return -1;
                default:
                    Debug.LogError("[ScriptData] Unsupported scriptNodeType.");
                    return -1;
            }
        }

        return currentNode.GetNextNodeID();
    }

    // ��带 ���������� ������.
    public void ChangeCurrentNode(int nodeid)
    {
        if(scripts.ContainsKey(nodeid))
        {
            currentNodeID = nodeid;
        }

        else
            Debug.LogError($"[ScriptData] This node ID({nodeid}) is not exisited.");
    }

    // ���� ��带 ã�� �����Ѵ�.
    public int GetAndChangeCurrentToNextNode()
    {
        int next = GetCurrentNextNode();
        if (next != -1)
        {
            ChangeCurrentNode(next);
        }

        return next;
    }

    public override string ToString()
    {
        string data = "";
        data += string.Format($"Script Name : {scriptName} \n");
        data += string.Format($"Last Key : {lastKey} \n");
        data += string.Format($"Start Node ID : {startNodeID}\n");
        data += string.Format($"[Script Node Datas] \n{scripts.GetString()}\n");
        data += string.Format($"[Parameter Datas]\n int : {parameter_Int.GetString()} \n");
        data += string.Format($"float : {parameter_Float.GetString()} \n");
        data += string.Format($"string : {parameter_String.GetString()} \n");
        data += string.Format($"bool : {parameter_Bool.GetString()} \n");

        return data;
    }

    public List<ScriptNode> GetConnectedNode(int id)
    {
        if (!scripts.ContainsKey(id))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{id}] dose not exist.");
            return null;
        }

        var targetNode = scripts[id];
        List<ScriptNode> nodes = new List<ScriptNode>();

        foreach(var key in targetNode.GetNodeConnector().Keys)
        {
            nodes.Add(scripts[key]);
        }

        return nodes;
    }

    public ScriptNode GetNode(int id)
    {
        if (!scripts.ContainsKey(id))
        {
            Debug.LogError($"[ScriptData] ScriptNodes : The key [{id}] dose not exist.");
            return null;
        }

        return scripts[id];
    }

    public ScriptNode GetCurrentNode()
    {
        return GetNode(currentNodeID);
    }

    public int GetParameterCount()
    {
        return parameter_Int.Count + parameter_Float.Count + parameter_String.Count + parameter_Bool.Count;
    }

    public JObject ToJson()
    {
        JObject obj = new JObject();
        obj.Add("lastKey", lastKey);
        obj.Add("scriptName", scriptName);
        obj.Add("startNodeID", startNodeID);
        obj.Add("currentNodeID", currentNodeID);
        
        JArray scriptsArray= new JArray();
        foreach(var node in scripts)
        {
            JObject nodeObj = new JObject();
            nodeObj.Add("key", node.Key);
            nodeObj.Add("value", node.Value.ToJson());
            scriptsArray.Add(nodeObj);
        }

        JArray paramIntArray = new JArray();
        foreach (var parm in parameter_Int)
        {
            JObject paramObj = new JObject();
            paramObj.Add("key", parm.Key);
            paramObj.Add("value", parm.Value);
            paramIntArray.Add(paramObj);
        }

        JArray paramFloatArray = new JArray();
        foreach (var parm in parameter_Float)
        {
            JObject paramObj = new JObject();
            paramObj.Add("key", parm.Key);
            paramObj.Add("value", parm.Value);
            paramFloatArray.Add(paramObj);
        }

        JArray paramStringArray = new JArray();
        foreach (var parm in parameter_String)
        {
            JObject paramObj = new JObject();
            paramObj.Add("key", parm.Key);
            paramObj.Add("value", parm.Value);
            paramStringArray.Add(paramObj);
        }

        JArray paramBoolArray = new JArray();
        foreach (var parm in parameter_Bool)
        {
            JObject paramObj = new JObject();
            paramObj.Add("key", parm.Key);
            paramObj.Add("value", parm.Value);
            paramBoolArray.Add(paramObj);
        }

        obj.Add("scripts", scriptsArray);
        obj.Add("parameter_Int", paramIntArray);
        obj.Add("parameter_Float", paramFloatArray);
        obj.Add("parameter_String", paramStringArray);
        obj.Add("parameter_Bool", paramBoolArray);

        return obj;
    }

    public void ToData(JObject obj)
    {
        lastKey = (int)obj.GetValue("lastKey");
        scriptName = (string)obj.GetValue("scriptName");
        startNodeID = (int)obj.GetValue("startNodeID");
        currentNodeID = (int)obj.GetValue("currentNodeID");

        JArray scriptsArray = (JArray)obj.GetValue("scripts");
        JArray paramIntArray = (JArray)obj.GetValue("parameter_Int");
        JArray paramFloatArray = (JArray)obj.GetValue("parameter_Float");
        JArray paramStringArray = (JArray)obj.GetValue("parameter_String");
        JArray paramBoolArray = (JArray)obj.GetValue("parameter_Bool");

        parameter_Int = new Dictionary<string, int>();
        parameter_Float = new Dictionary<string, float>();
        parameter_String = new Dictionary<string, string>();
        parameter_Bool = new Dictionary<string, bool>();

        foreach (JObject script in scriptsArray)
        {
            ScriptNode node = new ScriptNode((int)script.GetValue("key"));
            node.ToData((JObject)script.GetValue("value"));
            scripts.Add(node.GetNodeID(), node);
        }
        foreach (JObject param in paramIntArray)
        {
            parameter_Int.Add((string)param.GetValue("key"), (int)param.GetValue("value"));
        }
        foreach (JObject param in paramFloatArray)
        {
            parameter_Float.Add((string)param.GetValue("key"), (float)param.GetValue("value"));
        }
        foreach (JObject param in paramStringArray)
        {
            parameter_String.Add((string)param.GetValue("key"), (string)param.GetValue("value"));
        }
        foreach (JObject param in paramBoolArray)
        {
            parameter_Bool.Add((string)param.GetValue("key"), (bool)param.GetValue("value"));
        }
    }
}