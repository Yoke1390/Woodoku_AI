using UnityEngine;
using UnityEngine.EventSystems;

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
