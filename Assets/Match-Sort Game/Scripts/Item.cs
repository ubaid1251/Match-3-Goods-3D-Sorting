using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace KidsItemsSort
{
    public class Item : MonoBehaviour
    {
        string TAG = "Item ";
        public string id = "";
        bool isDragging = false;
        public Vector2 OrignalPosition;
        public SpriteRenderer FadeLayer,visuals;
        public Tile parentTile = null;

        public int OrignalSortingIndex;

        private void Start()
        {
            GameManager.instance.allItems.Add(this);
            GameManager.totalItems++;
            OrignalSortingIndex = visuals.sortingOrder;

            OrignalPosition = transform.localPosition;
            parentTile = transform.parent.GetComponent<Tile>();

            parentTile.item = this;

            Block b = parentTile.GetParentTileSet().GetParentBlock();

            if (!collidersInRange.Contains(b))
                collidersInRange.Add(b);
        }

        Tile previousTile = null;
        public void MouseDown()
        {
            if (!parentTile.parentTileSet.isFront)
                return;
            Debug.Log(parentTile.parentTileSet.GetParentBlock().name);
            previousTile = parentTile;
            isDragging = true;
            Vector2 pos = GetMouseWorldPosition(Camera.main);
            ScaleUp();
            visuals.sortingOrder = GameManager.InHandSortingIndex;
            transform.DOMove(new Vector2(pos.x, pos.y + .2f), .1f);
            SoundManager.instance.PlayEffect_Instance(21);
        }
        public void MouseUp() {
            if (!parentTile.parentTileSet.isFront)
                return;
            SalceDown();

            Block block = GetClosestCollider().GetComponent<Block>();

            Tile tile = block?.frontTiles.GetAvaiableTile(transform);

            if (block != null && tile!=null)
            {

                if (parentTile != null)
                {

                    parentTile.isFilled = false;
                    parentTile.item = null;

                }

                parentTile = tile;
                parentTile.isFilled = true;
                parentTile.item = this;
                transform.SetParent(parentTile.transform);
                transform.position = parentTile.transform.position;
                PlaceAnimation();
                StartCoroutine(PlaceRoutine(previousTile.GetParentTileSet().GetParentBlock()));
            }
            else {

                transform.localPosition = OrignalPosition;
                PlaceAnimation();

            }

        }

        public IEnumerator PlaceRoutine(Block PrevBlock) {
            yield return new WaitForSeconds(.25f);
            //visuals.sortingOrder = OrignalSortingIndex;
            parentTile.GetParentTileSet().OnItemPlaced();
            PrevBlock.BringForwardTileSet();
            parentTile.GetParentTileSet().GetParentBlock().BringForwardTileSet();
        }

        void PlaceAnimation() {

            Vector2 pos = transform.localPosition;

            transform.DOLocalMoveY((pos.y - .5f), .1f).OnComplete(() => {
                SoundManager.instance.PlayEffect_Instance(22);

                transform.DOLocalMoveY((pos.y), .1f);
            });
        }

        public void MouseDrag()
        {
            if (!parentTile.parentTileSet.isFront)
                return;
            Vector2 pos = GetMouseWorldPosition(Camera.main);

            transform.position = new Vector3(pos.x, pos.y + .2f);
        }

        void SalceDown()
        {
            //float randRotate = Random.Range(-7f, 7);
            //transform.DORotate(new Vector3(0, 0, randRotate), .1f);
            visuals.sortingOrder = OrignalSortingIndex;
            transform.DOScale(new Vector2(1f, 1f), .2f);
        }

        void ScaleUp()
        {
            transform.DOScale(new Vector2(1.3f, 1.3f), .1f);
        }

        public static Vector3 GetMouseWorldPosition(Camera camera)
        {
            
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = 0f;

            Vector3 worldPosition = camera.ScreenToWorldPoint(mousePoint);

            Vector3 screenBottomLeft = camera.ScreenToWorldPoint(new Vector3(0, 0, 0));
            Vector3 screenTopRight = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

            worldPosition.x = Mathf.Clamp(worldPosition.x, screenBottomLeft.x, screenTopRight.x);
            worldPosition.y = Mathf.Clamp(worldPosition.y, screenBottomLeft.y, screenTopRight.y);

            return worldPosition;

        }

        public void RemoveFadeLayer() {
            Debug.Log("BringForwardTileSet 5");

            visuals.color = Color.white;
        }
        public void ShowFadeLayer()
        {
            visuals.color = Color.grey;
        }

        public void SetSoringLayerByPositionIndex(int layerIndex)
        {

            int val = layerIndex * 10;
            OrignalSortingIndex = OrignalSortingIndex - val;
            visuals.sortingOrder = OrignalSortingIndex;

        }

        List<Block> collidersInRange = new List<Block>();
        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.tag.Equals("Block"))
                return;

            Block b = other.GetComponent<Block>();
            if (b != null)
            {
                if (!collidersInRange.Contains(b))
                    collidersInRange.Add(b);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (!other.tag.Equals("Block"))
                return;
            Block b = other.GetComponent<Block>();
            if (b != null)
            {
                if (collidersInRange.Contains(b))
                collidersInRange.Remove(b);
            }
        }
        Block GetClosestCollider()
        {
            if (collidersInRange.Count == 0) return null;

            Block closest = null;
            float minDist = float.MaxValue;
            Vector3 myPos = transform.position;

            foreach (var col in collidersInRange)
            {
                float dist = Vector3.Distance(myPos, col.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = col;
                }
            }

            return closest;
        }
        public bool CanMoveToAnyBlock()
        {
            foreach (Block block in collidersInRange)
            {
                if (block == null)
                    continue;

                Tile tile = block.frontTiles.GetAvaiableTile(transform);

                if (tile != null && tile != parentTile)
                {
                    return true;
                }
            }

            return false;
        }

        public void ShowHintFade()
        {
            visuals.DOKill();

            Sequence seq = DOTween.Sequence();

            seq.Append(visuals.DOColor(Color.black, 0.2f));
            seq.Append(visuals.DOColor(Color.white, 0.2f));

            seq.SetLoops(2);
        }
    }
}