using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class titleScript : MonoBehaviour
{
    public GameObject img_errorBack;
    public GameObject img_error;
    public GameObject btn_errorExit_1;
    public GameObject btn_errorExit_2;
    public GameObject txt_errorLog;

    public TMP_Text txt_nicknameUI;
    public TMP_Text txt_scoreUI;
    public Image    btn_login;
    public Sprite   change_img;
    public Sprite   orign_img;

    public void popFalse()
    {
        img_errorBack.SetActive(false);
        img_error.SetActive(false);
        btn_errorExit_1.SetActive(false);
        btn_errorExit_2.SetActive(false);
        txt_errorLog.SetActive(false);
    }

    public void popTrue()
    {
        img_errorBack.SetActive(true);
        img_error.SetActive(true);
        btn_errorExit_1.SetActive(true);
        btn_errorExit_2.SetActive(true);
        txt_errorLog.SetActive(true);
    }

    public void btnErrorExit()
    {
        popFalse();
    }

    public void Start()
    {
        popFalse();
        btn_login.GetComponent<Image>().sprite = orign_img;
        try
        {
            if (singletonWithUI.instance.txt_nickname != null && singletonWithUI.instance.txt_score != null)
            {
                txt_nicknameUI.text = singletonWithUI.instance.txt_nickname;
                txt_scoreUI.text = singletonWithUI.instance.txt_score;
                btn_login.GetComponent<Image>().sprite = change_img;
            }
        }
        catch
        {
        }
    }

    public void btnStart()
    {
        try
        {
            if (singletonWithUI.instance.txt_nickname != null && singletonWithUI.instance.txt_score != null && singletonWithUI.instance.txt_nickname != "" && singletonWithUI.instance.txt_score != "")
            {
                SceneManager.LoadScene("5_lobbyScene");
            }
            else
            {
                popTrue();
                txt_errorLog.GetComponent<TMP_Text>().text = "ERROR CODE :\r\n 로그인되어 있지 않습니다.";
            }
        }
        catch
        {
            popTrue();
            txt_errorLog.GetComponent<TMP_Text>().text = "ERROR CODE :\r\n 로그인되어 있지 않습니다.";
        }
    }

    public void btnLogin()
    {
        SceneManager.LoadScene("4_loginScene");
    }
    
    public void btnWithEmail()
    {
        SceneManager.LoadScene("3_signWithEmailScene");
    }
}
