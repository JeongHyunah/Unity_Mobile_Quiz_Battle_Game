using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class singletonWithTimer : MonoBehaviour
{
    public static singletonWithTimer instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("여기?");
        }
        else
        {
            if (instance != null)
            {
                Destroy(this.gameObject);
                Debug.Log("여기??");
            }
        }
    }

    public void Update()
    {
        if (timer_active)
        {
            timer_sec += Time.deltaTime;
            txt_Time = string.Format("{0:D2}:{1:D2}", timer_min, (int)timer_sec);
            if ((int)timer_sec > 59)
            {
                timer_sec = 0;
                timer_min++;
            }
            if (timer_min == 5)
            {
                gameOver();
            }
        }
    }

    public void timerOn()
    {
        timer_active = true;
    }

    public void timerOff()
    {
        timer_active = false;
    }

    public void gameOver()
    {
        timerOff();
        _gameOver = true;
    }

    public void gameInit()
    {
        txt_gameScore = 0;
        txt_Time = null;
        _gameOver = false;
    }

    public int txt_gameScore = 0;
    public bool _gameOver;
    public string txt_Time;
    public bool timer_active;

    public int timer_min = 0;
    public float timer_sec = 0;
}
