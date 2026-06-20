//using endSort;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
namespace KidsItemsSort
{
    public class GameManager : MonoBehaviour
    {
        public static bool GameTouch = true;
        public static int InHandSortingIndex = 3000,frontSortingIndex = 980;
        Item currentItem;
        public static GameManager instance;
        public List<Block> blocks;
        //public AudioSource dropAudio, tapAudio, matchAudio;
        public LevelGenerator generator;
        public int maxHints = 3;
        private int usedHints = 0;
        public TMP_Text HintNumText;
        //public Slider progressSlider;
        public Image progressFillImage;
        public TMP_Text progressText;
        public static int totalItems;
        int completedItems;
        int coin = 0;
        public GameObject outOfHintPopup, CoinAddPopUp,LoadingPanel;
        public Button CoinPlus;
        public bool IsUi=false, IsHomePress = false;
        private void Awake()
        {
            instance = this;
            IsUi = false;
            IsHomePress = false;

        }
        public List<Item> allItems;
        public void UpdateProgress()
        {
            //completedItems++;


            ////completedItems += 3;

            //float percent = (completedItems / (float)totalItems) * 100f;

            ////progressText.text =completedItems + " / " + totalItems +" ( " + Mathf.RoundToInt(percent) + "%)";
            //progressText.text = completedItems + " / " + totalItems;

            completedItems++;

            float percent = completedItems / (float)totalItems;

            progressText.text = completedItems + " / " + totalItems;

            // Fill amount works from 0 to 1
            progressFillImage.fillAmount = percent;
        }
        private int currentHintIndex = 0;
        public void ShowHint()
        {
            SoundManager.instance.PlayEffect_Instance(7);

            if (usedHints >= maxHints)
            {
                Ui_On();
                outOfHintPopup.GetComponent<ShowAndClosePanel>().ShowPanel();
                Debug.Log("No Hints Left");
                return;
            }

            usedHints++;
            UpdateHint_Text();

            Dictionary<string, List<Item>> groupedItems =
                new Dictionary<string, List<Item>>();

            foreach (Item item in allItems)
            {
                if (item == null)
                    continue;

                if (!groupedItems.ContainsKey(item.id))
                {
                    groupedItems[item.id] = new List<Item>();
                }

                groupedItems[item.id].Add(item);
            }

            List<List<Item>> validGroups = new List<List<Item>>();

            foreach (var pair in groupedItems)
            {
                if (pair.Value.Count >= 3)
                {
                    validGroups.Add(pair.Value);
                }
            }

            if (validGroups.Count == 0)
                return;

            if (currentHintIndex >= validGroups.Count)
            {
                currentHintIndex = 0;
            }

            List<Item> selectedGroup = validGroups[currentHintIndex];

            foreach (Item item in selectedGroup)
            {
                item.ShowHintFade();
            }

            currentHintIndex++;  
        }
        public void UpdateHint_Text()
        {
            HintNumText.text = (maxHints - usedHints).ToString();
        }
        public void BuyExtraHint()
        {
            bool spent = CoinUI.instance.SpendCoins(100);

            if (!spent)
            {
                Debug.Log("Not Enough Coins");
                CoinPlus.interactable = false;
                outOfHintPopup.GetComponent<ShowAndClosePanel>().Cross();
                StartCoroutine(ShowCoinPopupDelay());
                return;
            }

            usedHints--;
            UpdateHint_Text();
            outOfHintPopup.GetComponent<ShowAndClosePanel>().Cross();
        }
        IEnumerator ShowCoinPopupDelay()
        {
            yield return new WaitForSeconds(0.2f); // delay

            CoinAddPopUp.GetComponent<ShowAndClosePanel>().ShowPanel();
        }
        public void AddCoin(int amount)
        {
            coin += amount;
            PlayerPrefs.SetInt("Coin", coin);
            PlayerPrefs.Save();

            // Update CoinUI when coin changes
            //UpdateCoinUI();
        }
        //private void UpdateCoinUI()
        //{
        //    //CoinUI.UpdateCoinValueText();
        //}
        public int GetCoin()
        {
            return coin;
        }
        private void Start()
        {
            IntitializeAdmob.Instance.HideBigBanner();
            Show_Ad();
            Application.targetFrameRate = 120;

            foreach (Transform t in transform)
            {
                Block b = t.GetComponent<Block>();

                if (b != null)
                {
                    blocks.Add(b);
                }
            }
            //progressFillImage.fillAmount = 0;
        }

