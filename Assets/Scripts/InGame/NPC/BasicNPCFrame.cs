using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.AI;

[System.Serializable]
public enum NPCStatus
{
    MOVE, WAIT, TALK
}

[System.Serializable]
public class BasicNPCFrame : MonoBehaviour
{
    [Header("NPC Component")]
    [SerializeField] NavMeshAgent agent;
    public Transform tr;
    

    [Header("NPC Status")]
    public NPCStatus status = NPCStatus.WAIT;

    [Header("NPC Behaviour Setting")]
    public bool stopTracking;
    public float moveSpeed = 2;
    public float minMoveDist = 2;
    public float minDetectDist = 0.3f;
    public Vector3 moveTarget;
    public Vector3 prePos;
    public Animator anim;
    public float waitTime = 1;

    [Header("NPC Life Time")]
    public float lifeTime = 120;
    public float maxLifeTime = 300;
    public float minLifeTIme = 120;

    [Header("NPC Map Border")]
    public Vector2 leftBottom;
    public Vector2 rightTop;
    public float offsetX;
    public float offsetY;


    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(anim == null)
            anim = GetComponent<Animator>();

        lifeTime = Random.Range(minLifeTIme, maxLifeTime);
        offsetX = rightTop.x - leftBottom.x;
        offsetY = rightTop.y - leftBottom.y;

        agent.speed = moveSpeed;

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if(tr == null)
            tr = transform;
        prePos = tr.position;
        StartCoroutine(NPCCore());
    }

    public IEnumerator NPCCore()
    {
        while (true)
        {
            yield return null;

            switch (status)
            {
                case NPCStatus.MOVE:
                    agent.isStopped = false;
                    agent.SetDestination(moveTarget);
                    anim.SetBool("isMove", true);

                    if (Vector3.Distance(moveTarget, transform.position) < minDetectDist)
                        status = NPCStatus.WAIT;

                    if (tr.position.x >= prePos.x)
                        tr.localScale = new Vector3(1, 1, 0);
                    else
                        tr.localScale = new Vector3(-1, 1, 0);

                    prePos = tr.position;
                    break;

                case NPCStatus.WAIT:
                    yield return new WaitForSeconds(Random.Range(waitTime - 1, waitTime + 1));
                    moveTarget = Vector3.zero;

                    if (status == NPCStatus.TALK)
                        break;

                    while(true)
                    {
                        yield return null;

                        moveTarget.x = leftBottom.x + Random.Range(0, offsetX);
                        moveTarget.y = leftBottom.y + Random.Range(0, offsetY);

                        if(Vector3.Distance(moveTarget, transform.position) > minMoveDist)
                        {
                            Vector3 p = moveTarget;
                            RaycastHit2D hit = Physics2D.Raycast(moveTarget, Vector2.zero,
                                100f);
                            if (hit.collider != null)
                            {
                                status = NPCStatus.MOVE;
                                break;
                            }
                        }
                    }

                    anim.SetBool("isMove", false);
                    break;
                case NPCStatus.TALK:
                    agent.isStopped = true;
                    anim.SetBool("isMove", false);
                    break;
            }
        }
    }
}
