using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KidsItemsSort
{
    public class SaveLevel : MonoBehaviour
    {
        public Transform blockParent;
        public GameObject blockPrefab;
        public GameObject TileSetPrefab;
        public Vector3[] Scale_Parent;
        public Item[] items;

        public string path = "SortRack";
        public int LevelNumber;
        public GameObject saveButton;
        public Image darkImage;
        public TMP_Text levelText;
        public static SaveLevel ins;
        private void Awake()
        {
            ins = this;
        }
        private void Start()
        {

            CompletionPanel.instance.OnPressNoThanks += LoadNextLevel;
            LevelNumber = PlayerPrefs.GetInt("RackSortLevelNumber", 1);
            print(LevelNumber);
            print(blockParent.localScale = Scale_Parent[LevelNumber]);
            blockParent.localScale = Scale_Parent[LevelNumber];
            darkImage.gameObject.SetActive(true);
            SetLevelText();
            LoadLevel(LevelNumber);
        }
        public void OnSaveLevel()
        {
            //LevelData levelData = new LevelData();
            //levelData.LevelNumber = LevelNumber;

            //List<Block> blocks = GameManager.instance.GetBlocks();
            //levelData.blocks = new BlockData[blocks.Count];

            //for (int i = 0; i < levelData.blocks.Length; i++)
            //{
            //    Vector2 pos = blocks[i].transform.localPosition;

            //    levelData.blocks[i] = new BlockData
            //    {
            //        xAxis = pos.x,
            //        yAxis = pos.y,
            //        blockPosition = (int)blocks[i].rackPosition,
            //        sortingIndex = blocks[i].spriteRenderer.sortingOrder
            //    };

            //    Debug.Log("Rack Position: " + levelData.blocks[i].blockPosition + " orig " + blocks[i].rackPosition);

            //    // tilesData = 1 front layer + backTiles layers
            //    levelData.blocks[i].tilesData = new TileData[1 + blocks[i].backTiles.Count];

            //    // --- FRONT TILES ---
            //    levelData.blocks[i].tilesData[0] = new TileData();
            //    levelData.blocks[i].tilesData[0].isFront = blocks[i].frontTiles.isFront;
            //    levelData.blocks[i].tilesData[0].positionIndex = blocks[i].frontTiles.positionIndex;

            //    var frontCount = blocks[i].frontTiles.tiles.Count;
            //    levelData.blocks[i].tilesData[0].items = new ItemData[frontCount];

            //    for (int j = 0; j < frontCount; j++)
            //    {
            //        var tile = blocks[i].frontTiles.tiles[j];
            //        levelData.blocks[i].tilesData[0].items[j] = new ItemData
            //        {
            //            tileID = tile.id,
            //            ItemID = tile.item != null ? tile.item.id : null
            //        };
            //    }

            //    // --- BACK TILES ---
            //    for (int a = 0; a < blocks[i].backTiles.Count; a++)
            //    {
            //        var backTileLayer = blocks[i].backTiles[a];
            //        var backCount = backTileLayer.tiles.Count;

            //        levelData.blocks[i].tilesData[a + 1] = new TileData();
            //        levelData.blocks[i].tilesData[a + 1].isFront = blocks[i].backTiles[a].isFront;
            //        levelData.blocks[i].tilesData[a + 1].positionIndex = blocks[i].backTiles[a].positionIndex;
            //        levelData.blocks[i].tilesData[a + 1].items = new ItemData[backCount];

            //        for (int j = 0; j < backCount; j++)
            //        {
            //            var tile = backTileLayer.tiles[j];
            //            levelData.blocks[i].tilesData[a + 1].items[j] = new ItemData
            //            {
            //                tileID = tile.id,
            //                ItemID = tile.item != null ? tile.item.id : null
            //            };
            //        }
            //    }
            //}

            //string levelJson = JsonUtilityManager.ToJson(levelData);
            //Debug.Log(levelJson);

            //JsonFileWriter.SaveToResources((path + "/" + LevelNumber), levelJson);
        }

        public void LoadLevel(int LevelNumber)
        {
            
            string json = JsonFileWriter.LoadFromResources(path + "/" + LevelNumber);
            blockParent = GameManager.instance.transform;
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("No level data found.");
                this.LevelNumber = 2;
                LoadLevel(2);
                return;
            }

            this.LevelNumber = LevelNumber; ;
            //PlayerPrefs.SetInt("RackSortLevelNumber", 10);
            PlayerPrefs.SetInt("RackSortLevelNumber", this.LevelNumber);

            StartCoroutine(LoadRoutine(json));
        }

        public void ReloadLevel()
        {
            GameManager.instance.ClearOldBlocks();
            LoadLevel(LevelNumber);
        }
        public void OnLoadLevel()
        {
            try
            {
                //LevelNumber = int.Parse(input.text);
            } catch (Exception e) {
                LevelNumber = 1;
            }

            LoadLevel(LevelNumber);
            //string json = JsonFileWriter.LoadFromResources(path + "/" + LevelNumber);
            //blockParent = GameManager.instance.transform;
            //if (string.IsNullOrEmpty(json))
            //{
            //    Debug.LogWarning("No level data found.");
            //    return;
            //}

            //StartCoroutine(LoadRoutine(json));
        }

        IEnumerator LoadRoutine(string json)
        {
            darkImage.gameObject.SetActive(true);
            
            //darkImage.DOFade(0, 0);
            //darkImage.DOFade(1, .5f);
            //yield return new WaitForSeconds(.2f);
            GameManager.instance.Reset_Slider_Hint();
            SetLevelText();
            LevelData levelData = JsonUtilityManager.FromJson<LevelData>(json);

            // Clear old blocks
            //foreach (var oldBlock in GameManager.instance.blocks)
            //{
            //    if (oldBlock != null)
            //        Destroy(oldBlock.gameObject);
            //}
            //GameManager.instance.blocks.Clear();
            //yield return new WaitForSeconds(.1f);
            blockParent.localScale = Scale_Parent[LevelNumber];
            //blockParent.position = Position_Parent[LevelNumber];
            // Load new blocks
            foreach (BlockData blockData in levelData.blocks)
            {
                GameObject blockGO = Instantiate(blockPrefab, blockParent);
                yield return null;

                blockGO.transform.localPosition = new Vector2(blockData.xAxis, blockData.yAxis);

                Block block = blockGO.GetComponent<Block>();
                block.rackPosition = (RackPosition)blockData.blockPosition;
                block.SetSortingOrder(blockData.sortingIndex);
                Debug.Log("Rack Position" + block.rackPosition + " " + blockData.blockPosition);

                GameManager.instance.blocks.Add(block);
                block.SetRackSprite();

                // Loop through all tile sets (front + back)
                for (int a = 0; a < blockData.tilesData.Length; a++)
                {
                    TileData tileSetData = blockData.tilesData[a];

                    // Instantiate TileSet
                    GameObject tileSetGO = Instantiate(TileSetPrefab, blockGO.transform);
                    yield return null;

                    TileSet tileSet = tileSetGO.GetComponent<TileSet>();
                    tileSet.isFront = tileSetData.isFront;
                    tileSet.positionIndex = tileSetData.positionIndex;
                    if (tileSet.isFront)
                    {
                        tileSet.transform.localPosition = new Vector2(0, -0.05f);

                        tileSet.name = "FrontTiles";
                        block.frontTiles = tileSet;
                    }
                    else
                    {
                        tileSet.transform.localPosition = new Vector2(0, 0.005f);

                        tileSet.name = "BackTiles" + tileSet.positionIndex;
                        block.backTiles.Add(tileSet);
                    }

                    // Place items into correct TileSet
                    for (int i = 0; i < tileSetData.items.Length; i++)
                    {
                        ItemData itemData = tileSetData.items[i];

                        if (!string.IsNullOrEmpty(itemData.ItemID))
                        {
                            foreach (Item prefabItem in items)
                            {
                                if (prefabItem.id.Equals(itemData.ItemID))
                                {
                                    // Find the matching tile inside the *current* tileSet
                                    foreach (Tile t in tileSet.tiles)
                                    {
                                        if (t.id.Equals(itemData.tileID))
                                        {
                                            Item newItem = Instantiate(prefabItem.gameObject, t.transform).GetComponent<Item>();
                                            yield return null;

                                            newItem.id = itemData.ItemID;
                                            newItem.parentTile = t;
                                            t.isFilled = true;
                                            t.item = newItem;
                                            break; // Found the tile, stop searching
                                        }
                                    }
                                    break; // Found the prefab, stop searching
                                }
                            }
                        }

                    }
                    tileSet.Start();

                }
            }
            GameManager.totalItems = GameManager.totalItems / 3;

            //GameManager.instance.progressSlider.maxValue = GameManager.totalItems;
            //GameManager.instance.progressText.text = 0 + " / " + GameManager.totalItems + " ( 0%)";
            GameManager.instance.progressText.text = 0 + " / " + GameManager.totalItems;
            darkImage.DOFade(0, .1f).OnComplete(() => 
            {
                darkImage.gameObject.SetActive(false);
                darkImage.DOFade(1, .01f);
            });
            Debug.Log("Loaded level: " + LevelNumber);
        }

        public void SetLevelText() {
            levelText.text = "LEVEL " + LevelNumber;
        }

        public void LoadNextLevel()
        {
            LoadLevel(LevelNumber + 1);
        }

        private void OnDisable()
        {
            CompletionPanel.instance.OnPressNoThanks -= LoadNextLevel;
        }


    }
    [Serializable]
        public class LevelData
        {
            public int LevelNumber;
            public BlockData[] blocks;
        }
        [Serializable]
        public class BlockData
        {
            public float xAxis = 0, yAxis = 0;
            public TileData[] tilesData;
            public int blockPosition, sortingIndex;

        }
        [Serializable]
        public class TileData
        {
            public int positionIndex;
            public bool isFront;
            public ItemData[] items;
        }

        [Serializable]
        public class ItemData
        {

            public int tileID;
            public string ItemID;

        }
    }
