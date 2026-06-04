using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidsItemsSort
{
    public class Tile : MonoBehaviour
    {
        [HideInInspector] public Vector2 position;
        public int id = 0;
        public Item item;

        public bool isFilled = false;

        [HideInInspector]
        public ParticleSystem particle;
        [HideInInspector]public TileSet parentTileSet;
        private void Awake()
        {
            position = transform.position;
            //if (particle != null)
            //{
            //    particle = GetComponentInChildren<ParticleSystem>();
            //    //particle.gameObject.SetActive(false);
            //}
        }

        public void RemoveFadeLayer() {

            if(item!=null)
                item.RemoveFadeLayer();

        }
        public void ShowFadeLayer()
        {
            if (item != null)
                item.ShowFadeLayer();
        }

        public TileSet GetParentTileSet() {

            if (parentTileSet == null)
                parentTileSet = transform.parent.GetComponent<TileSet>();

            return parentTileSet;
        }

        public void SetSoringLayerByPositionIndex(int val) {

            if (item != null)
                item.SetSoringLayerByPositionIndex(val);
        }

        public void PlayParticles() {
            if (particle != null)
            {
                particle.gameObject.SetActive(true);
                particle.Play();
            }
            //particle.Play();
        }
    }
}