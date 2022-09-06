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
    

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInteraction) return;

        if(Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 100f, groundMask);

            if(hit.collider != null)
            {
                targetPos = pos;
                isMoveDone = false;
            }
        }

        agent.SetDestination(targetPos);

        if(!isMoveDone)
        {
            float dis = Vector3.Distance(tr.position, targetPos);

            if(dis < stopDistance)
            {
                isMoveDone = true;
            }
        }
    }
}
