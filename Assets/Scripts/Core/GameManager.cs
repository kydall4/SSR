using UnityEngine;
using UnityEngine.SceneManagement;

public enum WorldType { None, RomanceCombat, Mystery, Combat, UltraRareHybrid }
public enum WorldRarity { Common, Uncommon, Rare, SR, SSR }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public CharacterGender SelectedGender { get; private set; } = CharacterGender.None;
    public CombatStyle SelectedCombatStyle { get; private set; } = CombatStyle.None;

    [Header("Tickets")]
    public int regularTickets = 1;
    public int specialFragments = 0;
    public int specialTickets = 0;

    [Header("Current Pull")]
    public WorldType currentWorld = WorldType.None;
    public WorldRarity currentWorldRarity = WorldRarity.Common;
    public string currentWorldSceneName = "";

    // 🔹 DEMO FLAG: ensure the first normal pull is always Mystery
    // This is private and only lives for this play session.
    private bool hasForcedMysteryFirstPull = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ---- Pull Logic ----
    public void PullNormalWorld()
    {
        regularTickets--;

        // 🔹 DEMO OVERRIDE:
        // The first time we pull a normal world, always choose Mystery.
        if (!hasForcedMysteryFirstPull)
        {
            hasForcedMysteryFirstPull = true;

            currentWorld = WorldType.Mystery;
            currentWorldSceneName = "MysteryWorld";
            currentWorldRarity = WorldRarity.Rare; // keep your existing rarity behavior

            return; // skip the random roll below for this first pull
        }

        // 🔹 Normal behavior for all subsequent pulls:
        int roll = Random.Range(0, 3);
        switch (roll)
        {
            case 0:
                currentWorld = WorldType.RomanceCombat;
                currentWorldSceneName = "RomanceWorld";
                break;
            case 1:
                currentWorld = WorldType.Mystery;
                currentWorldSceneName = "MysteryWorld";
                break;
            case 2:
                currentWorld = WorldType.Combat;
                currentWorldSceneName = "CombatWorld";
                break;
        }

        currentWorldRarity = WorldRarity.Rare; // 50% fragment chance
    }

    public void PullSpecialWorld()
    {
        specialTickets--;
        currentWorld = WorldType.UltraRareHybrid;
        currentWorldSceneName = "UltraRareWorld";
        currentWorldRarity = WorldRarity.SSR;
    }

    public void GoToHub()
    {
        SceneManager.LoadScene("Hub");
    }

    public void GoToCurrentWorld()
    {
        if (!string.IsNullOrEmpty(currentWorldSceneName))
            SceneManager.LoadScene(currentWorldSceneName);
    }

    // ---- World Completion → Rewards ----
    public void OnWorldCompleted()
    {
        AwardRewards(currentWorldRarity);
        SceneManager.LoadScene("Results");
    }

    private void AwardRewards(WorldRarity rarity)
    {
        if (regularTickets == 0)
            regularTickets = 1;

        int fragmentsAwarded = 0;

        switch (rarity)
        {
            case WorldRarity.Common:
            case WorldRarity.Uncommon:
            case WorldRarity.Rare:
                if (Random.value <= 0.5f) fragmentsAwarded = 1;
                break;
            case WorldRarity.SR:
                fragmentsAwarded = 1;
                break;
            case WorldRarity.SSR:
                fragmentsAwarded = 2;
                break;
        }

        specialFragments += fragmentsAwarded;

        while (specialFragments >= 3)
        {
            specialFragments -= 3;
            specialTickets += 1;
        }
    }

    public void ReturnToHub()
    {
        // Unlock and show cursor for the 2D hub
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("Hub");
    }

    public void SetGender(CharacterGender gender)
    {
        SelectedGender = gender;
        Debug.Log("[GameManager] Gender set to: " + gender);
    }

    public void SetCombatStyle(CombatStyle style)
    {
        SelectedCombatStyle = style;
        Debug.Log("[GameManager] Combat style set to: " + style);
    }
}
