using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShowInteraction : MonoBehaviour
{
    public Text textInteractionName;
    public Image imgInteraction;
    public Text textInteractionReward;
    public float moveSpeed = 5;

    public void Move(Vector3 moveTarget)
    {
        transform.position = Vector3.Lerp(transform.position, moveTarget, moveSpeed * Time.deltaTime);
    }

    public void ChangeInfo(string name, Sprite img, string info)
    {
        textInteractionName.text = name;
        imgInteraction.sprite = img;
        textInteractionReward.text = info;

        if (img != null)
            imgInteraction.SetNativeSize();
    }
}
