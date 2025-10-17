using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MarqueeText : MonoBehaviour
{
    [Tooltip("Speed in UI units per second.")]
    public float speed = 200f;

    RectTransform rt, parentRt;
    Vector2 startPos;
    float resetX;
    bool inited = false;

    void Start()
    {
        // This runs when the object actually becomes active.
        rt = GetComponent<RectTransform>();
        parentRt = rt.parent.GetComponent<RectTransform>();

        // Compute widths in local/UI space
        float textWidth   = rt.rect.width  * Mathf.Abs(rt.localScale.x);
        float parentWidth = parentRt.rect.width * Mathf.Abs(parentRt.localScale.x);
        float pivotX      = rt.pivot.x;  // 0 to 1

        // Start just off the right edge
        startPos = rt.anchoredPosition;
        startPos.x = parentWidth/2 + textWidth * (1 - pivotX);

        // Reset point: when text's right edge fully clears left edge
        resetX = -parentWidth/2 - textWidth * (1 - pivotX);

        // Place it at the computed start
        rt.anchoredPosition = startPos;

        inited = true;
    }

    void Update()
    {
        if (!inited) return;

        Vector2 p = rt.anchoredPosition;
        p.x -= speed * Time.deltaTime;

        // Once off the left, jump back to right
        if (p.x <= resetX)
            p.x = startPos.x;

        rt.anchoredPosition = p;
    }
}