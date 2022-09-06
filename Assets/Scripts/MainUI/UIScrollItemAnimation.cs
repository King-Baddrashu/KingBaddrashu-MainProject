using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScrollItemAnimation : MonoBehaviour
{
    [System.Serializable]
    public struct Item
    {
        public string value;
    }

    [Header("Item Template")]
    public GameObject template;

    [Header("View")]
    public RectTransform viewTr;

    [Header("Controll Buttons")]
    public Button btnLeft;
    public Button btnRight;

    [Header("Item Settings")]
    public List<Item> items;

    [Header("Animation Settings")]
    public List<RectTransform> itemTrs;
    public int value;
    public float animationSpeed;
    public float offset = 200;
    Vector3 leftOffset;
    Vector3 rightOffset;

    // Start is called before the first frame update
    void Start()
    {
        leftOffset = Vector3.left * offset * ScreenResolutionExtenstion.GetScreenIncreaseRatioAuto();
        rightOffset = Vector3.right * offset * ScreenResolutionExtenstion.GetScreenIncreaseRatioAuto();
        itemTrs = new List<RectTransform>();

        // Create Items
        foreach(Item item in items)
        {
            var tmp = Instantiate(template, viewTr);
            tmp.GetComponent<Text>().text = item.value;
            tmp.transform.localPosition = rightOffset;

            tmp.SetActive(true);
            itemTrs.Add(tmp.transform as RectTransform);
        }

        // Initialize Item Position
        for(int i = 0; i < itemTrs.Count; i++)
        {
            var anim = itemTrs[i];

            if (i < value)
                anim.transform.localPosition = rightOffset;
            else if (i == value)
                anim.transform.localPosition = Vector3.zero;
            else
                anim.transform.position = leftOffset;
        }

        // Initialize Button Function
        btnLeft.onClick.AddListener(() => { MoveLeft(); });
        btnRight.onClick.AddListener(() => { MoveRight(); });
    }

    private void Update()
    {
        for(int i = 0; i < items.Count; i++)
        {
            var item = itemTrs[i];

            if (i < value) item.localPosition = Vector3.Lerp(item.localPosition, leftOffset, animationSpeed * Time.deltaTime);
            else if (i == value) item.localPosition = Vector3.Lerp(item.localPosition, Vector3.zero, animationSpeed * Time.deltaTime);
            else if (i > value) item.localPosition = Vector3.Lerp(item.localPosition, rightOffset, animationSpeed * Time.deltaTime);
        }
    }

    void MoveLeft()
    {
        value -= 1;

        if (value <= 0)
        {
            value = 0;
            btnLeft.interactable = false;
        }
        else btnRight.interactable = true;
    }

    void MoveRight()
    {
        value += 1;

        if (value >= items.Count - 1) {
            value = items.Count - 1;
            btnRight.interactable = false;
        }
        else
        {
            btnLeft.interactable = true;
        }
    }
}
