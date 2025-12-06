using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public enum RomanceDay
{
    None = 0,
    Day1_GildedEscape,
    Day2_SealedOrders,
    Day3_BurningGate,
    Completed
}

public enum RomancePhase
{
    None = 0,
    CombatLayer,
    GroupTalkLayer,
    PersonalLayer,
}

public class RomanceWorldController : MonoBehaviour
{
    public GameObject explorationHintPanel;

    [Header("Spawn Points")]
    public Transform spawnDay1;
    public Transform spawnDay2;
    public Transform spawnDay3;

    [Header("Intro UI")]
    public GameObject introPanel;
    public TMP_Text introText;

    [Header("World References")]
    public GameObject playerRoot;
    public GameObject playerBody;
    public GameObject crosshair;

    [Header("References")]
    public RomanceFavorManager favorManager;

    [Header("UI Panels")]
    public GameObject combatChoicePanel;
    public GameObject groupTalkPanel;
    public GameObject personalPanel;

    [Header("Combat UI")]
    public TMP_Text combatTitleText;
    public TMP_Text combatDescriptionText;
    public TMP_Text combatOptionAText;
    public TMP_Text combatOptionBText;

    [Header("Combat Result UI")]
    public GameObject combatResultPanel;
    public TMP_Text combatResultText;

    [Header("Group Talk UI")]
    public TMP_Text groupTitleText;
    public TMP_Text groupDescriptionText;
    public TMP_Text groupOption0Text;
    public TMP_Text groupOption1Text;
    public TMP_Text groupOption2Text;

    [Header("Personal UI")]
    public TMP_Text personalPromptText;

    private RomanceDay currentDay = RomanceDay.None;
    private RomancePhase currentPhase = RomancePhase.None;

    private IEnumerator Start()
    {
        // Start at Day 1 when the scene loads
        yield return null;

        EnterUIMode();
        ShowIntro();
    }

    private void StartDay(RomanceDay day)
    {
        currentDay = day;
        currentPhase = RomancePhase.None;

        Debug.Log($"[RomanceWorld] Starting {currentDay}");

        if (currentDay == RomanceDay.Day1_GildedEscape)
        {
            // Start Day 1 in exploration mode after intro
            EnterExplorationMode();
            if (explorationHintPanel != null)
                explorationHintPanel.SetActive(true);
            return;
        }

        // Other days we can keep UI-first for now
        ShowCombatChoice();
    }



    private void ShowCombatChoice()
    {
        currentPhase = RomancePhase.CombatLayer;

        if (combatChoicePanel != null) combatChoicePanel.SetActive(true);
        if (groupTalkPanel != null) groupTalkPanel.SetActive(false);
        if (personalPanel != null) personalPanel.SetActive(false);

        // Set up text based on current day
        if (currentDay == RomanceDay.Day1_GildedEscape)
        {
            if (combatTitleText != null)
                combatTitleText.text = "Day 1 — The Gilded Escape";

            if (combatDescriptionText != null)
                combatDescriptionText.text =
                    "The Solari capital is in lockdown. Princess Elara needs to escape without turning her own City Watch into corpses.";

            if (combatOptionAText != null)
                combatOptionAText.text =
                    "Rush the main gate and disarm the guards non-lethally.";

            if (combatOptionBText != null)
                combatOptionBText.text =
                    "Slip through a side passage and avoid the main force.";
        }

        Debug.Log($"[RomanceWorld] Combat Layer for {currentDay} shown.");
    }

    public void OnCombatChoiceSelected(int optionIndex)
    {
        Debug.Log($"[RomanceWorld] Combat choice {optionIndex} selected on {currentDay}");

        // Apply favor changes
        ApplyCombatFavorChanges(optionIndex);

        // Show a short “result scene” instead of jumping straight to group talk
        ShowCombatResult(optionIndex);
    }


    private void ShowGroupTalk()
    {
        currentPhase = RomancePhase.GroupTalkLayer;

        if (combatChoicePanel != null) combatChoicePanel.SetActive(false);
        if (groupTalkPanel != null) groupTalkPanel.SetActive(true);
        if (personalPanel != null) personalPanel.SetActive(false);

        if (currentDay == RomanceDay.Day1_GildedEscape)
        {
            if (groupTitleText != null)
                groupTitleText.text = "Campfire — After the Escape";

            if (groupDescriptionText != null)
                groupDescriptionText.text =
                    "Now that you’re safely outside the walls, the group argues about whether leaving the capital like that was the right call.";

            if (groupOption0Text != null)
                groupOption0Text.text = "We had no choice. The King is trapped behind liars.";

            if (groupOption1Text != null)
                groupOption1Text.text = "We should’ve stayed and rooted out the traitors from within.";

            if (groupOption2Text != null)
                groupOption2Text.text = "We were never safe there. The whole system is rigged.";
        }

        Debug.Log($"[RomanceWorld] Group Talk for {currentDay} shown.");
    }

