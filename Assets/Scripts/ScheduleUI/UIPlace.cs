using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIPlace : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Place Infomation")]
    public Sprite dataPlaceImage;
    public string dataPlaceText;
    public ScheduleManager.PlaceType placeType;
    public ScheduleManager.BehaviourType behaviourType;

    [Header("UI Place Element")]
    public Image imgPlace;
    public Text textPlace;

    [Header("UI Animation Setting")]
    public float animationTime;
    private float currentAnimationTime;
    public bool animTrigger;
    public Vector3 startPos;
    public Vector3 endPos;

    // Start is called before the first frame update
    void Start()
    {
        currentAnimationTime = 0.0f;
        animTrigger = false;

        startPos = textPlace.transform.localPosition;
        endPos = startPos + (50 * Vector3.down);

        if(dataPlaceImage != null && dataPlaceText != null)
        {
            imgPlace.sprite = dataPlaceImage;
        }
        if(dataPlaceText != "")
        {
            textPlace.text = dataPlaceText;
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentAnimationTime = Mathf.Clamp(animTrigger ?
            currentAnimationTime + Time.deltaTime :
            currentAnimationTime - Time.deltaTime,
            0, animationTime);
        float tmpAnimValue = AnimationExtenstion.EaseInOutCubic(currentAnimationTime / animationTime);

        if (behaviourType != ScheduleManager.BehaviourType.NONE)
        {
            // Just ZoomIn
            float zoom = 1 + tmpAnimValue / 2;
            textPlace.transform.localScale = Vector3.one * zoom;
            return;
        }

        Vector3 tmpPos = Vector3.Lerp(startPos, endPos, tmpAnimValue);

        textPlace.transform.localPosition = tmpPos;

        Color tmpColor = imgPlace.color;
        tmpColor.a = tmpAnimValue;
        imgPlace.color = tmpColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (behaviourType == ScheduleManager.BehaviourType.NONE)
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                ScheduleManager.instance.ChangeView(placeType);
            }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animTrigger = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animTrigger = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (behaviourType == ScheduleManager.BehaviourType.NONE) return;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(ScheduleManager.instance.dragDropUI.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var mousePosition))
        {
            ScheduleManager.instance.dragDropUI.transform.position = mousePosition;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (behaviourType == ScheduleManager.BehaviourType.NONE) return;

        ScheduleManager.instance.isDragged = true;
        var drag = ScheduleManager.instance.dragDropUI.GetComponent<UIDragTarget>();
        drag.UpdateBehaviorInfo(dataPlaceImage, dataPlaceText, placeType, behaviourType);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (behaviourType == ScheduleManager.BehaviourType.NONE) return;

        ScheduleManager.instance.isDragged = false;
        ScheduleManager.instance.dragDropUI.gameObject.SetActive(false);
    }
}