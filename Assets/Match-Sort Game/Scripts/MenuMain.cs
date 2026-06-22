using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MenuMain : MonoBehaviour
{
    public GameObject RemoveAds_Obj,All_Ui;
    void Start()
    {
        if (PlayerPrefs.GetInt("RemoveAds")==1)
        {
            RemoveAds_Obj.SetActive(false);
            All_Ui.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        }
    }
    public void PlayBtn()
    {
        SoundManager.instance.PlayEffect_Instance(7);
        IntitializeAdmob.Instance.HideBanner();

        //SceneManager.LoadScene("GoodSort");
    }
    public void PlaySound()
    {
        SoundManager.instance.PlayEffect_Instance(7);
    }

}
