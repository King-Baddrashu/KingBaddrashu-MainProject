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
    [SerializeField]public float curTime = 0; // 현재 시간
    public float dayTimeLimit = 300; // 하루 최대 시간
    public bool stopTimer;
    public bool isEndDay = false;
    public bool isEndWeak = false;
    public bool isEpisode = false;
    public Image timer;
    public Image interTimer;
    public int curDayNum; //현재 일수

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
        // 전체 초기화 진행

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
            dailyReward[(PlayerStatusType)i] = 0; // 0으로 초기화
            weaklyReward[(PlayerStatusType)i] = 0; // 0으로 초기화
        }

        ScheduleManager.instance.ClearSchedule();
        dialogMgr.showDialog = false ;
    }

    public void Update()
    {
        // 있는 스케줄 표가 없으면 스케줄 생성
        if(scheduleData.Count == 0)
        {
            uiSchedule.SetActive(true);
            isEndDay = true;
            curDayNum = -1;
            return;
        }

        // 하루 일과 루틴 진행
        if(isEndDay && !stopTimer)
        {
            // 하루가 끝나면 하루치 보상을 보여줌
            CaculateDailyReward();
            return;
        }
        else if(!stopTimer)
        {
            // 시간 관리
            CheckDayTime();
            //타이머 갱신
            timer.fillAmount = curTime / dayTimeLimit;
            // NPC 관리
            NPCMgr();
        }

        if(!isEndDay && isImmediateAction)
        {
            // 즉시 일과 진행시 약간의 설명과 함께 보상을 바로 보여줌
            StartCoroutine(ShowImmediateAction());
            return;
        }

        if(!isEndDay && isEpisode)
        {
            ShowDialog("");
        }

        // 에피소드는 에피소드 진행 후 다음 일과 진행
        // 다음 일과 진행, 현재 체력은 회복됨 -> Init하면서 진행
        // 모든 스케줄 표를 진행시 주간 보상을 보여줌 + 스케줄을 삭제함 -> 주간 스케줄 보여주면서 진행
        // 위부터 다시 시작
    }

    //다음 스케줄 진행전에 호출함
    public  void Init()
    {
        // 다음 맵 준비
        GetNextMap();

        // NPC 관리
        NPCMgr();

        // 시간 초기화
        isEndDay = false;
        stopTimer = false;
        curTime = 0;

        // 체력 초기화

        PlayerStatusManager.instance.data.SetStatusValue(PlayerStatusType.CUR_HEALTH,
            PlayerStatusManager.instance.data.GetStatusValue(PlayerStatusType.MAX_HEALTH));
        
        // 타이머 초기화
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
        //시작
        Init();
        StartCoroutine(FadeOutImg(fadeImage));
    }

    // 하루 시간 관리, 시간 종료시 정산 화면을 보여줌, 다음 맵 진행
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

    // 하루 맵 관리
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

    // 맵 생성시 NPC 생성 관리 (NPC 풀로 관리), 각 NPC는 25% 확률로 등장함
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

        // 초기화
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
    
    // 하루 끝날 시 정산 화면을 보여줌 -> 다음 맵으로 진행
    public void CaculateDailyReward()
    {
        dailyReward.Clear();

        for(int i = 0; i <= System.Enum.GetValues(typeof(NPCStatusType)).Length; i++)
        {
            dailyReward[(PlayerStatusType)i] = 0; // 0으로 초기화
        }

        foreach(var data in rewardAccumulate)
        {
            dailyReward[data.type] += data.value; // 보상을 전부 더함
            weaklyReward[data.type] += data.value;
        }
        rewardAccumulate.Clear();
        // 일일 보상 출력
        StartCoroutine(ShowReward(true));
    }

    IEnumerator ShowReward(bool daily)
    {
        Debug.Log("Reward 보여줌");
        // 보상 출력 루틴
        stopTimer = true;

        if(fadeImage.color.a != 1.0f)
        {
            StartCoroutine(FadeInImg(fadeImage));
            yield return new WaitForSeconds(fadeMaxTime);
        }

        // 맵, NPC, Player 꺼두기
        playerObj.SetActive(false);
        foreach (var obj in mapData)
            obj.Value.SetActive(false);
        foreach (var obj in npcData)
            obj.Value.SetActive(false);
        string txt = "";
        if(daily)
        {
            txt += "[ 하.루.일.과 ]\n";
            foreach(var data in dailyReward)
            {
                if(data.Value != 0)
                    txt += string.Format($"{PlayerStatusExtenstion.PlayerStatusType2Info(data.Key)} : {data.Value.ToString("F2")}\n");
            }
        }
        else
        {
            txt += "[ 주.간.일.과 ]\n";
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

        Debug.Log("즉시 행동 보여줌");

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

        Debug.Log("행동 텍스트 보여줌 + " + fadeText.text);

        ResultData result = new ResultData();
        // 보상 처리
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
        // 파일 불러오고
        // 실행하고
        // 끝나면 하루 보상 보여주고
        // 다음으로 넘기고

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
