using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PlayerStatusType
{
    MONEY,
    MAX_HEALTH,
    CUR_HEALTH,
    IQ,
    APPERARANCE,
    MORALITY,
    SOCIALITY,
    SYMPATHY,
    STRESS,

}
[System.Serializable]
public enum PlayerValue
{
    A = 390, B = 275, C = 150, D = 100, E = 50, F = 25
}

[System.Serializable]
public class PlayerStatus
{
    public PlayerValue value
    {
        get;
        private set;
    }
        
    [SerializeField] float money;
    [SerializeField] float health;
    [SerializeField] float curHealth;
    [SerializeField] float iq;
    [SerializeField] float apperarance;
    [SerializeField] float morality;
    [SerializeField] float sociality;
    [SerializeField] float sympathy;
    [SerializeField] float stress;

    public float GetStatusValue(PlayerStatusType type)
    {
        switch (type)
        {
            case PlayerStatusType.MONEY: return money;
            case PlayerStatusType.MAX_HEALTH: return health;
            case PlayerStatusType.CUR_HEALTH: return curHealth;
            case PlayerStatusType.IQ: return iq;
            case PlayerStatusType.APPERARANCE: return apperarance;
            case PlayerStatusType.MORALITY: return morality;
            case PlayerStatusType.SOCIALITY: return sociality;
            case PlayerStatusType.SYMPATHY: return sympathy;
            case PlayerStatusType.STRESS: return stress;
        }
        
        return -1;
    }

    public void SetStatusValue(PlayerStatusType type, float value)
    {
        switch (type)
        {
            case PlayerStatusType.MONEY: money = value; break;
            case PlayerStatusType.MAX_HEALTH: health = value; break;
            case PlayerStatusType.CUR_HEALTH: curHealth = value; break;
            case PlayerStatusType.IQ: iq = value; break;
            case PlayerStatusType.APPERARANCE: apperarance = value; break;
            case PlayerStatusType.MORALITY: morality = value; break;
            case PlayerStatusType.SOCIALITY: sociality = value; break;
            case PlayerStatusType.SYMPATHY: sympathy = value; break;
            case PlayerStatusType.STRESS: stress = value; break;
        }
        CaculatePlayerValue();
    }

    public void AddStatusValue(PlayerStatusType type, float value)
    {
        switch (type)
        {
            case PlayerStatusType.MONEY: money += value; break;
            case PlayerStatusType.MAX_HEALTH: health += value; break;
            case PlayerStatusType.CUR_HEALTH: curHealth += value; break;
            case PlayerStatusType.IQ: iq += value; break;
            case PlayerStatusType.APPERARANCE: apperarance += value; break;
            case PlayerStatusType.MORALITY: morality += value; break;
            case PlayerStatusType.SOCIALITY: sociality += value; break;
            case PlayerStatusType.SYMPATHY: sympathy += value; break;
            case PlayerStatusType.STRESS: stress += value; break;
        }
        CaculatePlayerValue();
    }

    public PlayerValue CaculatePlayerValue()
    {
        float tmp = iq + apperarance + sociality + sympathy;

        if(tmp >= (int)PlayerValue.A)
        {
            value = PlayerValue.A;
        }
        else if(tmp >= (int)PlayerValue.B)
        {
            value = PlayerValue.B;
        }
        else if(tmp >= (int)PlayerValue.C)
        {
            value = PlayerValue.C;
        }
        else if(tmp >= (int)PlayerValue.D)
        {
            value = PlayerValue.D;
        }
        else if(tmp >= (int)PlayerValue.E)
        {
            value = PlayerValue.E;
        }
        else if(tmp >= (int)PlayerValue.F)
        {
            value = PlayerValue.F;
        }

        return value;
    }

    public float GetPlayerValue()
    {
        return iq + apperarance + sociality + sympathy;
    }
}

public class ActionData
{
    public string script;
    public bool isAction;
}

public struct ResultData
{
    public PlayerStatusType type;
    public float value;
}

public class PlayerStatusManager : MonoBehaviour
{
    public static PlayerStatusManager instance;

    [Header("Player Status Data")]
    public PlayerStatus data;

    [Header("Player Script Data")]
    [Tooltip("하기 싫을 때 하는 말")]
    public List<string> excuseScripts;
    [Tooltip("행동을 수행할 때 하는 말")]
    public List<string> readyScripts;

    public int count;

    private void Awake()
    {
        instance = this;
    }

    // 행동 수행 여부 - 확률로 처리함
    // 확률 : (400 - (25~400)) / 4 * 0.2 + (스트레스 수치) * 0.8
    public ActionData CheckAction()
    {
        ActionData actionData = new ActionData();
        float perValue = ((400f - data.GetPlayerValue()) / 4f) * 0.2f + (Mathf.Log10(Mathf.Clamp(100 - data.GetStatusValue(PlayerStatusType.STRESS), 10f, 100f)) - 1f) * 80f;
        print($"perValue : {perValue}");
        float tmpPer = Random.Range(0f, 100f);

        if (tmpPer <= perValue)
        {
            actionData.isAction = true;

            actionData.script =
                readyScripts[Random.Range(0, readyScripts.Count - 1)];
        }
        else
        {
            actionData.isAction = false;

            actionData.script =
                excuseScripts[Random.Range(0, excuseScripts.Count - 1)];
        }

        return actionData;
    }

    // 행동 보상 처리
    // 처리 방식 : 해당 타입의 고정 보상량 * (스트레스에 따른 성취도(0.197~1.03)), 
    public float ResultPlayerStatus(ResultData resultDatas)
    {
        float result = 1.0f;
        if (resultDatas.type != PlayerStatusType.STRESS && resultDatas.type != PlayerStatusType.CUR_HEALTH && resultDatas.type != PlayerStatusType.MONEY)
        {
            float increaseValue = Mathf.Log10(Mathf.Clamp(100 - data.GetStatusValue(PlayerStatusType.STRESS), 10f, 100f)) - 1f;
            float randomInterval = 0.03f;
            increaseValue = Mathf.Clamp(increaseValue, 0.2f, 1.0f);
            float randomIncrease = Random.Range(increaseValue - randomInterval,
                increaseValue + randomInterval);
            if (randomIncrease <= 0) randomIncrease = 0f;

            result = resultDatas.value * randomIncrease;
            data.AddStatusValue(resultDatas.type, resultDatas.value * randomIncrease);
        }
        else
        {
            // Stress and current health
            data.AddStatusValue(resultDatas.type, resultDatas.value);
            result = resultDatas.value;
        }

        var input = new ResultData();
        input.type = resultDatas.type;
        input.value = result;

        // 보상 기록 진행
        GameManager.instance.rewardAccumulate.Add(input);
        return result;
    }

    // 플레이어 등급 계산
    // data에 저장된 등급을 가져옴
    public PlayerValue GetPlayerValue()
    {
        return data.value;
    }
}