        public List<Block> GetBlocks() 
        {
            blocks = new List<Block>();
            foreach (Transform t in transform)
            {
                Block b = t.GetComponent<Block>();
                if (b != null)
                {
                    blocks.Add(b);
                }
            }
            return blocks;
        }

        public void IsCompleted()
        {
            foreach (Block b in blocks)
            {
                if (!b.frontTiles.IsEmpty())
                {
                    return;
                }
            }
            StartCoroutine(CompletionPanel.instance.OpenCompletionScreen());
            //CompletionPanel.instance.OpenCompletionScreen();
            Debug.Log("Level Complete");         
        }
        public void Reset_Slider_Hint()
        {
            progressText.text = 0 + " / " + totalItems + "(0%)";
            totalItems = 0;
            completedItems = 0;
            progressFillImage.fillAmount = 0;
            usedHints = 0;
            HintNumText.text = maxHints.ToString();
        }
        private void Update()
        {
            if (IsUi)
               return;
            
            if (Input.GetMouseButtonDown(0))
            {
                DetectObject();
                if (currentItem != null)
                {
                    currentItem.MouseDown();
                }
            }
            if (Input.GetMouseButton(0))
            {
                if (currentItem != null)
                {
                    currentItem.MouseDrag();
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (currentItem != null)
                {
                    currentItem.MouseUp();
                }
                currentItem = null;
            }
        }

        void DetectObject()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D[] colliders = Physics2D.OverlapPointAll(mousePosition);

            if (colliders.Length > 0)
            {
                var rendererObjects = colliders
                    .Select(collider =>
                    {
                        Transform colliderTransform = collider.transform;
                        if (colliderTransform.childCount >= 2)
                        {
                            Transform secondChild = colliderTransform.GetChild(1); // 0 is first, 1 is second
                            SpriteRenderer renderer = secondChild.GetComponent<SpriteRenderer>();
                            return new { Collider = collider, Renderer = renderer };
                        }
                        return null;
                    })
                    .Where(entry => entry != null && entry.Renderer != null)
                    .OrderByDescending(entry => entry.Renderer.sortingOrder);

                var topEntry = rendererObjects.FirstOrDefault();

                if (topEntry != null)
                {
                    Debug.Log($"Clicked on 2nd child: {topEntry.Renderer.gameObject.name}");

                    if (topEntry.Renderer.gameObject.CompareTag("Item"))
                    {
                        currentItem = topEntry.Renderer.transform.parent.GetComponent<Item>();
                    }
                }
            }
        }
        public void Home()
        {
            SoundManager.instance.StopEffect(26);
            SoundManager.instance.PlayEffect_Instance(7);
            IsHomePress = true;
            Show_Ad();
            LoadingPanel.SetActive(true);
            GameManager.instance.Ui_On();
            PlayerPrefs.SetInt("Completed", 1);
            PlayerPrefs.SetInt("RateCounter", PlayerPrefs.GetInt("RateCounter") + 1);
            print("Now rateus = " + PlayerPrefs.GetInt("RateCounter"));
            print("Now Completed = " + PlayerPrefs.GetInt("Completed"));
            Invoke("WaitMain", 2f);
        }
        public void WaitMain()
        {
            SceneManager.LoadScene("MainMenu");
        }
        public void Ui_On()
        {
            IsUi = true;
        }
        public void Ui_Off()
        {
            IsUi = false;
        }
        public void Show_Ad()
        {
            IntitializeAdmob.Instance.ShowInterstitialAd();
        }
        public void RewardAd_Hint()
        {
            IntitializeAdmob.Instance.ShowRewarded(() =>
            {
                usedHints--;
                UpdateHint_Text();
                Debug.Log("Reward Given!");
            });
        }
        //public void GetReward_Ad()
        //{
        //    IntitializeAdmob.Instance.ShowRewarded(); 
           
        //}
        public void ClearOldBlocks()
        {
            foreach (var oldBlock in GameManager.instance.blocks)
            {
                if (oldBlock != null)
                    Destroy(oldBlock.gameObject);
            }
            GameManager.instance.blocks.Clear();
        }
    }
}