using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarPanel : MonoBehaviour
{
    public List<GameObject> selectedAvatar,mainAvatar;
    public List<Image> allAvatarBoxes;
    public Sprite selectedbox,notSelectedbox;
    public GameObject bg, avatarPanel, FlagPanel, namePanel;
    private CanvasGroup avatarGroup,nameGroup, FlagGroup;
    int selectedIndex = 0;

    public TMP_InputField nameInput;
    public TMP_Text saveName;
    public TMP_Text Icon_Name;

    public List<GameObject> selectedFlag, mainFlag;
    public List<Image> allFlagBoxes;
    public Sprite selectedboxFlag, notSelectedboxFlag;
    int selectedIndexFlag = 0;
    public Image Bar_Flag;
    private void OnEnable()
    {
       CheckData();
       CheckFlagData();
    }

    void CheckData()
    {
        int selectedCh=PlayerPrefs.GetInt("selectedCh",0);
        print(PlayerPrefs.GetString("name"));
        string savedName=PlayerPrefs.GetString("name");
        if (savedName != string.Empty)
        {
            print(savedName);
            Icon_Name.text = savedName;
        }
        saveName.text = savedName;
        
        foreach (var t in selectedAvatar)
        {
            t.SetActive(false);
        }
        selectedAvatar[selectedCh].SetActive(true);
        foreach (var t in mainAvatar)
        {
            t.SetActive(false);
        }
        mainAvatar[selectedCh].SetActive(true);
        
        foreach (var t in allAvatarBoxes)
        {
            t.sprite=notSelectedbox;
        }
        allAvatarBoxes[selectedCh].sprite = selectedbox;
        
        avatarGroup=avatarPanel.GetComponent<CanvasGroup>();
        nameGroup=namePanel.GetComponent<CanvasGroup>();
    }
    void CheckFlagData()
    {
        int selectedfl = PlayerPrefs.GetInt("selectedflg", 0);
        Bar_Flag.sprite = allFlagBoxes[selectedfl].transform.GetChild(0).GetComponent<Image>().sprite;
        foreach (var t in selectedFlag)
        {
            t.SetActive(false);
        }
        selectedFlag[selectedfl].SetActive(true);
        foreach (var t in mainFlag)
        {
            t.SetActive(false);
        }
        mainFlag[selectedfl].SetActive(true);

        foreach (var t in allFlagBoxes)
        {
            t.sprite = notSelectedboxFlag;
        }
        allFlagBoxes[selectedfl].sprite = selectedboxFlag;

        FlagGroup = FlagPanel.GetComponent<CanvasGroup>();
    }
    public void ShowAvatarPanel()
    {
        CheckData();
        bg.SetActive(true);
        avatarPanel.SetActive(true);
        avatarPanel.GetComponent<RectTransform>().DOScale(1.35f,.35f).SetEase(Ease.OutBack);
        avatarGroup.DOFade(1, .25f).SetEase(Ease.OutBack);
    }
    public void ShowFlagPanel()
    {
        CheckFlagData();
        //bg.SetActive(true);
        FlagPanel.SetActive(true);
        FlagPanel.GetComponent<RectTransform>().DOScale(1.35f, .35f).SetEase(Ease.OutBack);
        FlagGroup.DOFade(1, .25f).SetEase(Ease.OutBack);


        avatarPanel.GetComponent<RectTransform>().DOScale(0.5f, .25f).SetEase(Ease.OutBack);
        avatarGroup.DOFade(0, .25f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            avatarPanel.SetActive(false);
            //namePanel.GetComponent<RectTransform>().DOScale(2.2f, .25f).SetEase(Ease.OutBack);
            //nameGroup.DOFade(1, .25f).SetEase(Ease.OutBack);
        });
    }
    public void HideAvatarPanel()
    {
        selectedIndex = -1;
        avatarPanel.GetComponent<RectTransform>().DOScale(1f,.25f).SetEase(Ease.OutBack);
        avatarGroup.DOFade(0, .25f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            bg.SetActive(false);
            avatarPanel.SetActive(false);
        });
    }
    public void HideFlagPanel()
    {
        selectedIndexFlag = -1;
        FlagPanel.GetComponent<RectTransform>().DOScale(0.5f, .25f).SetEase(Ease.OutBack);
        FlagGroup.DOFade(0, .25f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            ShowAvatarPanel();
            FlagPanel.SetActive(false);
        });
    }
    public void SelectAvatar(int index)
    {
        SoundManager.instance.PlayEffect_Instance(7);
        foreach (var t in allAvatarBoxes)
        {
            t.sprite=notSelectedbox;
        }
        allAvatarBoxes[index].sprite = selectedbox;
        
        foreach (var t in selectedAvatar)
        {
            t.SetActive(false);
        }
        selectedAvatar[index].SetActive(true);
        selectedIndex=index;
    }
    public void SelectFlag(int index)
    {
        SoundManager.instance.PlayEffect_Instance(7);
        foreach (var t in allFlagBoxes)
        {
            t.sprite = notSelectedboxFlag;
        }
        allFlagBoxes[index].sprite = selectedboxFlag;

        foreach (var t in selectedFlag)
        {
            t.SetActive(false);
        }
        selectedFlag[index].SetActive(true);
        selectedIndexFlag = index;
    }
    public void SaveAvatar()
    {
        SoundManager.instance.PlayEffect_Instance(5);
        if (selectedIndex != -1)
        {
            PlayerPrefs.SetInt("selectedCh", selectedIndex);
            //InitializeFirebase_CB.instance.LogFirebaseEvent(selectedIndex + "_Avatar_Selected");
            foreach (var t in mainAvatar)
            {
                t.SetActive(false);
            }

            mainAvatar[selectedIndex].SetActive(true);
        }

        HideAvatarPanel();
    }
    public void SaveFlag()
    {
        SoundManager.instance.PlayEffect_Instance(5);
        if (selectedIndexFlag != -1)
        {
            PlayerPrefs.SetInt("selectedflg", selectedIndexFlag);
            //InitializeFirebase_CB.instance.LogFirebaseEvent(selectedIndexFlag + "_Country_Selected");
            foreach (var t in mainFlag)
            {
                t.SetActive(false);
            }
            Bar_Flag.sprite = allFlagBoxes[selectedIndexFlag].transform.GetChild(0).GetComponent<Image>().sprite;
            mainFlag[selectedIndexFlag].SetActive(true);
        }

        HideFlagPanel();
    }
    public void ShowNamePanel()
    {
        SoundManager.instance.PlayEffect_Instance(7);
        avatarPanel.GetComponent<RectTransform>().DOScale(0.5f, .25f).SetEase(Ease.OutBack);
        avatarGroup.DOFade(0, .25f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            avatarPanel.SetActive(false);
            namePanel.SetActive(true);
            namePanel.GetComponent<RectTransform>().DOScale(2.2f,.25f).SetEase(Ease.OutBack);
            nameGroup.DOFade(1, .25f).SetEase(Ease.OutBack);
        });
    }
    public void HideNamePanel()
    {
        nameInput.text = null;
        nameInput.placeholder.enabled = true;
        namePanel.GetComponent<RectTransform>().DOScale(1f,.25f).SetEase(Ease.OutBack);
        nameGroup.DOFade(0, .25f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            ShowAvatarPanel();
            namePanel.SetActive(false);
        });
    }

    public void SaveName()
    {
        SoundManager.instance.PlayEffect_Instance(5);
        if (nameInput.text.Length > 0)
        {
            PlayerPrefs.SetString("name", nameInput.text);
            string savedName = PlayerPrefs.GetString("name");
            saveName.text = savedName;
            Icon_Name.text = savedName;
        }
        HideNamePanel();
    }
    public void ClickSound()
    {
        SoundManager.instance.PlayEffect_Instance(7);
    }
}
