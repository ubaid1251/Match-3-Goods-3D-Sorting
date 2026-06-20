using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class AboutPanelHandler : MonoBehaviour
{
    private void OnEnable()
    {
      IntitializeAdmob.Instance.HideBanner();
    }
    public void OpenLink(string link)
    {
        //SoundManager.instance.PlayEffect_Instance(4);
        //Application.OpenURL(link);
    }
    private void OnDisable()
    {
     IntitializeAdmob.Instance.ShowBanner();//remove later
    }

    public void RestorePurchase()
    {
       // unityInAppPurchase_CB.instance.RestorePurchases();
    }
}
