using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class AboutPanelHandler : MonoBehaviour
{
    public GameObject[] panels;
    // Start is called before the first frame update
    public GameObject[] selectedIcons;
    public GameObject panel;
    public string PrivacyLink;

    private void OnEnable()
    {
        //IntitializeAdmob.instance.HideBanner();//remove later
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        for (int i = 0; i < selectedIcons.Length; i++)
        {
            selectedIcons[i].SetActive(false);
        }
        panels[0].SetActive(true);
        selectedIcons[0].SetActive(true);
    }
    public void OpenPrivacy()
    {
        SoundManager.instance.PlayEffect_Instance(4);
        Application.OpenURL(PrivacyLink);
    }
    public void ShowPanel(int index)
    {
        SoundManager.instance.PlayEffect_Instance(4);
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        for (int i = 0; i < selectedIcons.Length; i++)
        {
            selectedIcons[i].SetActive(false);
        }
        selectedIcons[index].SetActive(true);
        panels[index].SetActive(true);
    }

    public void CloseAbout()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        for (int i = 0; i < selectedIcons.Length; i++)
        {
            selectedIcons[i].SetActive(false);
        }
        panels[0].SetActive(true);
        selectedIcons[0].SetActive(true);
        gameObject.SetActive(false);
       SoundManager.instance.PlayEffect_Instance(4);
        //if (PlayerPrefs.GetInt("RemoveAds") == 0)//remove later
            //IntitializeAdmob.instance.ShowBanner();//remove later
    }
}
