using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverFontSize : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private float hoverMultiplier = 1.15f;

    private float originalSize;

    private void Awake()
    {
        if (label == null)
            label = GetComponentInChildren<TMP_Text>();

        if (label != null)
            originalSize = label.fontSize;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (label == null) return;
        label.fontSize = originalSize * hoverMultiplier;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (label == null) return;
        label.fontSize = originalSize;
    }

    private void OnDisable()
    {
        // safety: se o botão for desativado enquanto está em hover
        if (label != null)
            label.fontSize = originalSize;
    }
}

