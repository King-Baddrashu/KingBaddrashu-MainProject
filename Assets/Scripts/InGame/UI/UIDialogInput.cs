using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogInput : MonoBehaviour
{
    [Header("Template")]
    public GameObject template;
    
    [Header("Animation Element")]
    public GameObject layout;
    public List<GameObject> btnElement;
    public Image imgBackground;

    public bool isAction = false;

    public bool isEnd = false; 
    public int userInput = -1;
    public float currentTime = 0;
    public float animationTime = 1;
    public float waitTimer = 0;
    public float waitTime = 1;

    public Vector3 originPos;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        originPos = layout.transform.localPosition;
        isEnd = true;
        currentTime = 0;

        var c = imgBackground.color;
        c.a = 0; imgBackground.color = c;

        //Test SetUp
        //ScriptNode n1 = new ScriptNode(0);
        //n1.SetScriptContent("test 1");

        //ScriptNode n2 = new ScriptNode(1);
        //n2.SetScriptContent("test 2");

        //ScriptNode n3 = new ScriptNode(2);
        //n3.SetScriptContent("test 3");

        //var tmpList = new List<ScriptNode>();
        //tmpList.Add(n1);
        //tmpList.Add(n2);
        //tmpList.Add(n3);

        //ShowDialogInput(tmpList, 5);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(isAction)
        {
            if (waitTimer < waitTime)
            {
                waitTimer += Time.deltaTime;
                isEnd = true;

                foreach (var item in btnElement)
                {
                    item.GetComponent<Button>().interactable = false;
                }
            }
            else
            {
                if (userInput != -1)
                {
                    isEnd = true;
                    isAction = false;
                    foreach (var item in btnElement)
                    {
                        item.GetComponent<Button>().interactable = false;
                    }
                }
                else
                {
                    isEnd = false;
                    foreach (var item in btnElement)
                    {
                        item.GetComponent<Button>().interactable = true;
                    }

                }
            }
        }
        else
        {
            isEnd = true;
        }


        currentTime = Mathf.Clamp(isEnd ? currentTime - Time.deltaTime : currentTime + Time.deltaTime, 0, animationTime);

        float tmp = AnimationExtenstion.EaseInOutSine(currentTime / animationTime);
        layout.transform.localPosition = Vector2.Lerp(originPos + offset, originPos, tmp);

        var c = imgBackground.color;
        c.a = tmp * 0.3f;
        imgBackground.color = c;
        
        foreach(var item in btnElement)
        {
            c = item.GetComponent<Image>().color;
            c.a = tmp;
            item.GetComponent<Image>().color = c;

            c = item.transform.GetChild(0).GetComponent<Text>().color;
            c.a = tmp;
            item.transform.GetChild(0).GetComponent<Text>().color = c;
        }
    }

    public void ShowDialogInput(List<ScriptNode> input, float waitTime)
    {
        Clear();
        isAction = true;

        foreach (var item in input)
        {
            var obj = Instantiate(template, layout.transform);
            obj.SetActive(true);
            var c = obj.GetComponent<Image>().color;
            c.a = 0;
            obj.GetComponent<Image>().color = c;

            c = obj.transform.GetChild(0).GetComponent<Text>().color;
            c.a = 0;
            obj.transform.GetChild(0).GetComponent<Text>().color = c;

            obj.transform.GetChild(0).GetComponent<Text>().text = item.GetScriptContent();
            int x = input.IndexOf(item);
            obj.GetComponent<Button>().onClick.AddListener(() => { UserInput(x); });

            btnElement.Add(obj);
        }

        this.waitTime = waitTime;
    }

    public void UserInput(int input)
    {
        userInput = input;
        Debug.Log("Input : " + input);

        isEnd = true;
    }

    public void Clear()
    {
        isAction = false;
        isEnd = false;
        userInput = -1;
        foreach (var item in btnElement)
        {
            Destroy(item);
        }

        btnElement.Clear();
    }
}
