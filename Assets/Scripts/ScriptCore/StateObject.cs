using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class StateObject<T> : IEquatable<StateObject<T>>
{
    int objectID;
    string targetParamName;
    RelationalOper oper;
    T targetValue;

    public StateObject(int objectID) {
        this.objectID = objectID;
    }
    public StateObject(JObject obj)
    {
        ToData(obj);
    }
    public int GetObjectID() { return objectID; }
    public void SetTargetValue(T value) { targetValue = value; }
    public T GetTargetValue() { return targetValue; }
    public void SetRelationalOper(RelationalOper oper) { this.oper = oper; }
    public RelationalOper GetRelationalOper() { return oper; }
    public void SetTargetParameterName(string target) { targetParamName = target; }
    public string GetTargetParameterName() { return targetParamName; }
    public bool CompareStateValue(T value)
    {
        int tmp = 0;
        // 모호한 값에 대한 비교 연산이 불가능함! System.Comparer를 사용해야 함!
        tmp = Comparer<T>.Default.Compare(targetValue, value);

        switch (oper) { 
            case RelationalOper.GREATER:
                return tmp > 0;
            case RelationalOper.LESS:
                return tmp < 0;
            case RelationalOper.EQUALS:
                return tmp == 0;
            case RelationalOper.N_EQUALS:
                return tmp != 0;
            default:
                return false;
        }
    }

    public bool Equals(StateObject<T> other)
    {
        if (other == null) return false;

        if (other.objectID == this.objectID) return true;
        
        return false;
    }

    public override string ToString()
    {
        return string.Format($"TargetType : {typeof(T).Name} / TargetParam : {targetParamName} / TargetValue : {targetValue} / RelationalOper : {oper}");
    }

    public JObject ToJson()
    {
        JObject obj = new JObject();
        obj.Add("objectID", objectID);
        obj.Add("targetParamName", targetParamName);
        obj.Add("oper", (int)oper);

        return obj;
    }

    public void ToData(JObject obj)
    {
        objectID = (int)obj.GetValue("objectID");
        targetParamName = (string)obj.GetValue("targetParamName");
        oper = (RelationalOper)(int)obj.GetValue("oper");
    }
}
