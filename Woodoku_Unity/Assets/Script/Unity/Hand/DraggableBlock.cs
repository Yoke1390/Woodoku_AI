using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(HandBlock))]
[RequireComponent(typeof(CanvasGroup))]
public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private HandBlock _handBlock;
    private CanvasGroup _canvasGroup;

    private RectTransform _parentRectTransform;
    private Vector2 _initialLocalPosition;

    private DropHandler _onDropRequested;
    public event Action BlockPlaced;

    public void SetDropHandler(DropHandler handler)
    {
        _onDropRequested = handler;
    }

    private void Start()
    {
        _handBlock = GetComponent<HandBlock>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _parentRectTransform = transform.parent.GetComponent<RectTransform>();
        _initialLocalPosition = transform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _handBlock.SetScale(1f);
        _canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentRectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPointerPosition
            )
        )
        {
            transform.localPosition = localPointerPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool success = _onDropRequested?.Invoke(eventData, _handBlock.BlockShape) ?? false;
        if (success)
        {
            BlockPlaced?.Invoke();
            Destroy(gameObject);
        }
        else
        {
            ResetBlock();
        }
    }

    private void ResetBlock()
    {
        _canvasGroup.blocksRaycasts = true;
        transform.localPosition = _initialLocalPosition;
        _handBlock.ResetScale();
    }
}

public delegate bool DropHandler(PointerEventData eventData, BlockShape blockShape);
