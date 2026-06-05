using UnityEngine;

namespace Script.Unity.Hand
{
    public class BlockPreview : MonoBehaviour
    {
        [SerializeField] private HandBlock handBlockPrefab;

        [SerializeField] private int cellSize = 10;

        [SerializeField] private int slotSize = 300;

        private BlockData[] _blockDatas;

        private void Start()
        {
            InvokeRepeating(nameof(ShowAllBlocks), 0f, 2f);
        }

        private void ShowAllBlocks()
        {
            ClearAllBlocks();
            _blockDatas = Resources.LoadAll<BlockData>("");
            foreach (var blockData in _blockDatas) ShowHandBlock(blockData);
        }

        private void ClearAllBlocks()
        {
            foreach (Transform child in transform) Destroy(child.gameObject);
        }

        private void ShowHandBlock(BlockData blockData)
        {
            GameObject slot = new("Slot", typeof(RectTransform));
            var slotRectTransform = slot.GetComponent<RectTransform>();
            slotRectTransform.SetParent(transform, false);
            slotRectTransform.sizeDelta = new Vector2(slotSize, slotSize);

            var newHandBlock = Instantiate(handBlockPrefab, slotRectTransform);
            newHandBlock.Initialize(blockData.ToShape(), cellSize);
        }
    }
}
