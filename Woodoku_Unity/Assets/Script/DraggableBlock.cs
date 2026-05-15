using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(HandBlock))]
[RequireComponent(typeof(CanvasGroup))]
public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private HandBlock handBlock;
    private CanvasGroup canvasGroup;

    private RectTransform parentRectTransform;
    private Vector2 initialLocalPosition;

    private void Start()
    {
        handBlock = GetComponent<HandBlock>();
        canvasGroup = GetComponent<CanvasGroup>();

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
        bool isPlaceSuccess = TryPlaceBlock(eventData);
        if (isPlaceSuccess)
        {
            Destroy(gameObject);
        }
        else
        {
            canvasGroup.blocksRaycasts = true;
            transform.localPosition = initialLocalPosition;
            handBlock.ResetScale();
        }
    }

    private bool TryPlaceBlock(PointerEventData eventData)
    {
        BoardPosition blockBaseBoardPosition = BoardUI.Instance.GetBlockBaseBoardPosition(
            eventData,
            handBlock.CenterCellOffset
        );
        bool isPlaceSuccess = WoodokuGameManager.Instance.TryPlaceBlock(
            handBlock.BlockData,
            blockBaseBoardPosition
        );

        return isPlaceSuccess;
    }

    public void Reset()
    {
        canvasGroup.blocksRaycasts = true;
        transform.localPosition = initialLocalPosition;
        handBlock.ResetScale();
    }
}
