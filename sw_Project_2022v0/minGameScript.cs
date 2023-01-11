using Firebase.Database;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class minGameScript : MonoBehaviour
{
    private DatabaseReference databaseReference;

    public TMP_Text txt_nicknameUI;
    public TMP_Text txt_scoreUI;
    public TMP_Text timer;

    public GameObject img_errorBack;
    public GameObject img_error;
    public GameObject btn_errorExit_1;
    public GameObject btn_errorExit_2;
    public GameObject btn_again;
    public GameObject txt_errorLog;

    public GameObject btn_inExit;
    public GameObject btn_inAgain;

    public void popFalse()
    {
        img_errorBack.SetActive(false);
        img_error.SetActive(false);
        btn_errorExit_1.SetActive(false);
        btn_errorExit_2.SetActive(false);
        btn_again.SetActive(false);
        txt_errorLog.SetActive(false);
    }

    public void ExitPopFalse()
    {
        img_errorBack.SetActive(false);
        img_error.SetActive(false);
        btn_errorExit_1.SetActive(false);
        btn_inExit.SetActive(false);
        btn_inAgain.SetActive(false);
        txt_errorLog.SetActive(false);
    }

    public void popTrue()
    {
        img_errorBack.SetActive(true);
        img_error.SetActive(true);
        btn_errorExit_1.SetActive(true);
        btn_errorExit_2.SetActive(true);
        btn_again.SetActive(true);
        txt_errorLog.SetActive(true);
    }

    public void ExitPopTrue()
    {
        img_errorBack.SetActive(true);
        img_error.SetActive(true);
        btn_errorExit_1.SetActive(true);
        btn_inExit.SetActive(true);
        btn_inAgain.SetActive(true);
        txt_errorLog.SetActive(true);
    }

    public void btnExit()
    {
        scoreUpdate();
        SceneManager.LoadScene("5_lobbyScene");
    }

    public void btnAgain()
    {
        scoreUpdate();
        singletonWithTimer.instance.gameInit();
        singletonWithTimer.instance.timerOn();
        SceneManager.LoadScene("6_minGame01");
    }

    public void btninExit()
    {
        SceneManager.LoadScene("5_lobbyScene");
    }

    public void btninAgain()
    {
        ExitPopFalse();
    }

    public void Start()
    {
        popFalse();
        ExitPopFalse();
        try
        {
            if (singletonWithUI.instance.txt_nickname != null && singletonWithUI.instance.txt_score != null)
            {
                txt_nicknameUI.text = singletonWithUI.instance.txt_nickname;
                txt_scoreUI.text = singletonWithUI.instance.txt_score;
            }
        }
        catch
        {
        }
    }

    public void Update()
    {
        try
        {
            singletonWithTimer.instance.Update();

            timer.text = singletonWithTimer.instance.txt_Time;
            if (singletonWithTimer.instance._gameOver)
            {
                int gameScore = singletonWithTimer.instance.txt_gameScore;
                txt_errorLog.GetComponent<TMP_Text>().text = "Game Over :\r\n 맞힌 문제 : " + gameScore;
                popTrue();
            }
        }
        catch
        {
        }
    }

    public void scoreUpdate()
    {
        int gameScore = singletonWithTimer.instance.txt_gameScore;
        string uid = singletonWithUI.instance.uid;

        string user_score = singletonWithUI.instance.txt_score;
        int _user_score = int.Parse(user_score);

        gameScore = gameScore + _user_score;
        user_score = gameScore.ToString();
        singletonWithUI.instance.txt_score = user_score;

        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        databaseReference.Child("user").Child(uid).Child("score").SetValueAsync(user_score);
    }

    public void btnGameExit()
    {
        txt_errorLog.GetComponent<TMP_Text>().text = "정말로 게임을 \r\n 종료하시겠습니까? \r\n 점수 기록 안됨!";
        ExitPopTrue();
    }
}
