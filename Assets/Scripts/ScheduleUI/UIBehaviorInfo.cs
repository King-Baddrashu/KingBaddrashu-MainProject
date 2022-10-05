using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIBehaviorInfo : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDropHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    Image frame;

    [Header("UI Behavior Element")]
    public Image imgPlace;
    public Text textBehavior;
    public Text cancleInfo;
    public Image cancleImage;
    public ScheduleManager.PlaceType placeType;
    public ScheduleManager.BehaviourType behaviourType;


    [Header("UI Animation Setting")]
    public Image imgFrame;
    public float animationTime;
    private float currentAnimationTime;
    public bool animTrigger;
    public bool isShow;
    public Vector3 startPos;
    public Vector3 endPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = textBehavior.transform.localPosition;
        endPos = startPos + (130 * Vector3.right);

        Color tmp = cancleInfo.color;
        tmp.a = 0;
        cancleInfo.color = tmp;

        tmp = cancleImage.color;
        tmp.a = 0.3f;
        cancleImage.color = tmp;
        isShow = false;

        frame = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShow)
        {
            frame.color = Color.clear;
            
            imgPlace.sprite = null;
            textBehavior.text = "";
            placeType = ScheduleManager.PlaceType.NONE;
            behaviourType = ScheduleManager.BehaviourType.NONE;

            imgPlace.enabled = false;
            textBehavior.enabled = false;
            cancleImage.enabled = false;
            cancleInfo.enabled = false;
            return;
        }
        else
        {
            frame.color = Color.white;
            imgPlace.enabled = true;
            textBehavior.enabled = true;
            cancleImage.enabled = true;
            cancleInfo.enabled = true;
        }

        currentAnimationTime = Mathf.Clamp(animTrigger ?
            currentAnimationTime + Time.deltaTime :
            currentAnimationTime - Time.deltaTime,
            0, animationTime);
        float tmpAnimValue = AnimationExtenstion.EaseInOutCubic(currentAnimationTime / animationTime);
        Vector3 tmpPos = Vector3.Lerp(startPos, endPos, tmpAnimValue);

        textBehavior.transform.localPosition = tmpPos;

        Color tmpColor = imgPlace.color;
        tmpColor.a = tmpAnimValue;
        imgPlace.color = tmpColor;

        tmpColor = cancleImage.color;
        tmpColor.a = tmpAnimValue * 0.3f;
        cancleImage.color = tmpColor;

        tmpColor = cancleInfo.color;
        tmpColor.a = tmpAnimValue;
        cancleInfo.color = tmpColor;
    }

    public void UpdateBehaviorInfo(Sprite img, string text, ScheduleManager.PlaceType placeType, ScheduleManager.BehaviourType behaviourType)
    {
        imgPlace.sprite = img;
        textBehavior.text = text;
        animTrigger = false;
        currentAnimationTime = 0;

        this.placeType = placeType;
        this.behaviourType = behaviourType;

        if (textBehavior.text.CompareTo("") == 0)
            isShow = false;
        else
            isShow = true;
    }

    public void Clear()
    {
        Color tmp = cancleInfo.color;
        tmp.a = 0;
        cancleInfo.color = tmp;

        tmp = cancleImage.color;
        tmp.a = 0.3f;
        cancleImage.color = tmp;
        
        this.placeType = ScheduleManager.PlaceType.NONE;
        this.behaviourType = ScheduleManager.BehaviourType.NONE;

        isShow = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isShow = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animTrigger = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animTrigger = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            var gm = eventData.pointerDrag;

            if (gm.GetComponent<UIBehaviorInfo>() != null)
            {
                bool isShow = this.isShow;
                var tmpData = textBehavior.text;
                var tmpImg = imgPlace.sprite;
                var tmpPlaceType = placeType;
                var tmpBehaviourType = behaviourType;

                var changeTarget = gm.GetComponent<UIBehaviorInfo>();
                bool tmpisShow = this.isShow;
                this.isShow = changeTarget.isShow;

                changeTarget.isShow = tmpisShow;
                this.UpdateBehaviorInfo(changeTarget.imgPlace.sprite, changeTarget.textBehavior.text, changeTarget.placeType, changeTarget.behaviourType);
                changeTarget.UpdateBehaviorInfo(tmpImg, tmpData, tmpPlaceType, tmpBehaviourType);
            }

            if (gm.GetComponent<UIPlace>() != null)
            {
                var dataPlace = gm.GetComponent<UIPlace>().dataPlaceImage;
                var dataBehaviour = gm.GetComponent<UIPlace>().dataPlaceText;
                UpdateBehaviorInfo(dataPlace, dataBehaviour, gm.GetComponent<UIPlace>().placeType, gm.GetComponent<UIPlace>().behaviourType);
            }
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(ScheduleManager.instance.dragDropUI.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var mousePosition))
        {
            ScheduleManager.instance.dragDropUI.transform.position = mousePosition;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isShow) return;

        ScheduleManager.instance.isDragged = true;
        var drag = ScheduleManager.instance.dragDropUI.GetComponent<UIDragTarget>();
        drag.UpdateBehaviorInfo(imgPlace.sprite, textBehavior.text, placeType, behaviourType);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ScheduleManager.instance.isDragged = false;
        ScheduleManager.instance.dragDropUI.gameObject.SetActive(false);
    }
}