    public void OnGroupTalkChoiceSelected(int optionIndex)
    {
        Debug.Log($"[RomanceWorld] Group talk choice {optionIndex} selected on {currentDay}");

        // Apply medium favor changes
        ApplyGroupTalkFavorChanges(optionIndex);

        // Move to personal (romance) phase
        ShowPersonalPhase();
    }

    private void ShowPersonalPhase()
    {
        currentPhase = RomancePhase.PersonalLayer;

        if (combatChoicePanel != null) combatChoicePanel.SetActive(false);
        if (groupTalkPanel != null) groupTalkPanel.SetActive(false);
        if (personalPanel != null) personalPanel.SetActive(true);

        if (currentDay == RomanceDay.Day1_GildedEscape)
        {
            if (personalPromptText != null)
            {
                personalPromptText.text =
                    "Night falls on the road. Elara stares back toward the city lights, " +
                    "Kael sharpens his sword by the fire, and Nyx double-checks the smuggler routes. Who do you spend time with?";
            }
        }

        Debug.Log($"[RomanceWorld] Personal/Romance phase for {currentDay} shown.");
    }

    public void OnPersonalTargetSelected(int characterIndex)
    {
        if (favorManager == null)
        {
            Debug.LogWarning("[RomanceWorld] No favorManager set on controller.");
            OnPersonalPhaseFinished();
            return;
        }

        RomanceCharacter target = RomanceCharacter.Elara;

        switch (characterIndex)
        {
            case 0:
                target = RomanceCharacter.Elara;
                Debug.Log("[RomanceWorld] You spend the evening with Princess Elara.");
                break;
            case 1:
                target = RomanceCharacter.Kael;
                Debug.Log("[RomanceWorld] You spend the evening with Kael.");
                break;
            case 2:
                target = RomanceCharacter.Nyx;
                Debug.Log("[RomanceWorld] You spend the evening with Nyx.");
                break;
            default:
                Debug.LogWarning("[RomanceWorld] Invalid personal target index, defaulting to Elara.");
                break;
        }

        // High-impact favor change for romance layer.
        favorManager.AddFavor(target, +10);

        // Later we can branch into maintenance vs romance dialogue.
        OnPersonalPhaseFinished();
    }

    // Called after you’ve finished the personal talk for this day
    public void OnPersonalPhaseFinished()
    {
        Debug.Log($"[RomanceWorld] Personal phase finished for {currentDay}");
        AdvanceToNextDay();
    }

    private void AdvanceToNextDay()
    {
        switch (currentDay)
        {
            case RomanceDay.Day1_GildedEscape:
                StartDay(RomanceDay.Day2_SealedOrders);
                break;
            case RomanceDay.Day2_SealedOrders:
                StartDay(RomanceDay.Day3_BurningGate);
                break;
            case RomanceDay.Day3_BurningGate:
                currentDay = RomanceDay.Completed;
                OnRomanceWorldCompleted();
                break;
        }
    }

    private void OnRomanceWorldCompleted()
    {
        Debug.Log("[RomanceWorld] All 5 days completed, going to Results.");

        // IMPORTANT:
        // Call whatever you currently use to go from a world -> Results.
        // If in your Combat world you just do SceneManager.LoadScene("Results"),
        // do the same here. If you call a GameManager method, use that instead.

        // Example (if you use scenes directly):
        SceneManager.LoadScene("Results");
    }

    #region Favor Application (placeholder)

    private void ApplyCombatFavorChanges(int optionIndex)
    {
        if (favorManager == null) return;

        if (currentDay == RomanceDay.Day1_GildedEscape)
        {
            // City gate scenario:
            // A: Heroic non-lethal rush  -> +Elara, -Kael
            // B: Silent side-route       -> +Kael, -Nyx

            switch (optionIndex)
            {
                case 0:
                    favorManager.AddFavor(RomanceCharacter.Elara, +2);
                    favorManager.AddFavor(RomanceCharacter.Kael, -1);
                    break;
                case 1:
                    favorManager.AddFavor(RomanceCharacter.Kael, +2);
                    favorManager.AddFavor(RomanceCharacter.Nyx, -1);
                    break;
            }
        }
    }

