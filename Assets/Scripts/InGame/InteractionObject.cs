using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public string interactionName;
    public Sprite interactionImg;
    public List<RewardData> rewards;
    public float actionTime;

    public bool isCanAction;
    public bool isDone;

    public void Start()
    {
        isCanAction = true;
        isDone = false;
    }

    public void StartAction()
    {
        if (!isCanAction) return;
        isDone = false;
        isCanAction = false;
        Debug.Log("Start Action : " + interactionName);
        GameManager.instance.interTimer.fillAmount = (GameManager.instance.curTime + actionTime) / GameManager.instance.dayTimeLimit;
        StartCoroutine(Action());
    }

    public IEnumerator Action()
    {
        yield return new WaitForSeconds(actionTime);
        isDone = true;

        Debug.Log(GetRewardInfomation());
        foreach (RewardData data in rewards)
        {
            float random = Random.Range(0f, 100f);
            if(random <= data.percent)
            {
                ResultData result = new ResultData();
                result.type = data.type;
                result.value = data.value;
                var v = PlayerStatusManager.instance.ResultPlayerStatus(result);
                
                yield return new WaitForSeconds(1.0f);
                var reward = InteractionManager.instance.uiRewardTemplate;
                reward = Instantiate(reward);
                reward.SetActive(true);
                reward.GetComponent<UIRewardAnimation>().text.text =
                    PlayerStatusExtenstion.PlayerStatusType2Info(data.type) + " " + v.ToString("F2");
                reward.transform.position = InteractionManager.instance.player.transform.position + Vector3.up * 0.5f;
            }
        }

        isCanAction = true;
        Debug.Log("Done Action : " + interactionName);
    }

    public string GetRewardInfomation()
    {
        string str;
        str = string.Format($"[기대 보상]\n");

        foreach (RewardData data in rewards)
        {
            if(data.percent > 10 && data.type != PlayerStatusType.CUR_HEALTH && data.type != PlayerStatusType.STRESS)
                str += string.Format($"{PlayerStatusExtenstion.PlayerStatusType2Info(data.type)} : {data.value} \n");
            else if(data.percent <= 10)
                str += string.Format($"+ {PlayerStatusExtenstion.PlayerStatusType2Info(data.type)} \n");
            else if(data.type == PlayerStatusType.CUR_HEALTH || data.type == PlayerStatusType.STRESS)
                str += string.Format($"{PlayerStatusExtenstion.PlayerStatusType2Info(data.type)} : {data.value} \n");
        }

        return str;
    }
}
