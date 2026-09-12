using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoScrollText : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scrollRect;
    public RectTransform content;

    [Header("滚动速度")]
    public float scrollSpeed = 50f;

    private bool isDragging = false;

    private float textHeight;
    private float viewportHeight;

    void Start()
    {
        viewportHeight = scrollRect.viewport.rect.height;

        // Content 中第一份文字的高度
        if (content.childCount > 0)
        {
            RectTransform text = content.GetChild(0) as RectTransform;
            textHeight = text.rect.height;
        }
    }

    void Update()
    {
        if (isDragging)
            return;

        Vector2 pos = content.anchoredPosition;

        // Content 向上移动
        pos.y += scrollSpeed * Time.deltaTime;

        content.anchoredPosition = pos;

        // 第一份文字完全离开顶部
        if (pos.y >= textHeight)
        {
            pos.y -= textHeight;
            content.anchoredPosition = pos;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }
}
