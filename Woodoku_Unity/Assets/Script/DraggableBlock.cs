using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(HandBlock))]
[RequireComponent(typeof(CanvasGroup))]
public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private HandBlock handBlock;
    private BlockData blockData;
    private CanvasGroup canvasGroup;

    private RectTransform parentRectTransform;
    private Vector2 initialLocalPosition;

    private DropHandler _onDropRequested;

    public void SetDropHandler(DropHandler handler)
    {
        _onDropRequested = handler;
    }

    private void Start()
    {
        handBlock = GetComponent<HandBlock>();
        canvasGroup = GetComponent<CanvasGroup>();

        blockData = handBlock.BlockData;

        parentRectTransform = transform.parent.GetComponent<RectTransform>();
        initialLocalPosition = transform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        handBlock.SetScale(1f);
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRectTransform,
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
        bool success = _onDropRequested?.Invoke(eventData, blockData) ?? false;
        if (success)
        {
            Destroy(gameObject);
        }
        else
        {
            ResetBlock();
        }
    }

    private void ResetBlock()
    {
        canvasGroup.blocksRaycasts = true;
        transform.localPosition = initialLocalPosition;
        handBlock.ResetScale();
    }
}

public delegate bool DropHandler(PointerEventData eventData, BlockData blockData);
