using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Move AI")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] public Vector3 targetPos;
    public LayerMask groundMask;
    public Transform tr;
    public float stopDistance;
    public bool isMoveDone;
    public bool isInteraction;
    public Vector2 prePos;
    public Animator anim;
    

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoveDone)
        {
            float dis = Vector3.Distance(tr.position, targetPos);
            
            if (dis < stopDistance)
            {
                isMoveDone = true;
            }

            if (tr.position.x >= prePos.x)
                tr.localScale = new Vector3(1, 1, 0);
            else
                tr.localScale = new Vector3(-1, 1, 0);

            prePos = transform.position;
            anim.SetFloat("Speed", 1);
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }
    }

    public void MovePos(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        agent.SetDestination(targetPos);
        isMoveDone = false;
    }
}
