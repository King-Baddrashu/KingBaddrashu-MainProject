using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class UIDialogManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject uiDialog;
    public Image imgProfile;
    public Text textName;
    public Text textScriptField;
    public Image imgBackground;
    public ScheduleManager.PlaceType placeType;

    [Header("NPC Data")]
    public SerializableDictionary<NPCStatusType, Sprite> npc1SeoYeonwoo;
    public SerializableDictionary<NPCStatusType, Sprite> npc2LeeShiyoungImg;
    public SerializableDictionary<NPCStatusType, Sprite> npc3HanseaImg;
    public SerializableDictionary<ScheduleManager.PlaceType, Sprite> background;
    public Sprite playerProfile;
    public Sprite otherProfile;
    public bool showDialog = false;
    public bool isActiveInput = true;
    public bool isScriptEnd = false;
    public ScriptData scriptData;

    [Header("Animation Setting")]
    public bool isOnEmotionAnimation;
    public bool isOnTextAnimation = true;
    public bool isOnInputAnimation;
    public bool skipTextAnimation;

    [Tooltip("lower is fast")]
    public float textSpeed;

    [Header("UI User Input")]
    public UIDialogInput input;

    public void Start()
    {
        // Test Dialog
        //1. Load Script Data
        //ScriptDataFormatter.LoadScriptData(out scriptData, "E:\\_Projects\\Unity\\ScriptManagementSystem\\서연우episode1.scirptData");
        //2. Ready Script Data
        //ChangeScriptData(scriptData); 
    }

    public void Update()
    {
        if (!showDialog && isScriptEnd)
        {
            return;
        }
        else
            uiDialog.SetActive(true);

        if (!isActiveInput) return;
        // Space Bar || Mouse Button 0 누르면 다음 노드로 넘어가기
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if(isOnTextAnimation)
            {
                isOnTextAnimation = false;
            }
            else
            {
                LoadNextScript();
            }
        }

        if (input.userInput != -1)
        {
            LoadNextScript();
        }

        imgBackground.sprite = background[placeType];
    }

    public void ChangeScriptData(ScriptData data)
    {
        this.scriptData = data;
        scriptData.StartScript();
        isScriptEnd = false;
        ShowScript();
    }

    public IEnumerator TextAnimation(string data)
    {
        textScriptField.text = "";
        //타이핑
        for (int i =0; i < data.Length; i++)
        {
            if (isOnTextAnimation)
                textScriptField.text += data[i];
            else
            {
                if (input.isAction)
                    input.waitTimer = 0;
                break;
            }

            yield return new WaitForSeconds(textSpeed);
        }

        isOnTextAnimation = false;
        textScriptField.text = data;
    }

    public void ShowScript()
    {
        showDialog = true;
        ScriptNode node = scriptData.GetCurrentNode();

        textName.text = GetNPCName(node.GetNPCType());
        Sprite img = GetNPCSprite(node.GetNPCType(), node.GetNPCStatus());
        float ratio = img.border.y / img.border.y;
        imgProfile.sprite = img;
        imgProfile.SetNativeSize();

        isOnTextAnimation = true;


        switch (scriptData.GetCurrentNode().GetScriptType())
        {
            case ScriptType.NORMAL:
                isOnInputAnimation = true;

                if(node.GetNodeID() == scriptData.startNodeID)
                {
                    if(GetPlaceType(node.GetScriptContent()))
                    {
                        LoadNextScript();
                        return;
                    }
                }
                StartCoroutine(TextAnimation(node.GetScriptContent()));
                break;
            case ScriptType.OPTIONAL:
                isOnInputAnimation = true;
                StartCoroutine(TextAnimation(node.GetScriptContent()));
                input.ShowDialogInput(scriptData.GetConnectedNode(node.GetNodeID()), node.GetScriptContent().Length * textSpeed);

                break;
            case ScriptType.RESULT:
                Debug.Log(node.GetScriptContent());
                break;
        }
    }

    public bool GetPlaceType(string str)
    {
        if (str.CompareTo(ScheduleManager.PlaceType.LIBRARY.ToString()) == 0)
        {
            placeType = ScheduleManager.PlaceType.LIBRARY;
            return true;
        }
        else if (str.CompareTo(ScheduleManager.PlaceType.GYM.ToString()) == 0)
        {
            placeType = ScheduleManager.PlaceType.GYM;
            return true;
        }
        else if (str.CompareTo(ScheduleManager.PlaceType.STREET.ToString()) == 0)
        {
            placeType = ScheduleManager.PlaceType.STREET;
            return true;
        }
        else if (str.CompareTo(ScheduleManager.PlaceType.PC.ToString()) == 0)
        {
            placeType = ScheduleManager.PlaceType.PC;
            return true;
        }
        else if (str.CompareTo(ScheduleManager.PlaceType.CAFE.ToString()) == 0)
        {
            placeType = ScheduleManager.PlaceType.CAFE;
            return true;
        }
        else if (str.CompareTo(ScheduleManager.PlaceType.HOME.ToString()) == 0)
        {

            placeType = ScheduleManager.PlaceType.HOME;
            return true;
        }
        else if (str.CompareTo(ScheduleManager.PlaceType.COLLEGE.ToString()) == 0)
        {
            placeType = ScheduleManager.PlaceType.COLLEGE;
            return true;

        }
        else if (str.CompareTo(ScheduleManager.PlaceType.COURT.ToString()) == 0)
        {
            placeType = ScheduleManager.PlaceType.COURT;
            return true;

        }
        else if (str.CompareTo(ScheduleManager.PlaceType.EP1.ToString()) == 0)
        {
            placeType = ScheduleManager.PlaceType.EP1;
            return true;

        }
        if (str.CompareTo("AUTO") == 0)
        {
            if(GameManager.instance != null)
            {
                placeType = GameManager.instance.curPlaceType;
            }

            return true;
        }
        else
        {
            // 하드코딩 진행
        }

        Debug.Log("Place Type : " + placeType.ToString());

        return false;
    }

    public void LoadNextScript()
    {
        int test = -1;
        switch(scriptData.GetCurrentNode().GetScriptType())
        {
            case ScriptType.NORMAL:
                test = scriptData.GetAndChangeCurrentToNextNode();
                break;
            case ScriptType.OPTIONAL:
                if (input.userInput == -1) return;
                scriptData.SetInt("선택지", input.userInput);
                scriptData.CaculateNextNode();
                test = scriptData.GetAndChangeCurrentToNextNode();
                break;
            case ScriptType.RESULT:
                // 대화 종료 및 보상처리
                showDialog = false;
                isScriptEnd = true;
                test = scriptData.GetAndChangeCurrentToNextNode();
                return;
        }
        isOnTextAnimation = true;

        if(test == -1)
        {
            showDialog = false;
            isScriptEnd = true;
            Debug.Log("End Or Error...!");
            return;
        }

        input.Clear();
        ShowScript();
    }

    public static string GetNPCName(NPCType npc)
    {
        switch(npc)
        {
            case NPCType.NONE: return "주인공";
            case NPCType.NPC1: return "서연우";
            case NPCType.NPC2: return "이시영";
            case NPCType.NPC3: return "한세아";
            case NPCType.NPC4: return "남우빈";
            case NPCType.NPC5: return "교수";
            case NPCType.NPC6: return "남학생1";
            case NPCType.NPC7: return "남학생2";
            case NPCType.NPC8: return "여학생";
            case NPCType.NPC9: return "남자";
            case NPCType.NPC10: return "동아리 선배";
            default: return "???";
        }
    }

    public Sprite GetNPCSprite(NPCType npc, NPCStatusType type)
    {
        Sprite data;
        switch(npc)
        {
            case NPCType.NPC1:
                if (npc1SeoYeonwoo.TryGetValue(type, out data))
                    return data;
                else
                    return npc1SeoYeonwoo[NPCStatusType.NORMAL];
            case NPCType.NPC2:
                if(npc2LeeShiyoungImg.TryGetValue(type, out data))
                    return data;
                else
                    return npc2LeeShiyoungImg[NPCStatusType.NORMAL];
            case NPCType.NPC3:
                if (npc3HanseaImg.TryGetValue(type, out data))
                    return data;
                else
                    return npc3HanseaImg[NPCStatusType.NORMAL];
            case NPCType.NONE:
                return playerProfile;
            default: return otherProfile;
        }
    }
}
