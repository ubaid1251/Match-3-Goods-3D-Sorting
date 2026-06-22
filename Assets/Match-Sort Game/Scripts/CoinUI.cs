using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class CoinUI : MonoBehaviour
{
    [Header("Coin UI")]
    public TMP_Text coinText;
    public Transform coinTargetPos;

    [Header("Coin VFX")]
    public GameObject[] coinVfxs;
    public GameObject[] NewPos;
    int totalCoins;
    public static CoinUI instance;
    public bool Is_Buy;
    public static int CoinNum;
    private void Start()
    {
        CoinNum=0;
        Is_Buy=false;
        totalCoins = PlayerPrefs.GetInt("Coins", 0);
        UpdateCoinText();
    }
    private void Awake()
    {
        instance = this;
    }
    public void AddCoin(int amount)
    {
        totalCoins += amount;

        PlayerPrefs.SetInt("Coins", totalCoins);

        UpdateCoinText();
    }

    void UpdateCoinText()
    {
        coinText.text = totalCoins.ToString();
    }

    public void AddCoinWithEffect(int amount, Action onComplete = null)
    {
        if (coinTargetPos == null)
        {
            AddCoin(amount);
            onComplete?.Invoke();
            return;
        }

        int completed = 0;

        for (int i = 0; i < coinVfxs.Length; i++)
        {
            GameObject coin = coinVfxs[i];

            if (coin == null)
                continue;

            coin.SetActive(true);
            if (Is_Buy)
            {
                coin.transform.localPosition = NewPos[CoinNum].transform.localPosition;
            }
            else
            {
              coin.transform.localPosition = new Vector3(-164f, -171f,0);
            }
            float delay = i * 0.1f;

            Vector3 startPos = coin.transform.position;

            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-0.3f, 0.3f),
                UnityEngine.Random.Range(0.2f, 0.5f),
                0
            );

            Vector3 midPoint =
                (startPos + coinTargetPos.position) / 2f +
                randomOffset;

            Vector3[] path =
            {
            startPos,
            midPoint,
            coinTargetPos.position
        };

            Sequence seq = DOTween.Sequence();

            seq.AppendInterval(delay);

            seq.Append(
                coin.transform.DOPath(path, 0.6f, PathType.CatmullRom)
                .SetEase(Ease.OutQuad)
            );

            seq.OnComplete(() =>
            {
                coin.SetActive(false);

                completed++;

                if (completed >= coinVfxs.Length)
                {
                    AddCoin(amount);

                    onComplete?.Invoke();
                }
            });
        }
    }
    //public bool SpendCoins(int amount)
    //{
    //    if (totalCoins < amount)
    //        return false;

    //    totalCoins -= amount;

    //    PlayerPrefs.SetInt("Coins", totalCoins);

    //    UpdateCoinText();

    //    return true;
    //}
    Coroutine coinAnimCoroutine;
    public bool SpendCoins(int amount)
    {
        if (totalCoins < amount)
            return false;

        int oldCoins = totalCoins;

        totalCoins -= amount;

        PlayerPrefs.SetInt("Coins", totalCoins);

        // Stop previous animation if running
        if (coinAnimCoroutine != null)
            StopCoroutine(coinAnimCoroutine);

        coinAnimCoroutine = StartCoroutine(AnimateCoins(oldCoins, totalCoins));

        return true;
    }
    IEnumerator AnimateCoins(int from, int to)
    {
        int current = from;

        while (current > to)
        {
            current--;

            coinText.text = current.ToString();

            yield return new WaitForSeconds(0.0001f); // speed
        }

        coinText.text = to.ToString();
    }
    public void AddCoinButton()
    {
        AddCoinWithEffect(50);
    }
    public void SetPosCoin(int num)
    {
        CoinNum=num;
    }
    public void RewardAd_GetCoin()
    {
        IntitializeAdmob.Instance.ShowRewarded(() =>
        {
            AddCoinWithEffect(20);
            Debug.Log("Reward 20 Coin Given!");
        });
    }
    public void SetBuyFalse()
    {
        Is_Buy=false;
    }
    public void SetBuytrue()
    {
        Is_Buy=true;
    }
}
