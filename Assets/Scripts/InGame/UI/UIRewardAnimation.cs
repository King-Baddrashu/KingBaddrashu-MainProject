using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRewardAnimation : MonoBehaviour
{
    public UnityEngine.UI.Text text;
    public float lifeTime = 1;
    public float curTime = 0;
    public float moveSpeed = 2;

    // Start is called before the first frame update
    void Start()
    {
        curTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;

        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        var c = text.color;
        c.a =  1.0f - curTime / lifeTime;
        text.color = c;

        if(curTime >= lifeTime)
        {
            Destroy(this.gameObject);
        }
    }
}
