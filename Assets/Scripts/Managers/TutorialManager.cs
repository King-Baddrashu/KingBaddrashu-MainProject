using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Game Story")]
    public List<string> storyTxt;

    [Header("Fade In Fade Out Animation")]
    public Image fadeAnim;
    [Tooltip("스토리 보여주는 이미지")]
    public Text textImg;
    public float fadeMaxTime = 5;
    public float waitTime = 3;
    public bool skipStory;
    public int skipCount = 0;

    [Header("Episode Path")]
    public string episodePath;
    public UIDialogManager dialogMgr;

    // Start is called before the first frame update
    void Start()
    {
        dialogMgr.isActiveInput = false;

        StartCoroutine(GameStory());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            skipStory = true;
            skipCount++;
        }
    }

    void SetImageAlpha(Image img, float v)
    {
        var c = img.color;
        c.a = v;
        img.color = c;
    }
    void SetTextAlpha(Text img, float v)
    {
        var c = img.color;
        c.a = v;
        img.color = c;
    }

    public IEnumerator GameStory()
    {
        // 1. 3초마다 변경
        foreach( var item in storyTxt)
        {
            textImg.text = item;

            // 1-1. Text Animation Transition 적용
            StartCoroutine(FadeInText(textImg));
            yield return new WaitForSeconds(waitTime);
            StartCoroutine(FadeOutText(textImg));
            yield return new WaitForSeconds(fadeMaxTime);

            if (skipStory)
                break;
        }

        if(skipCount >= 20)
        {
            textImg.text = "스킵좀 그만해 이자식아...\n그럼 이제 시작합니다!";
            StartCoroutine(FadeInText(textImg));
            yield return new WaitForSeconds(waitTime);
            StartCoroutine(FadeOutText(textImg));
            yield return new WaitForSeconds(fadeMaxTime);
        }

        // 스크립트 파일 로드함
        ScriptData data = new ScriptData();
        ScriptDataFormatter.LoadScriptData(out data, PathExtenstion.GetScriptPath(episodePath));
        dialogMgr.ChangeScriptData(data);

        StartCoroutine(FadeOutImg(fadeAnim));
        dialogMgr.isActiveInput = true;
        
        while(true)
        {
            if (dialogMgr.isScriptEnd)
            {
                StartCoroutine(FadeInImg(fadeAnim));
                break;
            }

            yield return null;
        }

        yield return new WaitForSeconds(fadeMaxTime + 0.1f);
        // 씬 이동
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public IEnumerator FadeInImg(Image img)
    {
        float v = 0;

        for (; v < 1f;)
        {
            v += Time.deltaTime / fadeMaxTime;
            SetImageAlpha(img, v);
            yield return null;
        }

        SetImageAlpha(img, 1.0f);
    }

    public IEnumerator FadeOutImg(Image img)
    {
        float v = 1;

        for (; v > 0f;)
        {
            v -= Time.deltaTime / fadeMaxTime;
            SetImageAlpha(img, v);
            yield return null;
        }

        SetImageAlpha(img, 0.0f);
    }
    public IEnumerator FadeInText(Text img)
    {
        float v = 0;

        for (; v <= 1f;)
        {
            v += Time.deltaTime / fadeMaxTime;
            SetTextAlpha(img, v);
            yield return null;
        }

        SetTextAlpha(img, 1.0f);
    }

    public IEnumerator FadeOutText(Text img)
    {
        float v = 1;

        for (; v > 0f;)
        {
            v -= Time.deltaTime / fadeMaxTime;
            SetTextAlpha(img, v);
            yield return null;
        }

        SetTextAlpha(img, 0.0f);
    }
}