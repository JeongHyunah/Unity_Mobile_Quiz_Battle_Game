using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Firebase.Database;

public class loginScript : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseAuth user;
    private DatabaseReference databaseReference;
    private Queue<Action> m_queueAction = new Queue<Action>();

    public TMP_InputField   lbl_email;
    public TMP_InputField   lbl_password;
    public Sprite           img_orignImage;
    public Sprite           img_changeImage;
    public Image            img_emailCheck;
    public TMP_Text         txt_emailCheckLog;

    public TMP_Text txt_nicknameUI;
    public TMP_Text txt_scoreUI;

    private bool emailDataCheck;
    private bool emailData_return;
    private string user_nickname;
    private string user_score;

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

    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void Start()
    {
        popFalse();
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        singletonWithUI.instance.txt_nickname = null;
        singletonWithUI.instance.txt_score = null;
        singletonWithUI.instance.uid = null;
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
        auth.SignOut();
        Debug.Log("로그아웃");
    }

    void Update()
    {
        while (m_queueAction.Count > 0)
        {
            m_queueAction.Dequeue().Invoke();
        }
    }

    public void btnCheck()
    {
        Task.Run(() => emailCheck(lbl_email.text.Trim()));
    }

    async void emailCheck(string email)
    {
        await Task.Run(() => emailData(email));

        await Task.Delay(500);

        await Task.Run(() =>
        {
            Thread thread = new Thread(() =>
            {
                m_queueAction.Enqueue(() =>
                {
                    if (emailDataCheck && email != null && email != "")
                    {

                        img_emailCheck.GetComponent<Image>().sprite = img_orignImage;
                        txt_emailCheckLog.GetComponent<TMP_Text>().text = "로그인 가능";
                        emailData_return = true;
                    }
                    else
                    {
                        img_emailCheck.GetComponent<Image>().sprite = img_changeImage;
                        txt_emailCheckLog.GetComponent<TMP_Text>().text = "계정 정보 없음";
                        emailData_return = false;
                    }
                });
            });
            thread.Start();
        });
    }

    async void emailData(string email)
    {
        await Task.Run(() =>
        {
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            databaseReference.Child("user_email").GetValueAsync().ContinueWith(task =>
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
                    foreach (var item in snapshot.Children)
                    {
                        if (email.Equals(item.Value))
                        {
                            emailDataCheck = true;
                            break;
                        }
                        emailDataCheck = false;
                    }
                }
            });
        });
    }

    public void btnLogin()
    {
        string emailText = lbl_email.text.Trim();
        string passwordText = lbl_password.text.Trim();

        emailCheck(emailText);

        if (emailData_return && emailText != null && passwordText != null)
        {
            auth.SignInWithEmailAndPasswordAsync(emailText, passwordText).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    popTrue();
                    txt_errorLog.GetComponent<TMP_Text>().text = "ERROR CODE : \r\n 데이터를 불러오지 못함! \r\n 인터넷 연결을 확인하세요";
                    return;
                }
                FirebaseUser newUser = task.Result;
                singletonWithUI.instance.uid = newUser.UserId;
                Task.Run(() => userData(newUser.UserId));
                Debug.LogError("로그인 완료");
            });
        }
    }

    async void userData(string uid)
    {
        await Task.Run(() =>
        {
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            databaseReference.Child("user").Child(uid).GetValueAsync().ContinueWith(task =>
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
                    IDictionary<string, object> userInfo = new Dictionary<string, object>();
                    foreach (var item in snapshot.Children)
                    {
                        userInfo.Add(item.Key, item.Value);
                    }
                    Thread thread = new Thread(() =>
                    {
                        m_queueAction.Enqueue(() =>
                        {
                            user_nickname = (string)userInfo["nickname"];
                            user_score = (string)userInfo["score"];

                            singletonWithUI.instance.txt_nickname = user_nickname;
                            singletonWithUI.instance.txt_score = user_score;
                            txt_nicknameUI.text = singletonWithUI.instance.txt_nickname;
                            txt_scoreUI.text = singletonWithUI.instance.txt_score;
                        });
                    });
                    thread.Start();
                }
            });
        });
    }

    public void btnExit()
    {
        SceneManager.LoadScene("2_titleScene");
    }
}
