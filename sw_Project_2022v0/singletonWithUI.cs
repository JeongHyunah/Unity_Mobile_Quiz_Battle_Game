using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class singletonWithUI : MonoBehaviour
{
    public static singletonWithUI instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != null)
                Destroy(this.gameObject);
        }
    }

    public string txt_nickname = null;
    public string txt_score = null;
    public string uid = null;
}
