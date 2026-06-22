using UnityEngine;
using System.Collections.Generic;
using KidsItemsSort;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
public class LevelGeneratorEditor : EditorWindow
{
    int startLevel = 1;
    int totalLevels = 50;
    int itemTypes = 20;

    SaveLevel saveLevel;

    [MenuItem("Tools/Auto Level Generator")]
    public static void ShowWindow()
    {
        //GetWindow<AutoLevelGenerator>("Auto Level Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Match-3 Auto Generator", EditorStyles.boldLabel);

        saveLevel = (SaveLevel)EditorGUILayout.ObjectField("SaveLevel Ref", saveLevel, typeof(SaveLevel), true);

        startLevel = EditorGUILayout.IntField("Start Level", startLevel);
        totalLevels = EditorGUILayout.IntField("Total Levels", totalLevels);
        itemTypes = EditorGUILayout.IntField("Item Variety", itemTypes);

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Levels"))
        {
            GenerateLevels();
        }
    }

    void GenerateLevels()
    {
        if (saveLevel == null)
        {
            Debug.LogError("Assign SaveLevel script!");
            return;
        }

        for (int level = startLevel; level < startLevel + totalLevels; level++)
        {
            Debug.Log("Generating Level: " + level);

            ClearScene();

            GenerateLevelLayout(level);

            saveLevel.LevelNumber = level;
            //saveLevel.OnSaveLevel();
        }

        Debug.Log("All Levels Generated!");
    }

    void ClearScene()
    {
        var blocks = GameManager.instance.blocks;

        foreach (var b in blocks)
        {
            if (b != null)
                DestroyImmediate(b.gameObject);
        }

        blocks.Clear();
    }

    void GenerateLevelLayout(int level)
    {
        int blockCount = Mathf.Clamp(3 + level / 10, 3, 6);
        int tilesPerBlock = 3;

        int totalTiles = blockCount * tilesPerBlock;

        // Ensure divisible by 3
        totalTiles = (totalTiles / 3) * 3;

        List<string> items = GenerateMatch3Items(totalTiles, level);

        int itemIndex = 0;

        Vector2[] positions = new Vector2[]
        {
            new Vector2(0, 0.8f),
            new Vector2(-1.37f, -0.29f),
            new Vector2(1.37f, -0.29f),
            new Vector2(0, -1.2f),
            new Vector2(-1.5f, -1.8f),
            new Vector2(1.5f, -1.8f)
        };

        for (int i = 0; i < blockCount; i++)
        {
            GameObject blockGO = (GameObject)PrefabUtility.InstantiatePrefab(saveLevel.blockPrefab, saveLevel.blockParent);
            Block block = blockGO.GetComponent<Block>();

            blockGO.transform.localPosition = positions[i];
            //block.rackPosition = RackPosition.Front;
            block.SetSortingOrder(70 + i);

            GameManager.instance.blocks.Add(block);

            // Create TileSet
            GameObject tileSetGO = (GameObject)PrefabUtility.InstantiatePrefab(saveLevel.TileSetPrefab, blockGO.transform);
            TileSet tileSet = tileSetGO.GetComponent<TileSet>();

            tileSet.isFront = true;
            tileSet.positionIndex = 0;
            tileSet.transform.localPosition = new Vector2(0, -0.05f);

            block.frontTiles = tileSet;

            // Fill tiles
            foreach (Tile tile in tileSet.tiles)
            {
                if (itemIndex >= items.Count) break;

                string id = items[itemIndex++];

                foreach (Item prefab in saveLevel.items)
                {
                    if (prefab.id == id)
                    {
                        Item newItem = (Item)PrefabUtility.InstantiatePrefab(prefab, tile.transform);
                        newItem.id = id;
                        newItem.parentTile = tile;

                        tile.item = newItem;
                        tile.isFilled = true;

                        break;
                    }
                }
            }
        }
    }

    List<string> GenerateMatch3Items(int totalTiles, int seed)
    {
        Random.InitState(seed);

        List<string> items = new List<string>();

        int sets = totalTiles / 3;

        for (int i = 0; i < sets; i++)
        {
            string id = Random.Range(1, itemTypes).ToString();

            items.Add(id);
            items.Add(id);
            items.Add(id);
        }

        // Shuffle
        for (int i = 0; i < items.Count; i++)
        {
            int rand = Random.Range(i, items.Count);
            string temp = items[i];
            items[i] = items[rand];
            items[rand] = temp;
        }

        return items;
    }
}