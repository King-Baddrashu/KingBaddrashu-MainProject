using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDragTarget : MonoBehaviour
{
    public Image imgPlace;
    public Text textBehavior;

    public ScheduleManager.PlaceType placeType;
    public ScheduleManager.BehaviourType behaviourType;

    public void UpdateBehaviorInfo(Sprite img, string text, ScheduleManager.PlaceType placeType, ScheduleManager.BehaviourType behaviourType)
    {
        imgPlace.sprite = img;
        textBehavior.text = text;

        this.placeType = placeType;
        this.behaviourType = behaviourType;

        gameObject.SetActive(true);
    }
}
