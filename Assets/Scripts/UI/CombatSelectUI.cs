using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CombatSelectUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image characterSilhouette;
    [SerializeField] private Button meleeButton;
    [SerializeField] private Button magicButton;
    [SerializeField] private Button goButton;

    [Header("Male Sprites")]
    [SerializeField] private Sprite maleNeutralSprite;
    [SerializeField] private Sprite maleMeleeSprite;
    [SerializeField] private Sprite maleMagicSprite;

    [Header("Female Sprites")]
    [SerializeField] private Sprite femaleNeutralSprite;
    [SerializeField] private Sprite femaleMeleeSprite;
    [SerializeField] private Sprite femaleMagicSprite;

    private CombatStyle currentStyle = CombatStyle.None;

    private void Start()
    {
        // Make sure cursor is visible in this menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        meleeButton.onClick.AddListener(OnMeleeClicked);
        magicButton.onClick.AddListener(OnMagicClicked);
        goButton.onClick.AddListener(OnGoClicked);

        goButton.interactable = false;

        // Set initial silhouette based on chosen gender
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("[CombatSelectUI] GameManager.Instance is NULL.");
            return;
        }

        SetNeutralSilhouette(gm.SelectedGender);
    }

    private void SetNeutralSilhouette(CharacterGender gender)
    {
        switch (gender)
        {
            case CharacterGender.Male:
                if (maleNeutralSprite != null)
                    characterSilhouette.sprite = maleNeutralSprite;
                break;
            case CharacterGender.Female:
                if (femaleNeutralSprite != null)
                    characterSilhouette.sprite = femaleNeutralSprite;
                break;
            default:
                Debug.LogWarning("[CombatSelectUI] No gender selected; using default silhouette.");
                break;
        }
    }

    private void OnMeleeClicked()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        currentStyle = CombatStyle.Melee;
        gm.SetCombatStyle(CombatStyle.Melee);
        goButton.interactable = true;

        // Update silhouette pose
        if (gm.SelectedGender == CharacterGender.Male && maleMeleeSprite != null)
            characterSilhouette.sprite = maleMeleeSprite;
        else if (gm.SelectedGender == CharacterGender.Female && femaleMeleeSprite != null)
            characterSilhouette.sprite = femaleMeleeSprite;
    }

    private void OnMagicClicked()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        currentStyle = CombatStyle.Magic;
        gm.SetCombatStyle(CombatStyle.Magic);
        goButton.interactable = true;

        // Update silhouette pose
        if (gm.SelectedGender == CharacterGender.Male && maleMagicSprite != null)
            characterSilhouette.sprite = maleMagicSprite;
        else if (gm.SelectedGender == CharacterGender.Female && femaleMagicSprite != null)
            characterSilhouette.sprite = femaleMagicSprite;
    }

    private void OnGoClicked()
    {
        if (currentStyle == CombatStyle.None) return;

        // Go to the hub (your hub scene name)
        SceneManager.LoadScene("Hub");
    }
}
