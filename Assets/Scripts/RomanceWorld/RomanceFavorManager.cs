using UnityEngine;

public enum RomanceCharacter
{
    Elara,
    Kael,
    Nyx
}

public class RomanceFavorManager : MonoBehaviour
{
    [Header("Initial Favor Values (0-100)")]
    [Range(0, 100)] public int elaraFavor = 50;
    [Range(0, 100)] public int kaelFavor = 50;
    [Range(0, 100)] public int nyxFavor = 50;

    [Header("Thresholds")]
    [Tooltip("0-20: Kill Zone")]
    public int killZoneMax = 20;

    [Tooltip("21-70: Ally Zone")]
    public int allyZoneMax = 70;

    [Tooltip("71-100: Confidant Zone")]
    public int confidantZoneMin = 71;   // ← new field 

    public void AddFavor(RomanceCharacter character, int delta)
    {
        switch (character)
        {
            case RomanceCharacter.Elara:
                elaraFavor = Mathf.Clamp(elaraFavor + delta, 0, 100);
                Debug.Log($"[Favor] Elara favor now {elaraFavor}");
                break;
            case RomanceCharacter.Kael:
                kaelFavor = Mathf.Clamp(kaelFavor + delta, 0, 100);
                Debug.Log($"[Favor] Kael favor now {kaelFavor}");
                break;
            case RomanceCharacter.Nyx:
                nyxFavor = Mathf.Clamp(nyxFavor + delta, 0, 100);
                Debug.Log($"[Favor] Nyx favor now {nyxFavor}");
                break;
        }

        CheckKillZone(character);
    }

    public FavorZone GetZone(RomanceCharacter character)
    {
        int value = GetFavorValue(character);

        if (value <= killZoneMax)
            return FavorZone.Kill;
        if (value <= allyZoneMax)
            return FavorZone.Ally;
        return FavorZone.Confidant;
    }

    public int GetFavorValue(RomanceCharacter character)
    {
        return character switch
        {
            RomanceCharacter.Elara => elaraFavor,
            RomanceCharacter.Kael => kaelFavor,
            RomanceCharacter.Nyx => nyxFavor,
            _ => 0
        };
    }

    private void CheckKillZone(RomanceCharacter character)
    {
        int value = GetFavorValue(character);
        if (value <= 0)
        {
            Debug.LogWarning($"[Favor] {character} has hit 0 favor! World fail condition can trigger here.");
            // Later: trigger a bad end / game over in this world.
        }
    }
}

public enum FavorZone
{
    Kill,       // 0-20
    Ally,       // 21-70
    Confidant   // 71-100
}
