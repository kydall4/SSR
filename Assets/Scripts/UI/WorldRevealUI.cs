using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class WorldRevealUI : MonoBehaviour
{
    [Header("Capsule Position Settings")]
    [SerializeField] private float closedHalfDistance = 50f;  // distance from center when CLOSED
    [SerializeField] private float openHalfDistance = 500f;   // distance from center when OPEN


    [Header("UI References")]
    [SerializeField] private GameObject revealPanel;
    [SerializeField] private Image capsuleTop;
    [SerializeField] private Image capsuleBottom;
    [SerializeField] private Image orbImage;
    [SerializeField] private Image worldCardImage;
    [SerializeField] private Button continueButton;

    [Header("Orb Sprites")]
    [SerializeField] private Sprite orbRomance;
    [SerializeField] private Sprite orbMystery;
    [SerializeField] private Sprite orbCombat;
    [SerializeField] private Sprite orbUltra;

    [Header("Card Sprites")]
    [SerializeField] private Sprite cardRomance;
    [SerializeField] private Sprite cardMystery;
    [SerializeField] private Sprite cardCombat;
    [SerializeField] private Sprite cardUltra;

    public Action onRevealFinished;

    private bool isRevealing = false;

    private void Start()
    {
        // Ensure panel starts hidden
        if (revealPanel != null)
            revealPanel.SetActive(false);
    }

    public void StartReveal(WorldType world)
    {
        if (isRevealing) return;
        if (revealPanel == null)
        {
            Debug.LogError("[WorldRevealUI] revealPanel is NULL.");
            return;
        }

        isRevealing = true;
        Debug.Log("[WorldRevealUI] Starting reveal for world: " + world);

        revealPanel.SetActive(true);
        orbImage.gameObject.SetActive(false);
        worldCardImage.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);

        // Set orb & card sprites
        switch (world)
        {
            case WorldType.RomanceCombat:
                orbImage.sprite = orbRomance;
                worldCardImage.sprite = cardRomance;
                break;
            case WorldType.Mystery:
                orbImage.sprite = orbMystery;
                worldCardImage.sprite = cardMystery;
                break;
            case WorldType.Combat:
                orbImage.sprite = orbCombat;
                worldCardImage.sprite = cardCombat;
                break;
            case WorldType.UltraRareHybrid:
                orbImage.sprite = orbUltra;
                worldCardImage.sprite = cardUltra;
                break;
            default:
                Debug.LogWarning("[WorldRevealUI] Unknown world type: " + world);
                break;
        }

        // Make sure capsule starts CLOSED
        capsuleTop.rectTransform.anchoredPosition = new Vector2(0f, 100f);
        capsuleBottom.rectTransform.anchoredPosition = new Vector2(0f, -100f);

        StartCoroutine(RevealSequence());
    }

    private IEnumerator RevealSequence()
    {
        // Short pause before opening
        yield return new WaitForSeconds(0.3f);

        Vector2 closedTop = new Vector2(0f, closedHalfDistance);
        Vector2 closedBottom = new Vector2(0f, -closedHalfDistance);
        Vector2 openTop = new Vector2(0f, openHalfDistance);
        Vector2 openBottom = new Vector2(0f, -openHalfDistance);

        // Make sure they START closed
        capsuleTop.rectTransform.anchoredPosition = closedTop;
        capsuleBottom.rectTransform.anchoredPosition = closedBottom;

        // Animate capsule opening
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f; // speed of opening
            capsuleTop.rectTransform.anchoredPosition =
                Vector2.Lerp(closedTop, openTop, t);
            capsuleBottom.rectTransform.anchoredPosition =
                Vector2.Lerp(closedBottom, openBottom, t);
            yield return null;
        }

        capsuleTop.rectTransform.anchoredPosition = openTop;
        capsuleBottom.rectTransform.anchoredPosition = openBottom;

        // Show orb at center and spin
        orbImage.gameObject.SetActive(true);
        orbImage.rectTransform.anchoredPosition = Vector2.zero;
        orbImage.rectTransform.rotation = Quaternion.identity;

        float spinDuration = 3.0f;
        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            orbImage.rectTransform.Rotate(Vector3.forward * -180f * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        // Hide orb, show card in same place
        orbImage.gameObject.SetActive(false);
        worldCardImage.gameObject.SetActive(true);
        worldCardImage.rectTransform.anchoredPosition = Vector2.zero;

        // Tiny delay then show Continue
        yield return new WaitForSeconds(1.0f);

        continueButton.gameObject.SetActive(true);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(HideReveal);
    }

    private void HideReveal()
    {
        Debug.Log("[WorldRevealUI] Reveal finished, closing panel.");
        revealPanel.SetActive(false);
        isRevealing = false;
        onRevealFinished?.Invoke();
    }
}
