using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum NPCStatus
{
    MOVE, WAIT, TALK
}

[System.Serializable]
public abstract class BasicNPCFrame : MonoBehaviour
{
    public abstract void Move();
    public abstract void Talk();
    public abstract void Wait();
    public abstract IEnumerator NPCCore();
}

[System.Serializable]
public class NPCFrame : BasicNPCFrame {
    [Header("NPC Component")]
    Transform transform;

    [Header("NPC Status")]
    NPCStatus status = NPCStatus.WAIT;

    [Header("NPC Behaviour Setting")]
    float moveSpeed = 2;
    Vector3 moveTarget;
    float waitTime = 1;
    float curWaitTime = 0;

    [Header("NPC Life Time")]
    float lifeTime = 120;
    float maxLifeTime = 300;
    float minLifeTIme = 120;

    [Header("NPC Map Border")]
    public Vector2 leftBottom;
    public Vector2 rightTop;
    public float offsetX;
    public float offsetY;


    public void Start()
    {
        lifeTime = Random.Range(minLifeTIme, maxLifeTime);
        offsetX = leftBottom.x - rightTop.x;
        offsetY = rightTop.y - leftBottom.y;
    }

    public override IEnumerator NPCCore()
    {
        while (true)
        {
            switch (status)
            {
                case NPCStatus.MOVE: Move(); break;
                case NPCStatus.WAIT: Wait(); break;
                case NPCStatus.TALK: Talk(); break;
            }

            yield return null;
        }
    }

    public override void Move()
    {
        // NavMesh Code
        // Follow Target
    }

    public override void Talk()
    {
        // d
    }

    public override void Wait()
    {
        // d
        if(curWaitTime == 0)
        {

        }
        if(curWaitTime >= waitTime)
        {

        }
    }
}
