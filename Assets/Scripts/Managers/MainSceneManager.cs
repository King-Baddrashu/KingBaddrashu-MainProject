using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    public bool isMain = true;
    public bool showEsc = true;
    public GameObject escObj;

    public GameObject background;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMain)
            background.transform.localPosition = Random.insideUnitCircle * 5;
        else if (Input.GetKeyDown(KeyCode.Escape))
            showEsc = !showEsc;

        escObj.SetActive(showEsc);
    }

    public void StartNewGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TutorialScene");
    }
    public void GoMain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void StopGame()
    {
        Application.Quit();
    }

}
