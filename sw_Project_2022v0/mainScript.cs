using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using Google.MiniJSON;
using System.IO;
using Unity.VisualScripting;
using System.Linq;
using System.Threading;

public class mainScript : MonoBehaviour
{
    public TMP_Text txt_nicknameUI;
    public TMP_Text txt_scoreUI;
    public Button btnObject;

    private DatabaseReference databaseReference;
    private Queue<Action> m_queueAction = new Queue<Action>();

    public GameObject img_rank;
    public GameObject img_rankTitle;
    public GameObject rank1;
    public GameObject rank2;
    public GameObject rank3;
    public GameObject rank4;
    public GameObject rank5;
    public GameObject rank6;
    public GameObject rank7;
    public TMP_Text txt_rank1;
    public TMP_Text txt_rank2;
    public TMP_Text txt_rank3;
    public TMP_Text txt_rank4;
    public TMP_Text txt_rank5;
    public TMP_Text txt_rank6;
    public TMP_Text txt_rank7;
    public GameObject btn_exit;

    public GameObject img_errorBack;
    public GameObject img_error;
    public GameObject btn_errorExit_1;
    public GameObject btn_errorExit_2;
    public GameObject txt_errorLog;

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

    public void popRankFalse()
    {
        img_rank.SetActive(false);
        img_rankTitle.SetActive(false);
        rank1.SetActive(false);
        rank2.SetActive(false);
        rank3.SetActive(false);
        rank4.SetActive(false);
        rank5.SetActive(false);
        rank6.SetActive(false);
        rank7.SetActive(false);
        btn_exit.SetActive(false);
    }

    public void popRankTrue()
    {
        img_rank.SetActive(true);
        img_rankTitle.SetActive(true);
        rank1.SetActive(true);
        rank2.SetActive(true);
        rank3.SetActive(true);
        rank4.SetActive(true);
        rank5.SetActive(true);
        rank6.SetActive(true);
        rank7.SetActive(true);
        btn_exit.SetActive(true);
    }

    public void Start()
    {
        popFalse();
        popRankFalse();
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
    }

    public void SceneChange()
    {
        SceneManager.LoadScene("2_titleScene");
    }

    public void btnComputer()
    {
        btnObject.GetComponent<Animator>().Play("computer");
        Invoke("SceneChange", 2);
    }

    public static Dictionary<string, int> SortDictionary(Dictionary<string, int> dict)
    {
        return dict.OrderByDescending(item => item.Value).ToDictionary(x=> x.Key, x=> x.Value);
    }

    public void btnRank()
    {
        popRankTrue();
        IDictionary<string, string> user = new Dictionary<string, string>();
        Dictionary<string, int> rankData = new Dictionary<string, int>();
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        DatabaseReference userInfo = FirebaseDatabase.DefaultInstance.GetReference("user");
        userInfo.GetValueAsync().ContinueWith(task =>
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
                foreach (DataSnapshot data in snapshot.Children)
                {
                    IDictionary userData = (IDictionary)data.Value;
                    user.Add(userData["nickname"].ToString(), userData["score"].ToString());
                }
                foreach (var item in user)
                {
                    rankData.Add(item.Key, int.Parse(item.Value));
                }
            }
            rankData = SortDictionary(rankData);

            int idx = 1;
            int i = 0;
            string scoreStr = "";
            string[] scoreArr = new string[7] {"", "", "", "", "", "", ""};
            try
            {
                foreach (var item in rankData)
                {
                    scoreStr = $"{idx++}위 : {item.Key}님 , 점수 : {item.Value}";
                    scoreArr[i] = scoreStr;
                    i++;
                }
                Thread thread = new Thread(() =>
                {
                    m_queueAction.Enqueue(() =>
                    {
                        txt_rank1.text = scoreArr[0];
                        txt_rank2.text = scoreArr[1];
                        txt_rank3.text = scoreArr[2];
                        txt_rank4.text = scoreArr[3];
                        txt_rank5.text = scoreArr[4];
                        txt_rank6.text = scoreArr[5];
                        txt_rank7.text = scoreArr[6];
                    });
                });
                thread.Start();
            } catch(Exception e)
            {
                Debug.Log(e);
            }
        });
    }

    public void btnExit()
    {
        SceneManager.LoadScene("1_mainScene");
    }
}
