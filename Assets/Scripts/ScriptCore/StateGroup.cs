using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;


[System.Serializable]
public class StateGroup
{
    public int idCount;
    public List<StateObject<int>> stateType_int;
    public List<StateObject<float>> stateType_float;
    public List<StateObject<string>> stateType_string;
    public List<StateObject<bool>> stateType_bool;

    public StateGroup()
    {
        idCount = 0;
        stateType_int = new List<StateObject<int>>();
        stateType_float = new List<StateObject<float>>();
        stateType_string = new List<StateObject<string>>();
        stateType_bool = new List<StateObject<bool>>();
    }

    public StateObject<int> AddStateInt(int nextNodeID, string targetParamName, RelationalOper oper, int targetValue)
    {
        int id = 1000000 * nextNodeID + 10000 * 1 + idCount++;

        StateObject<int> state = new StateObject<int>(id);
        state.SetTargetValue(targetValue);
        state.SetRelationalOper(oper);
        state.SetTargetParameterName(targetParamName);
        stateType_int.Add(state);

        return state;
    }
    public StateObject<float> AddStateFloat(int nextNodeID, string targetParamName, RelationalOper oper, float targetValue)
    {
        int id = 1000000 * nextNodeID + 10000 * 2 + idCount++;

        StateObject<float> state = new StateObject<float>(id);
        state.SetTargetValue(targetValue);
        state.SetRelationalOper(oper);
        state.SetTargetParameterName(targetParamName);

        stateType_float.Add(state);

        return state;
    }
    public StateObject<string> AddStateString(int nextNodeID, string targetParamName, RelationalOper oper, string targetValue)
    {
        int id = 1000000 * nextNodeID + 10000 * 3 + idCount++;

        StateObject<string> state = new StateObject<string>(stateType_string.Count + 1);
        state.SetTargetValue(targetValue);
        state.SetRelationalOper(oper);
        state.SetTargetParameterName(targetParamName);

        stateType_string.Add(state);

        return state;
    }
    public StateObject<bool> AddStateBool(int nextNodeID, string targetParamName, RelationalOper oper, bool targetValue)
    {
        int id = 1000000 * nextNodeID + 10000 * 4 + idCount++;

        StateObject<bool> state = new StateObject<bool>(stateType_bool.Count + 1);
        state.SetTargetValue(targetValue);
        state.SetRelationalOper(oper);
        state.SetTargetParameterName(targetParamName);

        stateType_bool.Add(state);

        return state;
    }
    public void ChangeStateInt(int stateID, string targetParamName, RelationalOper oper, int targetValue)
    {
        foreach (var stateObj in stateType_int)
        {
            if (stateObj.GetObjectID() == stateID)
            {
                stateObj.SetTargetParameterName(targetParamName);
                stateObj.SetRelationalOper(oper);
                stateObj.SetTargetValue(targetValue);
                return;
            }
        }
    }
    public void ChangeStateFloat(int stateID, string targetParamName, RelationalOper oper, float targetValue)
    {
        foreach (var stateObj in stateType_float)
        {
            if (stateObj.GetObjectID() == stateID)
            {
                stateObj.SetTargetParameterName(targetParamName);
                stateObj.SetRelationalOper(oper);
                stateObj.SetTargetValue(targetValue);
            }
        }
    }
    public void ChangeStateString(int stateID, string targetParamName, RelationalOper oper, string targetValue)
    {
        foreach (var stateObj in stateType_string)
        {
            if (stateObj.GetObjectID() == stateID)
            {
                stateObj.SetTargetParameterName(targetParamName);
                stateObj.SetRelationalOper(oper);
                stateObj.SetTargetValue(targetValue);
            }
        }
    }
    public void ChangeStateBool(int stateID, string targetParamName, RelationalOper oper, bool targetValue)
    {
        foreach (var stateObj in stateType_bool)
        {
            if (stateObj.GetObjectID() == stateID)
            {
                stateObj.SetTargetParameterName(targetParamName);
                stateObj.SetRelationalOper(oper);
                stateObj.SetTargetValue(targetValue);
            }
        }
    }
    public void RemoveState(int stateID)
    {
        foreach(var s in stateType_int)
        {
            if(s.GetObjectID() == stateID)
            {
                stateType_int.Remove(s);
                return;
            }
        }
        foreach (var s in stateType_float)
        {
            if (s.GetObjectID() == stateID)
            {
                stateType_float.Remove(s);
                return;
            }
        }
        foreach (var s in stateType_string)
        {
            if (s.GetObjectID() == stateID)
            {
                stateType_string.Remove(s);
                return;
            }
        }
        foreach (var s in stateType_bool)
        {
            if (s.GetObjectID() == stateID)
            {
                stateType_bool.Remove(s);
                return;
            }
        }

        Debug.LogError("[StateGroup] Remove State : The State Object ID dose not exist.");
    }
    public bool CheckParameterAdded<T>(string param)
    {
        string type = typeof(T).Name;

        if (type.Equals(typeof(int).Name))
        {
            foreach(var state in stateType_int)
            {
                if(state.GetTargetParameterName().Equals(param))
                {
                    return true;
                }
            }
            return false;
        }
        else if (type.Equals(typeof(float).Name))
        {
            foreach (var state in stateType_float)
            {
                if (state.GetTargetParameterName().Equals(param))
                {
                    return true;
                }
            }
            return false;
        }
        else if (type.Equals(typeof(string).Name))
        {
            foreach (var state in stateType_string)
            {
                if (state.GetTargetParameterName().Equals(param))
                {
                    return true;
                }
            }
            return false;
        }
        else if (type.Equals(typeof(bool).Name))
        {
            foreach (var state in stateType_bool)
            {
                if (state.GetTargetParameterName().Equals(param))
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            Debug.LogError("[StateGroup] CheckParameterAdded : Unsupported generic type.");

            return false;
        }
    }

    public ResultInfo ChecktIntStateObject(string param, int value)
    {
        bool result = false;
        foreach (var state in stateType_int)
        {
            if(state.GetTargetParameterName().Equals(param))
            {
                result = state.CompareStateValue(value);

                if (result) return ResultInfo.OK;
                else return ResultInfo.FALSE;
            }
        }

        return ResultInfo.NOT_FOUND;
    }
    public ResultInfo ChecktFloatStateObject(string param, float value)
    {
        bool result = false;
        foreach (var state in stateType_float)
        {
            if (state.GetTargetParameterName().Equals(param))
            {
                result = state.CompareStateValue(value);

                if (result) return ResultInfo.OK;
                else return ResultInfo.FALSE;
            }
        }

        return ResultInfo.NOT_FOUND;
    }
    public ResultInfo ChecktStringStateObject(string param, string value)
    {
        bool result = false;
        foreach (var state in stateType_string)
        {
            if (state.GetTargetParameterName().Equals(param))
            {
                result = state.CompareStateValue(value);

                if (result) return ResultInfo.OK;
                else return ResultInfo.FALSE;
            }
        }

        return ResultInfo.NOT_FOUND;
    }
    public ResultInfo ChecktBoolStateObject(string param, bool value)
    {
        bool result = false;
        foreach (var state in stateType_bool)
        {
            if (state.GetTargetParameterName().Equals(param))
            {
                result = state.CompareStateValue(value);

                if (result) return ResultInfo.OK;
                else return ResultInfo.FALSE;
            }
        }

        return ResultInfo.NOT_FOUND;
    }
    public override string ToString()
    {
        string tmp = "";
        if(stateType_int.Count > 0)
            tmp += stateType_int.GetString() + "\n";
        if(stateType_float.Count > 0)
            tmp += stateType_float.GetString() + "\n";
        if(stateType_string.Count > 0)
            tmp += stateType_string.GetString() + "\n";
        if(stateType_bool.Count > 0)
            tmp += stateType_bool.GetString() + "\n";

        return tmp;
    }

    public JObject ToJson()
    {
        JObject obj = new JObject();

        JArray intArr = new JArray();
        foreach (var state in stateType_int)
        {
            JObject stateObj = state.ToJson();
            stateObj.Add("targetValue", state.GetTargetValue());
            intArr.Add(stateObj);
        }
        JArray floatArr = new JArray();
        foreach (var state in stateType_float)
        {
            JObject stateObj = state.ToJson();
            stateObj.Add("targetValue", state.GetTargetValue());
            floatArr.Add(stateObj);
        }
        JArray stringArr = new JArray();
        foreach (var state in stateType_string)
        {
            JObject stateObj = state.ToJson();
            stateObj.Add("targetValue", state.GetTargetValue());
            stringArr.Add(stateObj);
        }
        JArray boolArr = new JArray();
        foreach (var state in stateType_bool)
        {
            JObject stateObj = state.ToJson();
            stateObj.Add("targetValue", state.GetTargetValue());
            boolArr.Add(stateObj);
        }

        obj.Add("idCount", idCount);
        obj.Add("stateType_int", intArr);
        obj.Add("stateType_float", floatArr);
        obj.Add("stateType_string", stringArr);
        obj.Add("stateType_bool", boolArr);

        return obj;
    }

    public void ToData(JObject obj)
    {
        JArray intArr = (JArray)obj.GetValue("stateType_int");
        JArray floatArr = (JArray)obj.GetValue("stateType_float");
        JArray stringArr = (JArray)obj.GetValue("stateType_string");
        JArray boolArr = (JArray)obj.GetValue("stateType_bool");

        foreach (JObject stateObj in intArr)
        {
            StateObject<int> state = new StateObject<int>(stateObj);
            state.SetTargetValue((int)stateObj.GetValue("targetValue"));
            stateType_int.Add(state);
        }
        foreach (JObject stateObj in floatArr)
        {
            StateObject<float> state = new StateObject<float>(stateObj);
            state.SetTargetValue((float)stateObj.GetValue("targetValue"));
            stateType_float.Add(state);
        }
        foreach (JObject stateObj in stringArr)
        {
            StateObject<string> state = new StateObject<string>(stateObj);
            state.SetTargetValue((string)stateObj.GetValue("targetValue"));
            stateType_string.Add(state);
        }
        foreach (JObject stateObj in boolArr)
        {
            StateObject<bool> state = new StateObject<bool>(stateObj);
            state.SetTargetValue((bool)stateObj.GetValue("targetValue"));
            stateType_bool.Add(state);
        }

        idCount = (int)obj.GetValue("idCount");
    }
}
