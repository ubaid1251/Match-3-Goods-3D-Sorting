using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//using Firebase.Analytics;

public class Star_Controller : MonoBehaviour
{
    public GameObject[] Star;
    private void Start()
    {
        //if (ResCheck.instance.resType == ResType.tab)
        //{
        //    transform.GetChild(0).GetComponent<RectTransform>().DOScale(.25f, 0);
        //    transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPosX(-12f, 0);
        //}
    }
    private void OnEnable()
    {
       
       // if (PlayerPrefs.GetInt("sfx") == 0)
        {
            StartCoroutine(PlayMyAudio());
        
        }
       
    }

    public void RateStar(int selectedStar)
    {
        //SoundManager.instance.PlayEffect_Instance(7);
        StartCoroutine(StarActive(selectedStar));
    }
    public IEnumerator PlayMyAudio()
    {
        AudioSource s = GetComponent<AudioSource>();
        yield return new WaitForSeconds(2);
        //if (PlayerPrefs.GetInt("sfx") == 0)
        {
            s.enabled = true;
            s.Play();
        }
        float l = s.clip.length;
        yield return new WaitForSeconds(l);
        yield return new WaitForSeconds(1);
        StartCoroutine(PlayMyAudio());
    }
    public IEnumerator StarActive(int selectedStars = 1)
    {
        //yield return new WaitForSeconds(0.6f);
        for (int i = 0; i < selectedStars; i++)
        {
            Star[i].SetActive(true);
            Star[i].transform.DOScale(1f, 0.2f);
            yield return new WaitForSeconds(0.02f);
            //if (PlayerPrefs.GetInt("sfx") == 0)
            //    Star[i].GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(0.15f);
        }

        //lock
        //if (selectedStars == 1)
        //    FirebaseAnalytics.LogEvent("Rate_Game", new Parameter("rated", "OneStar"));
        //else if (selectedStars == 2)
        //    FirebaseAnalytics.LogEvent("Rate_Game", new Parameter("rated", "TwoStar"));
        //else if (selectedStars == 3)
        //    FirebaseAnalytics.LogEvent("Rate_Game", new Parameter("rated", "ThreeStar"));
        //else if (selectedStars == 4)
        //    FirebaseAnalytics.LogEvent("Rate_Game", new Parameter("rated", "FourStar"));
        //else if (selectedStars == 5)
        //    FirebaseAnalytics.LogEvent("Rate_Game", new Parameter("rated", "FiveStar"));

        if (selectedStars < 4)
        {
            RateUsHandler.Instance.cross();
        }
        else
        {
            RateUsHandler.Instance.Rate();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < Star.Length; i++)
        {
            Star[i].SetActive(false);
            Star[i].transform.DOScale(2f, 0f);
        }
    }
    public void Deactive()
    {
        gameObject.SetActive(false);
    }
}