    private void ApplyGroupTalkFavorChanges(int optionIndex)
    {
        if (favorManager == null) return;

        if (currentDay == RomanceDay.Day1_GildedEscape)
        {
            // Group discussion about leaving the capital.
            switch (optionIndex)
            {
                case 0:
                    // “We had no choice. The King is trapped behind liars.”
                    favorManager.AddFavor(RomanceCharacter.Elara, +5);
                    favorManager.AddFavor(RomanceCharacter.Kael, -3);
                    break;
                case 1:
                    // “We should’ve stayed and rooted out the traitors from within.”
                    favorManager.AddFavor(RomanceCharacter.Kael, +5);
                    favorManager.AddFavor(RomanceCharacter.Nyx, -3);
                    break;
                case 2:
                    // “We were never safe there. The whole system is rigged.”
                    favorManager.AddFavor(RomanceCharacter.Nyx, +5);
                    favorManager.AddFavor(RomanceCharacter.Elara, -3);
                    break;
            }
        }
    }

    public void EnterUIMode()
    {
        // Turn off 3D player + crosshair
        if (playerRoot != null) playerRoot.SetActive(false);
        if (crosshair != null) crosshair.SetActive(false);

        // Unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("[RomanceWorld] Entered UI mode (VN-style).");
    }


    public void EnterExplorationMode()
    {
        // Later, when you want 3D exploration:
        if (playerRoot != null)
            playerRoot.SetActive(true);

        if (crosshair != null)
            crosshair.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("[RomanceWorld] Entered Exploration mode (FPS).");
    }

    public void OnExplorationReachedDecisionPoint(RomanceDay day)
    {
        if (day != currentDay) return;

        Debug.Log("[RomanceWorld] Decision point reached for " + day);

        EnterUIMode();
        if (explorationHintPanel != null) explorationHintPanel.SetActive(false);
        ShowCombatChoice();
    }


    private void ShowIntro()
    {
        // Show intro panel, hide others
        if (introPanel != null) introPanel.SetActive(true);

        if (combatChoicePanel != null) combatChoicePanel.SetActive(false);
        if (groupTalkPanel != null) groupTalkPanel.SetActive(false);
        if (personalPanel != null) personalPanel.SetActive(false);

        if (explorationHintPanel != null) explorationHintPanel.SetActive(false);

        if (introText != null)
        {
            introText.text =
                "The Solari capital is in lockdown.\n\n" +
                "Princess Elara has uncovered a conspiracy: her own High Councilor " +
                "is sabotaging every attempt at peace.\n\n" +
                "With Kael and Nyx, she flees the palace under cover of night.\n" +
                "Reach the city gate before it seals, or you’ll never make it to the Frozen Lake rendezvous.";
        }

        Debug.Log("[RomanceWorld] Intro shown.");
    }

    public void OnIntroContinue()
    {
        Debug.Log("[RomanceWorld] Intro Continue pressed");

        if (introPanel != null) introPanel.SetActive(false);

        // Start Day 1 (which will put us into ExplorationMode)
        StartDay(RomanceDay.Day1_GildedEscape);
    }


    private int lastCombatChoiceIndex;

    private void ShowCombatResult(int optionIndex)
    {
        lastCombatChoiceIndex = optionIndex;

        if (combatChoicePanel != null) combatChoicePanel.SetActive(false);
        if (combatResultPanel != null) combatResultPanel.SetActive(true);

        if (combatResultText == null) return;

        if (currentDay == RomanceDay.Day1_GildedEscape)
        {
            if (optionIndex == 0)
            {
                combatResultText.text =
                    "You surge toward the main gate, blade and spells flashing.\n\n" +
                    "The City Watch raise their shields, but you knock weapons aside and slam them to the ground " +
                    "without spilling blood. Elara flinches at every near-miss, but her eyes shine.\n\n" +
                    "Kael grunts. \"Reckless... but effective.\"";
            }
            else
            {
                combatResultText.text =
                    "You tug Nyx toward a side passage, slipping into an old supply corridor " +
                    "while Kael covers your backs.\n\n" +
                    "A patrol rushes past the main street, never noticing you.\n\n" +
                    "Kael nods, impressed. Nyx smirks. \"See? Smart people live longer.\" " +
                    "Elara glances back at the empty gate, whispering, \"Those guards will never know how close they came...\"";
            }
        }

        Debug.Log("[RomanceWorld] Combat result shown.");
    }

    public void OnCombatResultContinue()
    {
        if (combatResultPanel != null) combatResultPanel.SetActive(false);
        ShowGroupTalk();
    }



    #endregion
}

