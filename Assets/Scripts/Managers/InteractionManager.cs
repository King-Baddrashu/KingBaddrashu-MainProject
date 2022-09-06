using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    public List<GameObject> gamaObject;
    public PlayerMovement player;
    public InteractionObject currentInteractionObject;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 100f);

            if (hit.collider.tag.CompareTo("Interaction Object") == 0)
            {
                currentInteractionObject = hit.collider.gameObject.GetComponent<InteractionObject>();
            }
        }

        if(player.isMoveDone)
        {
            if(currentInteractionObject != null)
            player.isInteraction = true;
        }
    }
}
