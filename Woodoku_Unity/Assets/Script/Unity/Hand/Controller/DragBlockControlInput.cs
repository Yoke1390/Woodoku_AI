using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Unity.Hand.Controller
{
    [RequireComponent(typeof(BlockManipulator))]
    public class DragBlockControlInput : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private BlockManipulator _blockManipulator;
        private CanvasGroup _canvasGroup;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _blockManipulator = GetComponent<BlockManipulator>();
        }

        public void OnBeginDrag(PointerEventData _)
        {
            _canvasGroup.blocksRaycasts = false;
            _blockManipulator.BeginMove();
        }

        public void OnDrag(PointerEventData _)
        {
            // IDragHandler を実装しないと drag 系イベントが発火しない
        }

        public void OnEndDrag(PointerEventData _)
        {
            _canvasGroup.blocksRaycasts = true;
            _blockManipulator.EndMove();
        }
    }
}