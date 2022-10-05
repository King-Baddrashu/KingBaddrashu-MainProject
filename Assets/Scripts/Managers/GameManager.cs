using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class NPCDailyData
{
    public NPCType npcType;
    public string path;
} 

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Control Day")]
    [SerializeField]public float curTime = 0; // ���� �ð�
    public float dayTimeLimit = 300; // �Ϸ� �ִ� �ð�
    public bool stopTimer;
    public bool isEndDay = false;
    public bool isEndWeak = false;
    public bool isEpisode = false;
    public Image timer;
    public Image interTimer;
    public int curDayNum; //���� �ϼ�

    [Header("Control Map")]
    public ScheduleManager.PlaceType curPlaceType;
    public ScheduleManager.BehaviourType curBehaviourType;
    public SerializableDictionary<ScheduleManager.PlaceType, GameObject> mapData;
    public bool isImmediateAction;
    public GameObject playerObj;

    [Header("Control NPC")]
    public SerializableDictionary<NPCType, GameObject> npcData;
    public SerializableDictionary<NPCType, float> npcStartTime;

    [Header("Calculate Reward")]
    public List<ResultData> rewardAccumulate;
    public SerializableDictionary<PlayerStatusType, float> dailyReward;
    public SerializableDictionary<PlayerStatusType, float> weaklyReward;

    [Header("Schedule Data")]
    public GameObject uiSchedule;
    public List<ScheduleManager.BehaviourData> scheduleData;

    [Header("Immediate Text Data")]
    public SerializableDictionary<ScheduleManager.BehaviourType, List<string>> immediateTextData;

    [Header("Animation Fade In")]
    public GameObject mainUI;
    public Image fadeImage;
    public Text fadeText;
    public float fadeMaxTime = 2;
    public float waitTime = 10;

    [Header("Contorl Dialog")]
    public GameObject dialogObj;
    public SerializableDictionary<ScheduleManager.BehaviourType, string> SYWEpisodePath;
    public SerializableDictionary<ScheduleManager.BehaviourType, string> LSYEpisodePath;
    public SerializableDictionary<ScheduleManager.BehaviourType, string> HSAEpisodePath;
    public UIDialogManager dialogMgr;
    public Image dialogFadeImg;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        // ��ü �ʱ�ȭ ����

        SetImageAlpha(dialogFadeImg, 1);
        StartCoroutine(FadeOutImg(dialogFadeImg));
        SetImageAlpha(fadeImage, 1);
        SetTextAlpha(fadeText, 0);

        curPlaceType = ScheduleManager.PlaceType.NONE;
        curBehaviourType = ScheduleManager.BehaviourType.NONE;
        isEndDay = false;
        isEndWeak = false;
        isEpisode = false;
        curDayNum = -1;

        isImmediateAction = false;
        if(scheduleData == null)
            scheduleData = new List<ScheduleManager.BehaviourData>();
        else
        {
            Init();
        }
        rewardAccumulate = new List<ResultData>();
        dailyReward = new SerializableDictionary<PlayerStatusType, float>();
        weaklyReward = new SerializableDictionary<PlayerStatusType, float>();

        for (int i = 0; i <= System.Enum.GetValues(typeof(NPCStatusType)).Length; i++)
        {
            dailyReward[(PlayerStatusType)i] = 0; // 0���� �ʱ�ȭ
            weaklyReward[(PlayerStatusType)i] = 0; // 0���� �ʱ�ȭ
        }

        ScheduleManager.instance.ClearSchedule();
        dialogMgr.showDialog = false ;
    }

    public void Update()
    {
        // �ִ� ������ ǥ�� ������ ������ ����
        if(scheduleData.Count == 0)
        {
            uiSchedule.SetActive(true);
            isEndDay = true;
            curDayNum = -1;
            return;
        }

        // �Ϸ� �ϰ� ��ƾ ����
        if(isEndDay && !stopTimer)
        {
            // �Ϸ簡 ������ �Ϸ�ġ ������ ������
            CaculateDailyReward();
            return;
        }
        else if(!stopTimer)
        {
            // �ð� ����
            CheckDayTime();
            //Ÿ�̸� ����
            timer.fillAmount = curTime / dayTimeLimit;
            // NPC ����
            NPCMgr();
        }

        if(!isEndDay && isImmediateAction)
        {
            // ��� �ϰ� ����� �ణ�� ����� �Բ� ������ �ٷ� ������
            StartCoroutine(ShowImmediateAction());
            return;
        }

        if(!isEndDay && isEpisode)
        {
            ShowDialog("");
        }

        // ���Ǽҵ�� ���Ǽҵ� ���� �� ���� �ϰ� ����
        // ���� �ϰ� ����, ���� ü���� ȸ���� -> Init�ϸ鼭 ����
        // ��� ������ ǥ�� ����� �ְ� ������ ������ + �������� ������ -> �ְ� ������ �����ָ鼭 ����
        // ������ �ٽ� ����
    }

    //���� ������ �������� ȣ����
    public  void Init()
    {
        // ���� �� �غ�
        GetNextMap();

        // NPC ����
        NPCMgr();

        // �ð� �ʱ�ȭ
        isEndDay = false;
        stopTimer = false;
        curTime = 0;

        // ü�� �ʱ�ȭ

        PlayerStatusManager.instance.data.SetStatusValue(PlayerStatusType.CUR_HEALTH,
            PlayerStatusManager.instance.data.GetStatusValue(PlayerStatusType.MAX_HEALTH));
        
        // Ÿ�̸� �ʱ�ȭ
        timer.fillAmount = 0f;
        interTimer.fillAmount = 0f;

        if (fadeImage.color.a != 0f)
            StartCoroutine(FadeOutImg(fadeImage));
        uiSchedule.SetActive(false);
    }

    public void StartWeak(List<ScheduleManager.BehaviourData> scheduleData)
    {
        this.scheduleData = scheduleData;
        
        isEndDay = false;
        isEndWeak = false;
        isEpisode = false;
        curDayNum = -1;

        StartCoroutine(StartWeak());
    }

    IEnumerator StartWeak()
    {
        StartCoroutine(FadeInImg(fadeImage));
        yield return new WaitForSeconds(fadeMaxTime);
        //����
        Init();
        StartCoroutine(FadeOutImg(fadeImage));
    }

    // �Ϸ� �ð� ����, �ð� ����� ���� ȭ���� ������, ���� �� ����
    public void CheckDayTime()
    {
        if (isEndDay) return;

        curTime += Time.deltaTime;
        if(curTime >= dayTimeLimit)
        {
            isEndDay = true;
            curTime = 0;
            Debug.Log("End Day");
        }
    }

    public void SkipDay()
    {
        isEndDay = true;
    }

    // �Ϸ� �� ����
    public void GetNextMap()
    {
        curDayNum++;

        if(curDayNum < scheduleData.Count)
        {
            var item = scheduleData[curDayNum];
            curPlaceType = item.placeType;
            curBehaviourType = item.type;

            isEpisode = false;

            foreach (var obj in mapData)
            {
                obj.Value.SetActive(false);
            }

            switch (curPlaceType)
            {
                case ScheduleManager.PlaceType.LIBRARY:
                    if(item.type == ScheduleManager.BehaviourType.JOB)
                    {
                        isImmediateAction = true;
                    }
                    else
                    {
                        mapData[curPlaceType].SetActive(true);
                        isImmediateAction = false;
                    }
                    break;
                case ScheduleManager.PlaceType.GYM:
                case ScheduleManager.PlaceType.STREET:
                case ScheduleManager.PlaceType.HOME:
                    isImmediateAction = false;
                    mapData[curPlaceType].SetActive(true);
                    break;
                case ScheduleManager.PlaceType.SYW:
                case ScheduleManager.PlaceType.LSY:
                case ScheduleManager.PlaceType.HSA:
                    isEpisode = true;
                    break;
                default:
                    isImmediateAction = true;
                    break;
            }


            isEndWeak = false;
        }
        else
        {
            isEndWeak = true;
        }

        if(!isImmediateAction && !isEpisode)
        {
            playerObj.SetActive(true);
            playerObj.transform.position = Vector3.zero;
        }
        else
        {
            playerObj.SetActive(false);
        }
    }

    // �� ������ NPC ���� ���� (NPC Ǯ�� ����), �� NPC�� 25% Ȯ���� ������
    public void NPCMgr()
    {
        if(!isEndDay)
        {
            foreach(var item in npcStartTime)
            {
                if(item.Value >= curTime)
                {
                    npcData[item.Key].SetActive(true);
                }
            }

            return;
        }

        // �ʱ�ȭ
        npcStartTime.Clear();
        foreach (var item in npcStartTime)
        {
            if (item.Value >= curTime)
            {
                npcData[item.Key].SetActive(false);
            }
        }
        if (curPlaceType != ScheduleManager.PlaceType.HOME && !isImmediateAction && !isEpisode)
        {
            foreach (var item in npcData)
            {
                if (Random.Range(0f, 100f) < 35f)
                {
                    npcStartTime.Add(new SerializableDictionary<NPCType, float>.Pair(item.Key, Random.Range(0, 20f)));
                    Debug.Log($"Show NPC{npcStartTime[item.Key]} : " + item.Key.ToString());
                }
            }
        }
    }
    
    // �Ϸ� ���� �� ���� ȭ���� ������ -> ���� ������ ����
    public void CaculateDailyReward()
    {
        dailyReward.Clear();

        for(int i = 0; i <= System.Enum.GetValues(typeof(NPCStatusType)).Length; i++)
        {
            dailyReward[(PlayerStatusType)i] = 0; // 0���� �ʱ�ȭ
        }

        foreach(var data in rewardAccumulate)
        {
            dailyReward[data.type] += data.value; // ������ ���� ����
            weaklyReward[data.type] += data.value;
        }
        rewardAccumulate.Clear();
        // ���� ���� ���
        StartCoroutine(ShowReward(true));
    }

    IEnumerator ShowReward(bool daily)
    {
        Debug.Log("Reward ������");
        // ���� ��� ��ƾ
        stopTimer = true;

        if(fadeImage.color.a != 1.0f)
        {
            StartCoroutine(FadeInImg(fadeImage));
            yield return new WaitForSeconds(fadeMaxTime);
        }

        // ��, NPC, Player ���α�
        playerObj.SetActive(false);
        foreach (var obj in mapData)
            obj.Value.SetActive(false);
        foreach (var obj in npcData)
            obj.Value.SetActive(false);
        string txt = "";
        if(daily)
        {
            txt += "[ ��.��.��.�� ]\n";
            foreach(var data in dailyReward)
            {
                if(data.Value != 0)
                    txt += string.Format($"{PlayerStatusExtenstion.PlayerStatusType2Info(data.Key)} : {data.Value.ToString("F2")}\n");
            }
        }
        else
        {
            txt += "[ ��.��.��.�� ]\n";
            foreach (var data in weaklyReward)
            {
                if (data.Value != 0)
                    txt += string.Format($"{PlayerStatusExtenstion.PlayerStatusType2Info(data.Key)} : {data.Value.ToString("F2")}\n");
            }
        }

        fadeText.text = txt;
        StartCoroutine(FadeInText(fadeText));
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(FadeOutText(fadeText));
        yield return new WaitForSeconds(fadeMaxTime);


        if (daily && isEndWeak)
        {
            StartCoroutine(ShowReward(false)); 
            scheduleData.Clear();
            ScheduleManager.instance.ClearSchedule();
        }
        else
        {
            Init();
            StartCoroutine(FadeOutImg(fadeImage));
        }

        if(!daily)
        {
            
            StartCoroutine(FadeOutImg(fadeImage));
        }

        isEndDay = false;
    }

    IEnumerator ShowImmediateAction()
    {
        stopTimer = true;
        isImmediateAction = false;

        Debug.Log("��� �ൿ ������");

        if (fadeImage.color.a == 0.0f)
        {
            StartCoroutine(FadeInImg(fadeImage));
            yield return new WaitForSeconds(fadeMaxTime);
        }
        yield return new WaitForSeconds(fadeMaxTime);

        //Show action
        fadeText.text = string.Format($"[ {scheduleData[curDayNum].behaviourInfo} ]\n\n");
        fadeText.text += immediateTextData[curBehaviourType][Random.Range(0, immediateTextData[curBehaviourType].Count-1)];
        Debug.Log(immediateTextData[curBehaviourType][Random.Range(0, immediateTextData[curBehaviourType].Count - 1)]);
        StartCoroutine(FadeInText(fadeText));
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(FadeOutText(fadeText));
        yield return new WaitForSeconds(fadeMaxTime);

        Debug.Log("�ൿ �ؽ�Ʈ ������ + " + fadeText.text);

        ResultData result = new ResultData();
        // ���� ó��
        switch (curBehaviourType)
        {
            case ScheduleManager.BehaviourType.JOB:
                result.type = PlayerStatusType.MONEY;
                result.value = 80000;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                result.type = PlayerStatusType.STRESS;
                result.value = 26;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                break;
            case ScheduleManager.BehaviourType.REST:
                result.type = PlayerStatusType.STRESS;
                result.value = -45;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                result.type = PlayerStatusType.IQ;
                result.value = -7;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                result.type = PlayerStatusType.MONEY;
                result.value = -30000;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                break;
            case ScheduleManager.BehaviourType.STUDY:
                result.type = PlayerStatusType.IQ;
                result.value = 8;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                result.type = PlayerStatusType.SOCIALITY;
                result.value = 10;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                result.type = PlayerStatusType.STRESS;
                result.value = 27;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                break;
            case ScheduleManager.BehaviourType.MAJOR:
                result.type = PlayerStatusType.IQ;
                result.value = 15;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                result.type = PlayerStatusType.SOCIALITY;
                result.value = 6;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                result.type = PlayerStatusType.STRESS;
                result.value = 30;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                break;
            case ScheduleManager.BehaviourType.CLUB:
                result.type = PlayerStatusType.SYMPATHY;
                result.value = 5;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                result.type = PlayerStatusType.APPERARANCE;
                result.value = 2;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                result.type = PlayerStatusType.SOCIALITY;
                result.value = 5;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                result.type = PlayerStatusType.MAX_HEALTH;
                result.value = 6;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                result.type = PlayerStatusType.STRESS;
                result.value = -5;
                PlayerStatusManager.instance.ResultPlayerStatus(result);
                break;
        }

        CaculateDailyReward();
        isEndDay = true;
    }

    public void ShowDialog(string filename)
    {
        stopTimer = true;

        if (isEpisode)
        {
            if (curPlaceType == ScheduleManager.PlaceType.SYW)
            {
                StartCoroutine(ShowScript(SYWEpisodePath[curBehaviourType], true));
            }
            if (curPlaceType == ScheduleManager.PlaceType.LSY)
            {
                StartCoroutine(ShowScript(LSYEpisodePath[curBehaviourType], true));
            }
            if (curPlaceType == ScheduleManager.PlaceType.HSA)
            {
                StartCoroutine(ShowScript(HSAEpisodePath[curBehaviourType], true));
            }
            isEpisode = false;
        }

        else
        {
            StartCoroutine(ShowScript(filename, false));
        }
    }

    public IEnumerator ShowScript(string filename, bool isEpisode) {
        // ���� �ҷ�����
        // �����ϰ�
        // ������ �Ϸ� ���� �����ְ�
        // �������� �ѱ��

        ScriptData data = new ScriptData();
        ScriptDataFormatter.LoadScriptData(out data, PathExtenstion.GetScriptPath(filename));
        dialogMgr.ChangeScriptData(data);

        Debug.Log(data.ToJson());
        dialogFadeImg.gameObject.SetActive(true);
        if (dialogFadeImg.color.a == 0.0f)
        {
            StartCoroutine(FadeInImg(dialogFadeImg));
            yield return new WaitForSeconds(fadeMaxTime);
        }
        yield return new WaitForSeconds(fadeMaxTime);

        dialogObj.SetActive(true);
        StartCoroutine(FadeOutImg(dialogFadeImg));

        dialogMgr.isActiveInput = true;

        while (true)
        {
            if (dialogMgr.isScriptEnd)
            {
                StartCoroutine(FadeInImg(dialogFadeImg));
                break;
            }

            yield return null;
        }

        mainUI.SetActive(true);
        yield return new WaitForSeconds(fadeMaxTime + 0.1f);
        dialogObj.SetActive(false);
        StartCoroutine(FadeOutImg(dialogFadeImg));
        stopTimer = false;
        if (isEpisode)
        {
            CaculateDailyReward();
        }
        else
        {
            InteractionManager.instance.EndTalk();
        }
    }

    public IEnumerator FadeInImg(Image img)
    {
        float v = 0;

        for (; v < 1f;)
        {
            v += Time.deltaTime / fadeMaxTime;
            SetImageAlpha(img, v);
            yield return null;
        }

        SetImageAlpha(img, 1.0f);
    }

    public IEnumerator FadeOutImg(Image img)
    {
        float v = 1;

        for (; v > 0f;)
        {
            v -= Time.deltaTime / fadeMaxTime;
            SetImageAlpha(img, v);
            yield return null;
        }

        SetImageAlpha(img, 0.0f);
    }
    public IEnumerator FadeInText(Text img)
    {
        float v = 0;

        for (; v <= 1f;)
        {
            v += Time.deltaTime / fadeMaxTime;
            SetTextAlpha(img, v);
            yield return null;
        }

        SetTextAlpha(img, 1.0f);
    }

    public IEnumerator FadeOutText(Text img)
    {
        float v = 1;

        for (; v > 0f;)
        {
            v -= Time.deltaTime / fadeMaxTime;
            SetTextAlpha(img, v);
            yield return null;
        }

        SetTextAlpha(img, 0.0f);
    }

    void SetImageAlpha(Image img, float v)
    {
        var c = img.color;
        c.a = v;
        img.color = c;
    }
    void SetTextAlpha(Text img, float v)
    {
        var c = img.color;
        c.a = v;
        img.color = c;
    }
}
