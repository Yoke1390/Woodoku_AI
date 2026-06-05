using UnityEngine;

namespace Script.Unity.Hand.Controller
{
    public class BlockManipulator : MonoBehaviour
    {
        private HandBlock _handBlock;

        private Vector2 _initialLocalPosition;
        private EndBlockMoveHandler _onEndBlockMoveRequested;
        private RectTransform _parentRectTransform;
        private int _slotIndex;
        private Camera _uiCamera;

        public bool IsMoving { get; private set; }

        private void Start()
        {
            IsMoving = false;
            _handBlock = GetComponent<HandBlock>();
            _uiCamera = GetComponentInParent<Canvas>().rootCanvas.worldCamera;

            _parentRectTransform = transform.parent.GetComponent<RectTransform>();
            _initialLocalPosition = transform.localPosition;
        }

        private void Update()
        {
            if (!IsMoving) return;
            FollowToPointer();
        }

        public void Initialize(int slotIndex, EndBlockMoveHandler handler)
        {
            _slotIndex = slotIndex;
            _onEndBlockMoveRequested = handler;
        }

        public void BeginMove()
        {
            _handBlock.SetScale(1f);
            transform.SetAsLastSibling();

            IsMoving = true;
        }

        public void EndMove()
        {
            bool success = _onEndBlockMoveRequested?.Invoke(Input.mousePosition, _slotIndex) ?? false;
            if (!success) ResetBlock();
        }

        private void FollowToPointer()
        {
            transform.localPosition = GetLocalPointerPosition();
        }

        private void ResetBlock()
        {
            transform.localPosition = _initialLocalPosition;
            _handBlock.ResetScale();
            IsMoving = false;
        }

        private Vector2 GetLocalPointerPosition()
        {
            Vector2 mousePosition = Input.mousePosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _parentRectTransform,
                    mousePosition,
                    _uiCamera,
                    out Vector2 localPointerPosition
                ))
                return localPointerPosition;

            return Vector2.zero;
        }
    }

    public delegate bool EndBlockMoveHandler(Vector2 pointerScreenPosition, int slotIndex);
}