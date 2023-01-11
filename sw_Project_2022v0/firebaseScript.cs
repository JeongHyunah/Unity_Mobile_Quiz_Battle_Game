using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;

public class firebaseScript : MonoBehaviour
{
    private FirebaseAuth        auth;
    private FirebaseAuth        user;
    private DatabaseReference   databaseReference;
    private Queue<Action> m_queueAction = new Queue<Action>();
    private bool    emailDataCheck;
    private bool    emailData_return;
    private bool    nicknameDataCheck;
    private bool    nicknameData_return;

    public TMP_InputField   lbl_email;
    public TMP_InputField   lbl_password;
    public TMP_InputField   lbl_passwordCheck;
    public TMP_InputField   lbl_nickname;
    public Image            img_emailCheck;
    public Image            img_pwCheck;
    public Image            img_nicknameCheck;
    public Sprite           img_changeImage;
    public Sprite           img_orignImage;
    public TMP_Text         txt_pwCheckLog;
    public TMP_Text         txt_emailCheckLog;
    public TMP_Text         txt_nicknameCheckLog;

    public TMP_Text txt_nicknameUI;
    public TMP_Text txt_scoreUI;

    public GameObject img_errorBack;
    public GameObject img_error;
    public GameObject btn_errorExit_1;
    public GameObject btn_errorExit_2;
    public GameObject txt_errorLog;

    public class User
    {
        public string email = "default";
        public string password = "default";
        public string nickname = "default";
        public string score = "0";

        public void setUser(string email, string password, string nickName)
        {
            this.email = email;
            this.password = password;
            this.nickname = nickName;
        }
    }

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

    public void writeUser(string uid, string emailText, string passwordText, string nicknameText)
    {
        try
        {
            User user = new User();
            user.setUser(emailText, passwordText, nicknameText);
            string json = JsonUtility.ToJson(user);
            databaseReference.Child("user").Child(uid).SetRawJsonValueAsync(json);
            databaseReference.Child("user_email").Child(uid).SetValueAsync(emailText);
            databaseReference.Child("user_nickname").Child(uid).SetValueAsync(nicknameText);

            popTrue();
            txt_errorLog.GetComponent<TMP_Text>().text = "SUCCESS : \r\n 계정 생성 완료! \r\n 로그인하십시오.";
        }
        catch(Exception e)
        {
            popTrue();
            txt_errorLog.GetComponent<TMP_Text>().text = "ERROR CODE : \r\n 계정 생성 실패! \r\n 개발자에게 문의하세요.";
            return;
        }
    }

    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void Start()
    {
        popFalse();
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

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

    public void btnEmailCheck()
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
                        txt_emailCheckLog.GetComponent<TMP_Text>().text = "사용 가능한 이메일";
                        emailData_return = true;
                    }
                    else
                    {
                        img_emailCheck.GetComponent<Image>().sprite = img_changeImage;
                        txt_emailCheckLog.GetComponent<TMP_Text>().text = "사용 불가능한 이메일";
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
                            emailDataCheck = false;
                            break;
                        }
                        emailDataCheck = true;
                    }
                }
            });
        });
    }

    public void btnPwCheck()
    {
        pwCheck(lbl_password.text.Trim(), lbl_passwordCheck.text.Trim());
    }

    public bool pwCheck(string password, string pwCheck)
    {
        if (password.Equals(pwCheck) && password != "" && pwCheck != "")
        {
            img_pwCheck.GetComponent<Image>().sprite = img_orignImage;
            txt_pwCheckLog.GetComponent<TMP_Text>().text = "비밀번호 일치";
            return true;
        }
        else
        {
            img_pwCheck.GetComponent<Image>().sprite = img_changeImage;
            txt_pwCheckLog.GetComponent<TMP_Text>().text = "비밀번호 불일치";
            return false;
        }
    }

    public void btnNicknameCheck()
    {
        Task.Run(() => nicknameCheck(lbl_nickname.text.Trim()));
    }
    
    async void nicknameCheck(string nickname)
    {
        await Task.Run(() => nicknameData(nickname));

        await Task.Delay(500);

        await Task.Run(() =>
        {
            Thread thread = new Thread(() =>
            {
                m_queueAction.Enqueue(() =>
                {
                    if (nicknameDataCheck && nickname != null && nickname != "")
                    {

                        img_nicknameCheck.GetComponent<Image>().sprite = img_orignImage;
                        txt_nicknameCheckLog.GetComponent<TMP_Text>().text = "사용 가능한 닉네임";
                        nicknameData_return = true;
                    }
                    else
                    {
                        img_nicknameCheck.GetComponent<Image>().sprite = img_changeImage;
                        txt_nicknameCheckLog.GetComponent<TMP_Text>().text = "사용 불가능한 닉네임";
                        nicknameData_return = false;
                    }
                });
            });
            thread.Start();
        });
    }

    async void nicknameData(string nickname)
    {
        await Task.Run(() =>
        {
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            databaseReference.Child("user_nickname").GetValueAsync().ContinueWith(task =>
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
                        if (nickname.Equals(item.Value))
                        {
                            nicknameDataCheck = false;
                            break;
                        }
                        nicknameDataCheck = true;
                    }
                }
            });
        });
    }

    public void btnCreate()
    {
        string emailText = lbl_email.text.Trim();
        string passwordText = lbl_password.text.Trim();
        string pwCheckText = lbl_passwordCheck.text.Trim();
        string nicknameText = lbl_nickname.text.Trim();

        emailCheck(emailText);
        bool pwCheckBool = pwCheck(passwordText, pwCheckText);
        nicknameCheck(nicknameText);

        if(emailData_return && pwCheckBool && nicknameData_return)
        {
            auth.CreateUserWithEmailAndPasswordAsync(emailText, passwordText).ContinueWith(task =>
            {
                {
                    if (task.IsFaulted)
                    {
                        popTrue();
                        txt_errorLog.GetComponent<TMP_Text>().text = "ERROR CODE : \r\n 계정 생성 실패! \r\n 인터넷 연결을 확인하세요";
                        return;
                    }
                    FirebaseUser newUser = task.Result;
                    writeUser(newUser.UserId, emailText, passwordText, nicknameText);
                }
            });
        }
    }

    public void btnExit()
    {
        SceneManager.LoadScene("2_titleScene");
    }
}
