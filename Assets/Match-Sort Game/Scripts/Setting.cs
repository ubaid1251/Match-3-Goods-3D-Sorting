using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Setting : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] musicBtnSprite;
    public GameObject[] soundBtnSprite,Vibrate;
    public static Setting ins;
    public RectTransform panel;
    CanvasGroup cg;
    private void Awake()
    {
        ins = this;
        cg = panel.GetComponent<CanvasGroup>();
    }
    void Start()
    { 
        SoundManager.instance.CheckOnStart();
        if (PlayerPrefs.GetInt("IsStart") == 0)
        {
            PlayerPrefs.SetInt("Vibrate", 1);
            PlayerPrefs.SetInt("IsStart", 1);
        //    print("in");

        }
        if (PlayerPrefs.GetInt("Vibrate") == 1)
        {
       //     print("in2");
            Vibrate[0].SetActive(true);
            Vibrate[1].SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Vibrate") == 0)
        {
        //    print("in3");
            Vibrate[0].SetActive(false);
            Vibrate[1].SetActive(true);
        }
    }
    public void playSound()
    {
        SoundManager.instance.PlayEffect_Instance(4);
    }
    public void ForSound()
    {
        // SoundManager.instance.PlayEffect_Instance(4);
        SoundManager.instance.SoundSettingBtn_OnClick();
    }
    public void ForMusic()
    {
        // SoundManager.instance.PlayEffect_Instance(4);
        SoundManager.instance.MusicSettingBtn_OnClick();
    }
    public void VibrateOf()
    {
        SoundManager.instance.PlayEffect_Instance(4);
        PlayerPrefs.SetInt("Vibrate", 0);
    }
    public void VibrateOn()
    {
        SoundManager.instance.PlayEffect_Instance(4);
        PlayerPrefs.SetInt("Vibrate", 1);
    }
    public void PP()
    {
        SoundManager.instance.PlayEffect_Instance(4);   
        Application.OpenURL("https://sites.google.com/view/amasconsultant-privacy-policy/home");

    }
    public void Term()
    {
        SoundManager.instance.PlayEffect_Instance(4);
    ///    Application.OpenURL("https://muhammadubaidprivacy.blogspot.com/2025/06/terms-of-use.html");
     ////   Application.OpenURL("https://muhammadubaidprivacy.blogspot.com/2025/06/privacy-policy.html");
    }
    public void ShowPanel()
    {
        SoundManager.instance.PlayEffect_Instance(11);
        SoundManager.instance.PlayEffect_Instance(9);
        panel.transform.parent.gameObject.SetActive(true);
        panel.DOScale(1, .25f);
        cg.DOFade(1, .25f);
    }
    public void Cross()
    {
        SoundManager.instance.PlayEffect_Instance(9);

        panel.DOScale(.5f, .25f);
        cg.DOFade(0, .25f).OnComplete(() =>
        {
            //isUiActive = false;
            panel.transform.parent.gameObject.SetActive(false);
        });
    }
    void Update()
    {
        
    }
}
