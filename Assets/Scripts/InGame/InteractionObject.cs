using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    
}

[System.Serializable]
public struct RewardData
{
    public PlayerStatusType type;
    public float percent;
    public float value;
    public string ToString()
    {
        return type.ToString() + " / value : " + value.ToString() + " / percent : " + percent.ToString();
    }
}

public class InteractionObject : MonoBehaviour
{
    [Header("Reward Data")]
    public List<RewardData> rewards;
    public float actionTime;
    public bool isCanAction;

    public void StartAction()
    {
        if (!isCanAction) return;

        isCanAction = false;
    }

    public IEnumerator Action()
    {
        yield return new WaitForSeconds(actionTime);
        isCanAction = true;

        foreach(RewardData data in rewards)
        {
            float random = Random.Range(0f, 100f);
            if(random <= data.percent)
            {
                ResultData result = new ResultData();
                result.type = data.type;
                result.value = data.value;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
            }
        }
    }
}
