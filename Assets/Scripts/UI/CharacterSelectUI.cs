using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button maleSelectButton;
    [SerializeField] private Button femaleSelectButton;
    [SerializeField] private Button nextButton;

    [Header("Highlights (optional)")]
    [SerializeField] private Image maleHighlight;
    [SerializeField] private Image femaleHighlight;

    private CharacterGender currentSelection = CharacterGender.None;

    private void Start()
    {
        Debug.Log("[CharacterSelectUI] Start called.");

        // Make sure cursor is visible in this menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (maleSelectButton == null)
            Debug.LogError("[CharacterSelectUI] maleSelectButton is NULL. Assign it in Inspector.");
        if (femaleSelectButton == null)
            Debug.LogError("[CharacterSelectUI] femaleSelectButton is NULL. Assign it in Inspector.");
        if (nextButton == null)
            Debug.LogError("[CharacterSelectUI] nextButton is NULL. Assign it in Inspector.");

        if (maleSelectButton != null)
            maleSelectButton.onClick.AddListener(OnMaleSelected);
        if (femaleSelectButton != null)
            femaleSelectButton.onClick.AddListener(OnFemaleSelected);
        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextClicked);

        if (nextButton != null)
            nextButton.interactable = false;

        UpdateHighlights();
    }

    private void OnMaleSelected()
    {
        Debug.Log("[CharacterSelectUI] Male selected.");
        if (GameManager.Instance == null)
        {
            Debug.LogError("[CharacterSelectUI] GameManager.Instance is NULL in OnMaleSelected.");
            return;
        }

        currentSelection = CharacterGender.Male;
        GameManager.Instance.SetGender(CharacterGender.Male);

        if (nextButton != null)
            nextButton.interactable = true;

        UpdateHighlights();
    }

    private void OnFemaleSelected()
    {
        Debug.Log("[CharacterSelectUI] Female selected.");
        if (GameManager.Instance == null)
        {
            Debug.LogError("[CharacterSelectUI] GameManager.Instance is NULL in OnFemaleSelected.");
            return;
        }

        currentSelection = CharacterGender.Female;
        GameManager.Instance.SetGender(CharacterGender.Female);

        if (nextButton != null)
            nextButton.interactable = true;

        UpdateHighlights();
    }

    private void OnNextClicked()
    {
        Debug.Log("[CharacterSelectUI] Next clicked. Current selection: " + currentSelection);

        if (currentSelection == CharacterGender.None)
        {
            Debug.LogWarning("[CharacterSelectUI] Next clicked but no gender selected.");
            return;
        }

        SceneManager.LoadScene("CombatSelect");
    }

    private void UpdateHighlights()
    {
        if (maleHighlight != null)
            maleHighlight.enabled = (currentSelection == CharacterGender.Male);

        if (femaleHighlight != null)
            femaleHighlight.enabled = (currentSelection == CharacterGender.Female);
    }
}
