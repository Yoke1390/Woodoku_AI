using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField]
    private Image background;

    [SerializeField]
    private Image filledOverlay;

    [SerializeField]
    private Image borderRight;

    [SerializeField]
    private Image borderTop;

    public void Show()
    {
        filledOverlay.enabled = true;
    }

    public void Hide()
    {
        filledOverlay.enabled = false;
    }

    public void SetBackgroundColor(Color backgroundColor)
    {
        background.color = backgroundColor;
    }

    public void InitializeBorder(Color defaultBorderColor)
    {
        borderRight.color = defaultBorderColor;
        borderTop.color = defaultBorderColor;
    }

    public void HighlightRightBorder(Color highlightBorderColor)
    {
        borderRight.color = highlightBorderColor;
        borderRight.transform.SetAsLastSibling();
    }

    public void HighlightTopBorder(Color highlightBorderColor)
    {
        borderTop.color = highlightBorderColor;
        borderTop.transform.SetAsLastSibling();
    }

    public void HideRightBorder()
    {
        borderRight.enabled = false;
    }

    public void HideTopBorder()
    {
        borderTop.enabled = false;
    }
}
