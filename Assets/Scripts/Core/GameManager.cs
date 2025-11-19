using UnityEngine;
using UnityEngine.SceneManagement;

public enum WorldType { None, RomanceCombat, Mystery, Combat, UltraRareHybrid }
public enum WorldRarity { Common, Uncommon, Rare, SR, SSR }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Tickets")]
    public int regularTickets = 1;
    public int specialFragments = 0;
    public int specialTickets = 0;

    [Header("Current Pull")]
    public WorldType currentWorld = WorldType.None;
    public WorldRarity currentWorldRarity = WorldRarity.Common;
    public string currentWorldSceneName = "";

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
}
