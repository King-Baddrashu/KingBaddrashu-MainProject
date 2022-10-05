using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Familiarity
{
    S = 89, A = 79, B = 56, C = 33, D = 10, E = 0
}

[RequireComponent(typeof(BasicNPCFrame))]
public class NPCDialogManager : MonoBehaviour
{
    [HideInInspector]public BasicNPCFrame frame;

    [Header("NPC Dialog Path")]
    public SerializableDictionary<int, List<string>> pcPaths;
    public SerializableDictionary<int, List<string>> gymPaths;
    public SerializableDictionary<int, List<string>> streetPaths;
    public Familiarity familiarityType;
    public float familiarityValue;
    public int accessLevel = 0;

    public float greetPer;
    public float dialogPer;
    public float spDialogPer;

    public string path;
    public bool isEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        if(familiarityValue != 0f)
        {
            SetFamiliarity(familiarityValue);
        }

        frame = GetComponent<BasicNPCFrame>();
    }

    // Update is called once per frame
    void Update()
    {
        SetFamiliarity(familiarityValue);
    }

    public Familiarity GetFamiliarityType()
    {
        if (familiarityValue >= (int)Familiarity.S)
            return Familiarity.S;
        else if (familiarityValue >= (int)Familiarity.A)
            return Familiarity.A;
        else if (familiarityValue >= (int)Familiarity.B)
            return Familiarity.B;
        else if (familiarityValue >= (int)Familiarity.C)
            return Familiarity.C;
        else if (familiarityValue >= (int)Familiarity.D)
            return Familiarity.D;
        else return Familiarity.E;
    }

    public void SetFamiliarity(float v)
    {
        familiarityValue = v;
        familiarityType = GetFamiliarityType();

        if (familiarityValue >= (int)Familiarity.S)
            accessLevel = 3;
        else if (familiarityValue >= (int)Familiarity.B)
            accessLevel = 2;
        else if (familiarityValue >= (int)Familiarity.D)
            accessLevel = 1;
        else
            accessLevel = 0;
    }

    public bool LoadDialogData()
    {
        // Get percent
        if (accessLevel == 0)
        {
            greetPer = 40;
            dialogPer = 0;
            spDialogPer = 0;
        }
        else if (accessLevel == 1)
        {
            greetPer = 100;
            dialogPer = 0;
            spDialogPer = 0;
        }
        else if(accessLevel == 2)
        {
            greetPer = 60;
            dialogPer = 35;
            spDialogPer = 5;
        }
        else if(accessLevel == 3)
        {
            greetPer = 45;
            dialogPer = 40;
            spDialogPer = 15;
        }


        // Random
        float value = Random.Range(0f, 100f);
        var pType = ScheduleManager.PlaceType.GYM;//GameManager.instance.curPlaceType;
        string path = "";
        if(value <= greetPer)
        {
            if(pType == ScheduleManager.PlaceType.PC)
            {
                path = pcPaths[0][Random.Range(0, pcPaths[0].Count - 1)];
            }
            else if(pType == ScheduleManager.PlaceType.GYM)
            {
                path = gymPaths[0][Random.Range(0, gymPaths[0].Count - 1)];
            }
            else if(pType == ScheduleManager.PlaceType.STREET)
            {
                path = streetPaths[0][Random.Range(0, streetPaths[0].Count - 1)];
            }
        }
        else if(value - greetPer <= dialogPer)
        {
            if (pType == ScheduleManager.PlaceType.PC)
            {
                path = pcPaths[1][Random.Range(0, pcPaths[1].Count - 1)];
            }
            else if (pType == ScheduleManager.PlaceType.GYM)
            {
                path = gymPaths[1][Random.Range(0, gymPaths[1].Count - 1)];
            }
            else if (pType == ScheduleManager.PlaceType.STREET)
            {

                path = streetPaths[1][Random.Range(0, streetPaths[1].Count - 1)];
            }
        }
        else if(value - greetPer - dialogPer <= spDialogPer)
        {
            if (pType == ScheduleManager.PlaceType.PC)
            {
                path = pcPaths[2][Random.Range(0, pcPaths[2].Count - 1)];
            }
            else if (pType == ScheduleManager.PlaceType.GYM)
            {
                path = gymPaths[2][Random.Range(0, gymPaths[2].Count - 1)];
            }
            else if (pType == ScheduleManager.PlaceType.STREET)
            {
                path = streetPaths[2][Random.Range(0, streetPaths[2].Count - 1)];
            }
        }

        // Play dialog
        Debug.Log("path : " + path);
        Debug.Log("Value : " + value);
        this.path = path;
        if(path.CompareTo("") == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool StartDialog()
    {
        if(LoadDialogData())
        {
            GameManager.instance.ShowDialog(path); 
            frame.status = NPCStatus.TALK;
            return true;
        }
        else
        {
            isEnd = true;
            return false;
        }
    }
}
