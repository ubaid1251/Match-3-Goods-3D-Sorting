using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    void Start()
    {
        //string json = GenerateLevelJSON(2);
        //Debug.Log(json);
    }
    [System.Serializable]
    public class ItemData
    {
        public int tileID;
        public string ItemID;
    }

    [System.Serializable]
    public class TileData
    {
        public int positionIndex;
        public bool isFront;
        public List<ItemData> items = new List<ItemData>();
    }

    [System.Serializable]
    public class BlockData
    {
        public float xAxis;
        public float yAxis;
        public List<TileData> tilesData = new List<TileData>();
        public int blockPosition;
        public int sortingIndex;
    }

    [System.Serializable]
    public class LevelData
    {
        public int LevelNumber;
        public List<BlockData> blocks = new List<BlockData>();
    }

    public enum PatternType
    {
        Horizontal,
        Cross,
        TileShuffle,
        Vertical
    }

    // Positions (same as your JSON)
    Vector2[] blockPositions = new Vector2[]
    {
        new Vector2(0f, 0.814f),
        new Vector2(-1.37f, -0.29f),
        new Vector2(1.37f, -0.29f)
    };

    int[] sortingIndexes = new int[] { 80, 70, 70 };

    List<string> baseItems = new List<string>()
    {
        "19", "19",
        "21", "21", "21"
    };

    // ?? MAIN FUNCTION
    public string GenerateLevelJSON(int levelNumber)
    {
        PatternType pattern = GetPattern(levelNumber);

        List<string> items = Shuffle(baseItems);

        LevelData level = new LevelData();
        level.LevelNumber = levelNumber;

        for (int i = 0; i < 3; i++)
        {
            BlockData block = new BlockData();
            block.xAxis = blockPositions[i].x;
            block.yAxis = blockPositions[i].y;
            block.blockPosition = 0;
            block.sortingIndex = sortingIndexes[i];

            TileData tile = new TileData();
            tile.positionIndex = 0;
            tile.isFront = true;

            tile.items = new List<ItemData>()
            {
                new ItemData(){ tileID = 1, ItemID = "" },
                new ItemData(){ tileID = 2, ItemID = "" },
                new ItemData(){ tileID = 3, ItemID = "" }
            };

            block.tilesData.Add(tile);
            level.blocks.Add(block);
        }

        ApplyPattern(level, items, pattern);

        return JsonUtility.ToJson(level, true);
    }

    // ?? PATTERN SELECTOR
    PatternType GetPattern(int level)
    {
        switch (level)
        {
            case 1: return PatternType.Horizontal;
            case 2: return PatternType.Cross;
            case 3: return PatternType.TileShuffle;
            case 4: return PatternType.Vertical;
            default: return PatternType.Cross;
        }
    }

    // ?? SHUFFLE
    List<string> Shuffle(List<string> list)
    {
        return list.OrderBy(x => Random.value).ToList();
    }

    // ?? APPLY PATTERN
    void ApplyPattern(LevelData level, List<string> items, PatternType pattern)
    {
        switch (pattern)
        {
            case PatternType.Horizontal:
                FillSequential(level, items);
                break;

            case PatternType.Cross:
                ApplyCross(level, items);
                break;

            case PatternType.TileShuffle:
                FillSequential(level, Shuffle(items));
                break;

            case PatternType.Vertical:
                ApplyVertical(level, items);
                break;
        }
    }

    // ?? SIMPLE FILL
    void FillSequential(LevelData level, List<string> items)
    {
        int index = 0;

        foreach (var block in level.blocks)
        {
            foreach (var item in block.tilesData[0].items)
            {
                if (index < items.Count)
                {
                    item.ItemID = items[index];
                    index++;
                }
            }
        }
    }

    // ? CROSS PATTERN
    void ApplyCross(LevelData level, List<string> items)
    {
        var groupA = items.Where(x => x == "19").ToList();
        var groupB = items.Where(x => x == "21").ToList();

        FillBlock(level.blocks[1], groupA); // left
        FillBlock(level.blocks[2], groupB); // right

        var remaining = Shuffle(items);
        FillBlock(level.blocks[0], remaining); // center
    }

    // ?? VERTICAL (front/back idea simplified)
    void ApplyVertical(LevelData level, List<string> items)
    {
        int index = 0;

        foreach (var block in level.blocks)
        {
            TileData backTile = new TileData();
            backTile.positionIndex = 1;
            backTile.isFront = false;

            backTile.items = new List<ItemData>()
            {
                new ItemData(){ tileID = 1, ItemID = "" },
                new ItemData(){ tileID = 2, ItemID = "" },
                new ItemData(){ tileID = 3, ItemID = "" }
            };

            block.tilesData.Add(backTile);

            foreach (var item in block.tilesData[0].items)
            {
                if (index < items.Count)
                {
                    item.ItemID = items[index];
                    index++;
                }
            }
        }
    }

    // ?? FILL BLOCK HELPER
    void FillBlock(BlockData block, List<string> items)
    {
        int index = 0;

        foreach (var item in block.tilesData[0].items)
        {
            if (index < items.Count)
            {
                item.ItemID = items[index];
                index++;
            }
        }
    }
}