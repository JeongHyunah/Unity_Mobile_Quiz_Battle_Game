using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase.Database;
using System;
using Random = System.Random;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.Assertions.Must;
using System.Threading;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class txtQuiz : MonoBehaviour
{
    public TextMeshProUGUI question;
    [SerializeField] private GameObject YES;
    [SerializeField] private GameObject NO;
    private string correctAnswer;
    private string buttonName;

    private Queue<Action> m_queueAction = new Queue<Action>();
    private DatabaseReference databaseReference;
    private SpriteRenderer sprite;

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
        SceneManager.LoadScene("6_soloGame");
    }

    public void btninExit()
    {
        SceneManager.LoadScene("5_lobbyScene");
    }

    public void btninAgain()
    {
        ExitPopFalse();
    }

    void Start()
    {
        Init();
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

    void Update()
    {
        while (m_queueAction.Count > 0)
        {
            m_queueAction.Dequeue().Invoke();
        }
        singletonWithTimer.instance.Update();

        timer.text = singletonWithTimer.instance.txt_Time;
        if (singletonWithTimer.instance._gameOver)
        {
            popTrue();
            int gameScore = singletonWithTimer.instance.txt_gameScore;
            txt_errorLog.GetComponent<TMP_Text>().text = "Game Over :\r\n 맞힌 문제 : " + gameScore;
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

    void Init()
    {
        Random seed = new Random();
        int randomNum = seed.Next(1, 11);
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        DatabaseReference quiz = FirebaseDatabase.DefaultInstance.GetReference("quiz");

        string Q = "Q" + randomNum as string;
        Debug.Log("문제 번호: " + Q);
        quiz.Child(Q).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted) 
            {
                popTrue();
                txt_errorLog.GetComponent<TMP_Text>().text = "ERROR CODE : \r\n 데이터를 불러오지 못함! \r\n 인터넷 연결을 확인하세요";
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                IDictionary<string, object> quizInfo = new Dictionary<string, object>();
                try
                {
                    foreach (var item in snapshot.Children)
                    {
                        quizInfo.Add(item.Key, item.Value);
                    }
                    Thread thread = new Thread(() =>
                    {
                        m_queueAction.Enqueue(() =>
                            {
                                question.SetText((string)quizInfo["Q"]);
                                correctAnswer = (string)quizInfo["correctAnswer"];
                            });
                    });
                    thread.Start();
                }
                catch
                {
                }
            }
        });
    }

    public void btnClickYES()
    {
        buttonName = EventSystem.current.currentSelectedGameObject.name;

        correctAnswerCheck(buttonName);
    }

    public void btnClickNO()
    {
        buttonName = EventSystem.current.currentSelectedGameObject.name;

        correctAnswerCheck(buttonName);
    }

    public void correctAnswerCheck(string buttonName)
    {
        if (buttonName == correctAnswer)
        {
            singletonWithTimer.instance.txt_gameScore++;
            Debug.Log(singletonWithTimer.instance.txt_gameScore);
            SceneManager.LoadScene("6_minGame01");
        }
        else
        {
            SceneManager.LoadScene("6_minGame01");
        }
    }

    public void btnGameExit()
    {
        txt_errorLog.GetComponent<TMP_Text>().text = "정말로 게임을 \r\n 종료하시겠습니까? \r\n 점수 기록 안됨!";
        ExitPopTrue();
    }
}

