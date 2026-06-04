using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidsItemsSort
{
    public class Block : MonoBehaviour
    {
        public TileSet frontTiles;
        //This is for the Later Update In case we need to add more back layers
        public List<TileSet> backTiles;

        public SpriteRenderer spriteRenderer;
        public Sprite centerSprite, rightSprite, leftSprite, TopCenterSprite, TopRightSprite, TopLeftSprite;
        public RackPosition rackPosition;

        private void Start()
        {
            SetRackSprite();
        }

        public void SetSortingOrder(int val) { 
        
            spriteRenderer.sortingOrder = val;
        
        }

        public void SetRackSprite() {


            if (rackPosition.Equals(RackPosition.Left))
            {
                spriteRenderer.sprite = leftSprite;
            }
            else if (rackPosition.Equals(RackPosition.Right))
            {
                spriteRenderer.sprite = rightSprite;
            }
            else if (rackPosition.Equals(RackPosition.TopLeft))
            {
                spriteRenderer.sprite = TopLeftSprite;
            }
            else if (rackPosition.Equals(RackPosition.TopRight))
            {
                spriteRenderer.sprite = TopRightSprite;
            }
            else if (rackPosition.Equals(RackPosition.TopCenter))
            {
                spriteRenderer.sprite = TopCenterSprite;
            }
            else {
                spriteRenderer.sprite = centerSprite;
            }

        }

        public void BringForwardTileSet() {
            Debug.Log("BringForwardTileSet 1: "+ backTiles.Count+" "+name);
            if (frontTiles.GetItemCount() <=0  && backTiles.Count > 0)
            {
                Debug.Log("BringForwardTileSet 2:");
                frontTiles = backTiles[0];
                backTiles.RemoveAt(0);
                frontTiles.BringForward();


            }
            else {

                Debug.Log("Not here");

            }
        }


    }

    public enum RackPosition { 
        Center,Right,Left, TopCenter, TopRight, TopLeft
    }
}

