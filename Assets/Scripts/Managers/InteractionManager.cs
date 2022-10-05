using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    public List<GameObject> gamaObject;
    public PlayerMovement player;
    public InteractionObject currentInteractionObject;

    [Header("UI Element")]
    public GameObject uiInfomation;
    public GameObject uiRewardTemplate;

    public ActionData tmpActionData;

    [Header("UI Interaction")]
    public NPCDialogManager talkTarget;
    public bool isTalk;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        tmpActionData = null;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D groundhit = Physics2D.Raycast(pos, Vector2.zero, 100f, LayerMask.GetMask("Ground"));
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 100f, LayerMask.GetMask("Interaction"));
        if(groundhit.collider != null && !(player.isInteraction && player.isMoveDone))
        {
            if(Input.GetMouseButtonDown(0))
            player.MovePos(pos);
        }

        if(hit.collider != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!(player.isInteraction && player.isMoveDone))
                {
                    if (hit.collider.tag.CompareTo("Interaction Object") == 0)
                    {
                        // Change Interaction Object
                        currentInteractionObject = hit.collider.gameObject.GetComponent<InteractionObject>();
                    }
                    else if(hit.collider.tag.CompareTo("NPC") == 0)
                    {
                        talkTarget = hit.collider.gameObject.GetComponent<NPCDialogManager>();
                    }
                    else
                    {
                        currentInteractionObject = null;
                        talkTarget = null;
                    }
                }
            }
            if (hit.collider.tag.CompareTo("Interaction Object") == 0)
            {
                if (currentInteractionObject == hit.collider.gameObject.GetComponent<InteractionObject>())
                {
                    //Do not show infomation
                    uiInfomation.SetActive(false);
                }

                var interaction = hit.collider.gameObject.GetComponent<InteractionObject>();

                // Show Infomation
                uiInfomation.SetActive(true);
                uiInfomation.GetComponent<UIShowInteraction>().ChangeInfo(interaction.interactionName, interaction.interactionImg, interaction.GetRewardInfomation());
                uiInfomation.GetComponent<UIShowInteraction>().Move(pos);
            }
            else
            {
                //Do not show inomation
                uiInfomation.SetActive(false);
            }
        }
        else
        {
            //Do not show infomation
            uiInfomation.SetActive(false);
        }

        if (player.isMoveDone)
        {
            if(currentInteractionObject != null)
            {
                player.isInteraction = true;

                if(tmpActionData == null)
                {
                    //Check can action
                    tmpActionData = PlayerStatusManager.instance.CheckAction();
                    var info = uiRewardTemplate;
                    info = Instantiate(info);
                    info.transform.position = player.transform.position + Vector3.up * 0.5f;
                    info.SetActive(true);
                    info.GetComponent<UIRewardAnimation>().text.text = tmpActionData.script;

                    if (!tmpActionData.isAction)
                    {
                        player.isInteraction = false;
                        currentInteractionObject = null;
                        tmpActionData = null;
                        return;
                    }
                }

                // Start Interaction
                currentInteractionObject.StartAction();

                if(currentInteractionObject.isDone)
                {
                    currentInteractionObject.isDone = false;
                    player.isInteraction = false;
                    currentInteractionObject = null;
                    tmpActionData = null;
                }
            }
        }

        if (talkTarget != null)
        {
            if (!isTalk)
            {
                player.isInteraction = true;

                bool tmp = talkTarget.StartDialog();

                if (!tmp)
                {
                    isTalk = false;
                    player.isInteraction = false;
                    talkTarget = null;

                    GameObject info = Instantiate(uiRewardTemplate);
                    info.transform.position = player.transform.position + Vector3.up * 0.5f;
                    info.SetActive(true);
                    info.GetComponent<UIRewardAnimation>().text.text = "±î¿´´Ù....";
                }
                else { 
                    isTalk = true;
                }
            }

            if (isTalk)
            {
                if (!talkTarget.isEnd)
                {
                    player.isInteraction = true;
                }
                else if (talkTarget.isEnd)
                {
                    talkTarget = null;
                    player.isInteraction = false;
                }
            }
        }
    }

    public void EndTalk()
    {
        isTalk = false;
        talkTarget.frame.status = NPCStatus.WAIT;
        talkTarget = null;
        player.isInteraction = false;
    }
}
