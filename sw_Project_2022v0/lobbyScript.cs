using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class lobbyScript : MonoBehaviour
{
    public TMP_Text txt_nicknameUI;
    public TMP_Text txt_scoreUI;
    
    public void Start()
    {
        try
        {
            if (singletonWithUI.instance.txt_nickname != null && singletonWithUI.instance.txt_score != null)
            {
                txt_nicknameUI.text = singletonWithUI.instance.txt_nickname;
                txt_scoreUI.text = singletonWithUI.instance.txt_score;
            }
            else
            {
                txt_nicknameUI.text = "로그인 필요";
                txt_scoreUI.text = "";
            }
        }
        catch
        {
        }
    }

    public void btnClickSolo()
    {
        singletonWithTimer.instance.gameInit();
        singletonWithTimer.instance.timerOn();
        SceneManager.LoadScene("6_minGame01");
    }

    public void btnClickExit()
    {
        SceneManager.LoadScene("1_mainScene");
    }

    public void btnClickLogout()
    {
        SceneManager.LoadScene("2_titleScene");
    }
}
