using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CelebrationEffect : MonoBehaviour
{
   public Image dialogue;
   public Sprite[] allSp;
   public AudioClip[] allClips;
   private void OnEnable()
   {
      int i = Random.Range(0, allSp.Length);
      dialogue.sprite = allSp[i];
      dialogue.SetNativeSize();
      if (GetComponent<AudioSource>().enabled)
         GetComponent<AudioSource>().PlayOneShot(allClips[i]);
        PlayerPrefs.SetString("CameFrom", SceneManager.GetActiveScene().name);
        Invoke(nameof(MoveToSelection),5);
   }

   void MoveToSelection()
   {
      DOTween.KillAll(false);
      //   if (IntitializeAdmob.Instance.IsInterAvailable() || IntitializeAdmob.Instance.IsStaticInterAvailable())
      //   {
      //    gameObject.SetActive(false);
      //    Loading.Instance.handler.GetComponent<LoadingHandler>().moveToScene=PlayerPrefs.GetString("ReloadScene");
      //    Loading.Instance.handler.SetActive(true);
      // }
      // else
      {

            //SceneManager.LoadScene(PlayerPrefs.GetString("ReloadScene"));
            //SceneManager.LoadScene("PurchasePanel_New");
            if (PlayerPrefs.GetInt("Purchased") == 0)
            {
                //PlayerPrefs.SetString("CameFrom", SceneManager.GetActiveScene().name);
                SceneManager.LoadScene("PurchasePanel_New");
            }
            else
            {
                SceneManager.LoadScene(PlayerPrefs.GetString("ReloadScene"));
            }

        }
   }
}
