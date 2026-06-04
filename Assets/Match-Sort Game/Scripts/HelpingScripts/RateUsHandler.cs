//using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using UnityEngine.iOS;

public class RateUsHandler : MonoBehaviour
{
    public static RateUsHandler Instance;
    public GameObject rate;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        //DontDestroyOnLoad(this);

        if (CheckRateCondition())
        {
            rate.SetActive(true);
        }
    }

    public void Deactive()
    {
        //if (PlayerPrefs.GetInt("RemoveAds") == 0)
        //{
        //    IntitializeAdmob.instance.ShowBanner();//remove later
        //}
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    public void Rate()
    {
        SoundManager.instance.PlayEffect_Instance(4);
        PlayerPrefs.SetInt("RateDone", 1);
        rate.SetActive(false);
        //Device.RequestStoreReview();
    }

    public void cross()
    {
        SoundManager.instance.PlayEffect_Instance(4);
        rate.GetComponent<Animator>().Play("PanelOut");
        //InitializeFirebase_CB.instance.LogFirebaseEvent("Rate_Game_CrossBtn_Pressed"); //lock
        Invoke(nameof(HidePanel), 0.9f);
    }
    void HidePanel()
    {
        rate.gameObject.SetActive(value: false);
    }


    public bool CheckRateCondition()
    {
        if (PlayerPrefs.GetInt("RateDone") == 0)
        {
            if ((PlayerPrefs.GetInt("RateCounter") % 2 == 0) && PlayerPrefs.GetInt("Completed") == 1 )
            {
                PlayerPrefs.SetInt("RateUsAppear", PlayerPrefs.GetInt("RateUsAppear") + 1);
                Debug.Log("5 Time RateUs " + PlayerPrefs.GetInt("RateUsAppear"));
                PlayerPrefs.SetInt("Completed", 0);
                return true;
            }
        }

        return false;
    }
}
