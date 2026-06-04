using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace KidsItemsSort
{
    public class TileSet : MonoBehaviour
    {
        public List<Tile> tiles;
        public bool isFront = false;
        public int positionIndex;
        private Block ParentBlock;

        public void Start()
        {
            tiles = new List<Tile>();
            foreach (Transform t in transform) {

                Tile tile = t.GetComponent<Tile>();
                tiles.Add(tile);
            }

            SetLayerVisual();
        }

        public void SetLayerVisual() {

            if (isFront)
            {

                RemoveFadeLayer();

            }
            else
            {
                ShowFadeLayer();
            }

            SetSoringLayerByPositionIndex(positionIndex);
        }

        public Block GetParentBlock() { 
        
            if(ParentBlock==null)
                ParentBlock = transform.parent.GetComponent<Block>();

            return ParentBlock;
        }

        public bool IsEmpty() {

            foreach (Tile tile in tiles) {

                if (tile.isFilled)
                    return false;
            }
            
            return true;
        }

        void RemoveFadeLayer() {

            foreach (var tile in tiles) {
                tile.RemoveFadeLayer();
            }
        }

        void ShowFadeLayer() {
            foreach (var tile in tiles)
            {
                tile.ShowFadeLayer();
            }

        }

        public void SetSoringLayerByPositionIndex(int val) {
            GetParentBlock();
            foreach (var tile in tiles)
            {
                tile.SetSoringLayerByPositionIndex(val);
            }
        }

        public Tile GetAvaiableTile(Transform t) {
            Tile closestTile = null;
            float minSqrDistance = float.MaxValue;
            Vector2 targetPosition = t.position;
            foreach (Tile tile in tiles)
            {
                if (!tile.isFilled)
                {
                    float sqrDist = (tile.position - targetPosition).sqrMagnitude;
                    if (sqrDist < minSqrDistance)
                    {
                        minSqrDistance = sqrDist;
                        closestTile = tile;
                    }
                }
            }
            return closestTile;
        }

        public void OnItemPlaced()
        {

            if (CheckItemMatch())
            {
                SoundManager.instance.PlayEffect_Instance(25);
                Debug.Log("MATCH...");
                GameManager.instance.UpdateProgress();
                RemoveEmpty();
            }
            
            GameManager.instance.IsCompleted();

        }
        public int GetItemCount()
        {
            int i = 0;

            foreach (var t in tiles) {
                if (t.isFilled) {
                    i++;
                }
            }
            Debug.Log("Get Item: Count "+i+" : "+name+GetParentBlock().name);
            return i;
        }
        public void RemoveEmpty() {

            foreach (Tile tile in tiles)
            {
                if (tile.isFilled)
                {
                    if (tile.item != null)
                    {
                        tile.PlayParticles();
                        Destroy(tile.item.gameObject);
                        tile.item = null;
                        
                    }
                    tile.isFilled = false;
                }
            }
        }

        public void BringForward() {
            Debug.Log("BringForwardTileSet 3");

            foreach (Tile tile in tiles) {

                if (tile.item != null) {
                    Debug.Log("BringForwardTileSet 4");

                    tile.item.visuals.sortingOrder = GameManager.frontSortingIndex;
                    tile.item.RemoveFadeLayer();
                }
            
            }
            isFront = true;
        }

        public bool CheckItemMatch()
        {
            Item itemOne = tiles[0].item;

            if (itemOne != null)
            {
                for (int i = 1; i < tiles.Count; i++)
                {
                    Item itemTwo = tiles[i].item;
                    if (itemTwo == null || itemTwo.id != itemOne.id)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}