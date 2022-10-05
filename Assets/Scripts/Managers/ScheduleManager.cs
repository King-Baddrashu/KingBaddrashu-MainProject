using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleManager : MonoBehaviour
{
    public static ScheduleManager instance = null;

    [System.Serializable]
    public enum BehaviourType
    {
        NONE = -1, JOB, GYM, WALK, REST, HOME, MAJOR, STUDY, CLUB, EP1=20, EP2, EP3, EP4, EP5, EP6, EP7, EP8, EP9, EP10
    }

    [System.Serializable]
    public enum PlaceType
    {
        NONE = -1, LIBRARY, GYM, STREET, PC, CAFE, HOME, COLLEGE, EP1, SYW, LSY, HSA, COURT
    }

    [System.Serializable]
    public struct BehaviourData
    {
        public Sprite img;
        public string behaviourInfo;
        public BehaviourType type;
        public PlaceType placeType;
    }

    public BehaviourType selected;
    public List<UIPlace> dailyDatas;
    public Button btnMoveMain;

    [Header("UI Status")]
    public PlaceType curPlace = PlaceType.NONE;
    public PlaceType prePlace = PlaceType.NONE;

    [Header("UI Object List")]
    public SerializableDictionary<PlaceType, GameObject> uiObject;

    [Header("Animation Settings")]
    public float animationTime;
    public float currentAnimationTime;
    public float offset = 2500;
    public Vector3 leftWaitPos;
    public Vector3 rightWaitPos;
    public Vector3 centerShowPos;

    [Header("UI Drag Animation")]
    public GameObject startBtn;
    public GameObject coverUI;
    public GameObject dragDropUI;
    public bool isDragged;

    [Header("UI Schedule Data")]
    public List<GameObject> scheduleFrame;
    public List<BehaviourData> scheduleData;
    public bool isFull;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        centerShowPos = uiObject[PlaceType.NONE].transform.localPosition * ScreenResolutionExtenstion.GetScreenIncreaseRatioAuto();
        leftWaitPos = centerShowPos     + Vector3.left * offset * ScreenResolutionExtenstion.GetScreenIncreaseRatioAuto();
        rightWaitPos = centerShowPos    + Vector3.right * offset * ScreenResolutionExtenstion.GetScreenIncreaseRatioAuto();

        foreach (var obj in uiObject)
        {
            obj.Value.SetActive(true);
            if (obj.Key != PlaceType.NONE)
            {
                obj.Value.SetActive(true);
                obj.Value.transform.localPosition = rightWaitPos;
            }
        }

        btnMoveMain.onClick.AddListener(() => { ChangeView(PlaceType.NONE); });
        ClearSchedule();

        startBtn.GetComponent<Button>().onClick.AddListener(() => { StartAction(); });
    }

    // Update is called once per frame
    void Update()
    {
        // UI Animation Part
        currentAnimationTime = Mathf.Clamp(curPlace != PlaceType.NONE ?
            currentAnimationTime + Time.deltaTime :
            currentAnimationTime - Time.deltaTime,
            0, animationTime);

        float tmpAnimValue = AnimationExtenstion.EaseInOutExpo(currentAnimationTime / animationTime);
        Vector3 mainPos = Vector3.Lerp(centerShowPos, leftWaitPos, tmpAnimValue);
        Vector3 currentShowPos = Vector3.Lerp(rightWaitPos, centerShowPos, tmpAnimValue);

        uiObject[PlaceType.NONE].transform.localPosition = mainPos;

        if (curPlace != PlaceType.NONE)
            uiObject[curPlace].transform.localPosition = currentShowPos;

        foreach (var obj in uiObject)
        {
            if (obj.Key == PlaceType.NONE || obj.Key == curPlace) continue;

            var tmpTr = obj.Value.transform;
            tmpTr.localPosition = rightWaitPos;
        }

        if (prePlace != PlaceType.NONE)
            uiObject[prePlace].transform.localPosition = currentShowPos;

        coverUI.SetActive(isDragged);

        CollectScheduleData();
        startBtn.SetActive(isFull);
    }

    // 현재 화면에 보일 것을 설정함
    public void ChangeView(PlaceType type) {
        prePlace = curPlace;
        curPlace = type;

        if (type == PlaceType.NONE) btnMoveMain.interactable = false;
        else btnMoveMain.interactable = true;
    }

    public void StartAction()
    {
        GameManager.instance.StartWeak(scheduleData);
    }

    // 데이터를 정리해서 구 함
    public void CollectScheduleData()
    {
        scheduleData = new List<BehaviourData>();

        int count = 0;
        foreach(var item in scheduleFrame)
        {
            var info = item.transform.GetChild(0).GetComponent<UIBehaviorInfo>();

            var data = new BehaviourData();
            data.img = info.imgPlace.sprite;
            data.behaviourInfo = info.textBehavior.text;
            data.type = info.behaviourType;
            data.placeType = info.placeType;

            if (info.placeType == PlaceType.NONE)
                count++;

            scheduleData.Add(data);
        }

        if (count == 0) isFull = true;
        else isFull = false;
    }

    public void ClearSchedule()
    {
        foreach(var item in scheduleFrame)
        {
            var obj = item.transform.GetChild(0).GetComponent<UIBehaviorInfo>();
            obj.Clear();
        }
    }

}