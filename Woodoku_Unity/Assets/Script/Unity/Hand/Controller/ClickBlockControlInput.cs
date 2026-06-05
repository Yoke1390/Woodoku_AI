using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Unity.Hand.Controller
{
    [RequireComponent(typeof(BlockManipulator))]
    public class ClickBlockControlInput : MonoBehaviour, IPointerClickHandler
    {
        private BlockManipulator _blockManipulator;

        private void Start()
        {
            _blockManipulator = GetComponent<BlockManipulator>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_blockManipulator.IsMoving)
                _blockManipulator.EndMove();
            else
                _blockManipulator.BeginMove();
        }
    }
}