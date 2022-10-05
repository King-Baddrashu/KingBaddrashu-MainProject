using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class ScriptNode
{
    int nodeID;
    string stringContent;
    ScriptType type;
    NPCType npcType;
    NPCStatusType status;
    int nextNodeID;
    Dictionary<int, StateGroup> scriptNodeConnector;
    float[] nodePosition;

    public ScriptNode(int id)
    {
        nodeID = id;
        nextNodeID = -1;
        stringContent = "Default Content";
        type = ScriptType.NORMAL;
        npcType = NPCType.NONE;
        status = NPCStatusType.NORMAL;
        scriptNodeConnector = new Dictionary<int, StateGroup>();
        nodePosition = new float[3];
    }

    public void SetScriptContent(string content) { stringContent = content; }
    public void SetNodePosition(Vector3 pos) { nodePosition[0] = pos.x; nodePosition[1] = pos.y; nodePosition[2] = pos.z; }
    public void SetScriptType(ScriptType type) { this.type = type; }
    public void SetNextNodeID(int id)
    {
        if (type != ScriptType.RESULT)
            nextNodeID = id;
        else
            Debug.LogWarning("[ScriptNode] Unsupported scriptNodeType. Change nodes type to 'ScripType.NORMAL.' or 'ScriptType.OPTIONAL'.");
    }
    public void SetNPCStatus(NPCStatusType status) { this.status = status; }
    public void SetNPCType(NPCType type) { this.npcType = type; }


    public string GetScriptContent() { return stringContent; }
    public ScriptType GetScriptType() { return type; }
    public int GetNodeID() { return nodeID; }
    public Dictionary<int, StateGroup> GetNodeConnector() { return scriptNodeConnector; }
    public Vector3 GetNodePosition() { return new Vector3(nodePosition[0], nodePosition[1], nodePosition[2]); }
    public NPCStatusType GetNPCStatus() { return status; }
    public NPCType GetNPCType() { return npcType; }
    public override string ToString()
    {
        string data;
        data = string.Format($"Node {type}{nodeID}\nPosition : {nodePosition[0]}, {nodePosition[1]}, {nodePosition[2]}\ncontent : {stringContent}\n[ScriptNodeProcessorInfo]\n");
        data += scriptNodeConnector.GetString();

        return data;
    }

    public int GetNextNodeID()
    {
        return nextNodeID;
    }

    public void AddConnection(int nextNodeID)
    {
        if(!scriptNodeConnector.ContainsKey(nextNodeID))
        {
            scriptNodeConnector.Add(nextNodeID, new StateGroup());
        }
    }

    public bool CheckConnection(int nextNodeID)
    {
        return scriptNodeConnector.ContainsKey(nextNodeID);
    }

    // AddState의 Int, Float, String, Bool을 생성함
    public StateObject<int> AddIntState(int nextNodeID, string param, RelationalOper oper, int targetValue)
    {
        return scriptNodeConnector[nextNodeID].AddStateInt(nextNodeID, param, oper, targetValue);
    }
    public StateObject<float> AddFloatState(int nextNodeID, string param, RelationalOper oper, float targetValue)
    {
        return scriptNodeConnector[nextNodeID].AddStateFloat(nextNodeID, param, oper, (float)(object)targetValue);
    }
    public StateObject<string> AddStringState(int nextNodeID, string param, RelationalOper oper, string targetValue)
    {
        return scriptNodeConnector[nextNodeID].AddStateString(nextNodeID, param, oper, targetValue);
    }
    public StateObject<bool> AddBoolState(int nextNodeID, string param, RelationalOper oper, bool targetValue)
    {
        return scriptNodeConnector[nextNodeID].AddStateBool(nextNodeID, param, oper, targetValue);
    }

    public void ChangeState<T>(int stateID, string param, RelationalOper oper, T targetValue)
    {
        string type = typeof(T).Name;
        int nextNodeID = stateID / 1000000;
        if (type.Equals(typeof(int).Name))
        {
            scriptNodeConnector[nextNodeID].ChangeStateInt(stateID, param, oper, (int)(object)targetValue);
        }
        else if (type.Equals(typeof(float).Name))
        {
            scriptNodeConnector[nextNodeID].ChangeStateFloat(stateID, param, oper, (float)(object)targetValue);
        }
        else if (type.Equals(typeof(string).Name))
        {
            scriptNodeConnector[nextNodeID].ChangeStateString(stateID, param, oper, (string)(object)targetValue);
        }
        else if (type.Equals(typeof(bool).Name))
        {
            scriptNodeConnector[nextNodeID].ChangeStateBool(stateID, param, oper, (bool)(object)targetValue);
        }
        else
        {
            Debug.LogError("[ScriptNode] StateManager : Unsupported generic type.");
        }
    }

    public void RemoveState(int stateID)
    {
        scriptNodeConnector[stateID / 1000000].RemoveState(stateID);
    }

    public void RemoveNodeConnect(int nextNodeID)
    {
        if(scriptNodeConnector.ContainsKey(nextNodeID))
        {
            scriptNodeConnector.Remove(nextNodeID);

            if (this.nextNodeID == nextNodeID)
                this.nextNodeID = -1;
        }
    }

    public List<int> GetConnectedNodeID()
    {
        List<int> nodes = new List<int>();

        foreach (var key in GetNodeConnector().Keys)
        {
            nodes.Add(key);
        }

        return nodes;
    }

    public JObject ToJson()
    {
        JObject obj = new JObject();
        obj.Add("nodeID", nodeID);
        obj.Add("stringContent", stringContent);
        obj.Add("type", (int)type);
        obj.Add("npcType", (int)npcType);
        obj.Add("status", (int)status);
        obj.Add("nextNodeID", nextNodeID);
        
        JArray connectorArr = new JArray();
        foreach(var connector in scriptNodeConnector)
        {
            JObject connectorObj = new JObject();
            connectorObj.Add("Key", connector.Key);
            connectorObj.Add("Value", connector.Value.ToJson());
            connectorArr.Add(connectorObj);
        }

        obj.Add("scriptNodeConnector", connectorArr);
        obj.Add("nodePosition", new JArray(nodePosition));

        return obj;
    }

    public void ToData(JObject obj)
    {
        nodeID = (int)obj.GetValue("nodeID");
        stringContent = (string)obj.GetValue("stringContent");
        type = (ScriptType)(int)obj.GetValue("type");
        npcType = (NPCType)(int)obj.GetValue("npcType");
        status = (NPCStatusType)(int)obj.GetValue("status");
        nextNodeID = (int)obj.GetValue("nextNodeID");

        JArray connectorArr = (JArray)obj.GetValue("scriptNodeConnector");
        foreach(JObject connector in connectorArr) {
            StateGroup tmp = new StateGroup();
            tmp.ToData((JObject)connector.GetValue("Value"));
            scriptNodeConnector.Add((int)connector.GetValue("Key"), tmp);
        }

        JArray nodePosition = (JArray)obj.GetValue("nodePosition");
        this.nodePosition[0] = (float)nodePosition[0];
        this.nodePosition[1] = (float)nodePosition[1];
        this.nodePosition[2] = (float)nodePosition[2];
    }
}