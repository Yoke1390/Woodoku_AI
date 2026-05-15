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
        BoardPosition? blockBaseBoardPosition = BoardUI.Instance.GetBlockBaseBoardPosition(
            eventData,
            handBlock.CenterCellOffset
        );

        bool canPlace = WoodokuGameManager.Instance.CanPlaceBlock(
            handBlock.BlockData,
            blockBaseBoardPosition
        );
        if (canPlace)
        {
            Debug.Log("Block Placeable");
        }
        else
        {
            Debug.Log("Block not Placeable");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        BoardPosition? blockBaseBoardPosition = BoardUI.Instance.GetBlockBaseBoardPosition(
            eventData,
            handBlock.CenterCellOffset
        );
        canvasGroup.blocksRaycasts = true;
    }
}
