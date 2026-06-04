using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Coffee.UIExtensions;
namespace KidsItemsSort
{
    public class CompletionPanel : MonoBehaviour
    {
        public GameObject panel;
        public static CompletionPanel instance;
        public Button[] Btns;
        public GameObject[] bjas;
        public event Action OnPressNoThanks;
        public GameObject text,Stars,Coin50Btn,Particle;
        CanvasGroup cg;
        private void Awake()
        {
            cg = panel.GetComponent<CanvasGroup>();
            instance = this;
        }
        public void OnNoThanks()
        {        
            //panel.transform.DOScale(.5f, .25f);
            cg.DOFade(0, .25f).OnComplete(() =>
            {               
                OnPressNoThanks?.Invoke();
                OnResetPanel();
            });           
        }
        public void OnReload()
        {
            //panel.transform.DOScale(.5f, .25f);
            cg.DOFade(0, .25f).OnComplete(() =>
            {
                OnResetPanel();
            });
        }
        public void OnResetPanel()
        {
            SoundManager.instance.StopEffect(26);
            SoundManager.instance.PlayEffect_Instance(7);
            panel.transform.parent.gameObject.SetActive(false);
            bjas[0].gameObject.SetActive(false);
            bjas[1].gameObject.SetActive(false);
            text.transform.DOLocalMoveY(500, 0.01f);
            text.gameObject.SetActive(false);
            Stars.transform.GetChild(0).transform.DOScale(0f, 0.1f);
            Stars.transform.GetChild(1).transform.DOScale(0f, 0.1f);
            Stars.transform.GetChild(2).transform.DOScale(0f, 0.1f);
            Coin50Btn.transform.DOScale(0f, 0.1f);
            Particle.SetActive(false);
            Stars.SetActive(false);
            OnAllBtnsOff();
        }
        public IEnumerator OpenCompletionScreen()
        {
            //yield return new WaitForSeconds(.2f);
            //panel.transform.parent.gameObject.SetActive(true);
            yield return new WaitForSeconds(.2f);
            panel.transform.parent.gameObject.SetActive(true);
            SoundManager.instance.PlayEffect_Instance(1);
            //panel.transform.DOScale(1, .6f);
            cg.DOFade(1, .3f)
            .OnComplete(() =>
             {
                 SoundManager.instance.PlayEffect_Instance(23);
                 GameManager.instance.ClearOldBlocks();
                 SaveLevel.ins.darkImage.gameObject.SetActive(true);
                 bjas[0].gameObject.SetActive(true);
                 bjas[1].gameObject.SetActive(true);
                 text.gameObject.SetActive(true);
                 text.transform.DOLocalMoveY(20, 0.7f).SetDelay(1.6f)
                 .OnComplete(() =>
                 {
                     SoundManager.instance.PlayEffect_Instance(17);

                     Particle.SetActive(true);
                     SoundManager.instance.PlayEffect_Loop(26);

                     OnStars();
                     SoundManager.instance.PlayEffect_Instance(21);
                     Coin50Btn.transform.DOScale(0.78f, 0.7f).SetEase(Ease.OutBack).SetDelay(1f)
                     .OnComplete(() =>
                     {
                         //bjas[0].GetComponent<Animator>().Play("idl");
                         //bjas[1].GetComponent<Animator>().Play("idl");
                         Coin50Btn.GetComponent<UIShiny>().enabled = true;
                     });
                 });
             });
        }
        public void CoinBtn(int num)
        {
            SoundManager.instance.PlayEffect_Instance(3);
            CoinUI.instance.AddCoinWithEffect(num);
            OnAllBtnsOn();
        }
        public void OnAllBtnsOn()
        {
            Btns[0].interactable = false;
            for (int i = 1; i < Btns.Length; i++)
            {
                Btns[i].interactable = true;
            }
        }
        public void OnAllBtnsOff()
        {
            Btns[0].interactable = true;
            for (int i = 1; i < Btns.Length; i++)
            {
                Btns[i].interactable = false;
            }
        }
        public void OnStars()
        {
            Stars.SetActive(true);
            Stars.transform.GetChild(0).transform.DOScale(0.71f, 0.7f).SetEase(Ease.OutBack).SetDelay(0.1f).OnPlay(() =>
            {
                SoundManager.instance.PlayEffect_Instance(21);
            });
            Stars.transform.GetChild(1).transform.DOScale(0.71f, 0.7f).SetEase(Ease.OutBack).SetDelay(0.4f).OnPlay(() =>
            {
                SoundManager.instance.PlayEffect_Instance(21);
            });
            Stars.transform.GetChild(2).transform.DOScale(0.71f, 0.7f).SetEase(Ease.OutBack).SetDelay(0.6f).OnPlay(() =>
            {
                SoundManager.instance.PlayEffect_Instance(21);
            });
        }
    }
}