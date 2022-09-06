using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIAnimation : MonoBehaviour
{
    [System.Serializable]
    public enum UIStatus
    {
        MAIN = 0, LOAD = 1, CREDIT = 2, OPTION = 3
    }

    [Header("UI Status")]
    public UIStatus status;
    public UIStatus preStatus;

    [Header("UI Object List")]
    public SerializableDictionary<UIStatus, GameObject> uiObject;

    [Header("UI Animation Option")]
    public float animationTime;
    public float currentAnimationTime;
    public float offset = 1000;
    public Vector3 leftWaitPos;
    public Vector3 rightWaitPos;
    public Vector3 centerShowPos;


    // Start is called before the first frame update
    void Start()
    {
        centerShowPos = uiObject[UIStatus.MAIN].transform.localPosition * ScreenResolutionExtenstion.GetScreenIncreaseRatioAuto();
        leftWaitPos = centerShowPos + Vector3.left * offset * ScreenResolutionExtenstion.GetScreenIncreaseRatioAuto();
        rightWaitPos = centerShowPos + Vector3.right * offset * ScreenResolutionExtenstion.GetScreenIncreaseRatioAuto();

        foreach(var obj in uiObject)
        {
            obj.Value.SetActive(true);
            if(obj.Key != UIStatus.MAIN)
            {
                obj.Value.transform.localPosition = rightWaitPos;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // UI Animation Part
        currentAnimationTime = Mathf.Clamp(status != UIStatus.MAIN ?
            currentAnimationTime + Time.deltaTime :
            currentAnimationTime - Time.deltaTime,
            0, animationTime);

        float tmpAnimValue = AnimationExtenstion.EaseInOutExpo(currentAnimationTime/animationTime);
        Vector3 mainPos = Vector3.Lerp(centerShowPos, leftWaitPos, tmpAnimValue);
        Vector3 currentShowPos = Vector3.Lerp(rightWaitPos, centerShowPos, tmpAnimValue);

        uiObject[UIStatus.MAIN].transform.localPosition = mainPos;
        
        if(status != UIStatus.MAIN)
            uiObject[status].transform.localPosition = currentShowPos;
        
        foreach (var obj in uiObject)
        {
            if (obj.Key == UIStatus.MAIN || obj.Key == status) continue;

            var tmpTr = obj.Value.transform;
            tmpTr.localPosition = rightWaitPos;
        }

        if (preStatus != UIStatus.MAIN)
            uiObject[preStatus].transform.localPosition = currentShowPos;

    }
    
    public void ChangeStatus(int status)
    {
        preStatus = this.status;
        this.status = (UIStatus)status;
    }
}
