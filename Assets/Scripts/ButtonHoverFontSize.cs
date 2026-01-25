using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverFontSize : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Text")]
    [SerializeField] private TMP_Text label;
    [SerializeField] private float hoverMultiplier = 1.15f;

    [Header("Hover SFX")]
    [SerializeField] private AudioSource audioSource;   // recomenda-se um AudioSource 2D (UI)
    [SerializeField] private AudioClip hoverClip;
    [Range(0f, 1f)][SerializeField] private float hoverVolume = 0.7f;
    [SerializeField] private float hoverSfxCooldown = 0.05f;

    private float originalSize;
    private float lastHoverSfxTime = -999f;

    private void Awake()
    {
        if (label == null)
            label = GetComponentInChildren<TMP_Text>();

        if (label != null)
            originalSize = label.fontSize;

        // Se não arrastares um AudioSource, tenta encontrar um no próprio objeto
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (label != null)
            label.fontSize = originalSize * hoverMultiplier;

        TryPlayHoverSfx();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (label != null)
            label.fontSize = originalSize;
    }

    private void OnDisable()
    {
        // safety: se o botão for desativado enquanto está em hover
        if (label != null)
            label.fontSize = originalSize;
    }

    private void TryPlayHoverSfx()
    {
        if (audioSource == null || hoverClip == null) return;

        float t = Time.unscaledTime; // UI/pausa não dependem do timeScale
        if (t - lastHoverSfxTime < hoverSfxCooldown) return;

        audioSource.PlayOneShot(hoverClip, hoverVolume);
        lastHoverSfxTime = t;
    }
}

