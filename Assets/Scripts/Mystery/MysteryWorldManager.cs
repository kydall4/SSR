using System.Collections.Generic;
using UnityEngine;
using System.Collections; // Required for Coroutines

public class MysteryWorldManager : MonoBehaviour
{
    public static MysteryWorldManager Instance;

    [Header("--- Debug Settings ---")]
    [Tooltip("Set to -1 for Random. Set to 0, 1, 2 etc to force a specific scenario.")]
    public int forceScenarioIndex = -1; // <--- ADD THIS

    [Header("--- Data Pools ---")]
    public List<MysteryScenario> allScenarios;
    public List<EvidenceClue> physicsClues;
    public List<EvidenceClue> traceClues;

    [Header("--- Spawn Points ---")]
    public Transform villainSpawnPoint;
    // Order must match the 'All Scenarios' list (0=CEO, 1=Heiress, 2=General)
    public List<Transform> flavorPropSpawnPoints;
    public Transform physicsClueSpawnPoint;
    public Transform traceClueSpawnPoint;

    [Header("--- Current Game State ---")]
    public MysteryScenario activeScenario;
    public EvidenceClue activePhysicsClue;
    public EvidenceClue activeTraceClue;
    
    public List<EvidenceClue> collectedEvidence = new List<EvidenceClue>();

    public enum InvestigationState { FindingPhysics, FindingTrace, CaseSolved }
    public InvestigationState currentState = InvestigationState.FindingPhysics;

    void Awake() { Instance = this; }

    void Start() { SetupMystery(); }

    void SetupMystery()
    {
        // 1. Pick Scenario (Villain)
        if (allScenarios.Count > 0)
        {
            int indexToUse = -1;
            if (forceScenarioIndex >= 0 && forceScenarioIndex < allScenarios.Count)
            {
                indexToUse = forceScenarioIndex;
                Debug.Log($"[Mystery] DEBUG: Forcing Scenario #{indexToUse}");
            }
            else
            {
                // If -1 (or invalid), pick Randomly
                indexToUse = Random.Range(0, allScenarios.Count);
            }

            activeScenario = allScenarios[indexToUse];
            
            //activeScenario = allScenarios[Random.Range(0, allScenarios.Count)];
            
            if (activeScenario.villainModelPrefab != null) {
                // Spawn the generic card prefab
                GameObject villainObj = Instantiate(activeScenario.villainModelPrefab, villainSpawnPoint.position, villainSpawnPoint.rotation);

                // Find the SpriteRenderer and inject the specific scenario image
                SpriteRenderer sr = villainObj.GetComponent<SpriteRenderer>();
                if (sr != null && activeScenario.villainPortrait != null)
                {
                    sr.sprite = activeScenario.villainPortrait;
                }
            }

            if (activeScenario.flavorPropPrefab != null)
            {
                // Safety Check: Do we have a spawn point for this index?
                Transform targetSpawn = (indexToUse < flavorPropSpawnPoints.Count) 
                                        ? flavorPropSpawnPoints[indexToUse] 
                                        : flavorPropSpawnPoints[0]; // Fallback
                
                GameObject newProp = Instantiate(
                    activeScenario.flavorPropPrefab, 
                    targetSpawn.position, 
                    activeScenario.flavorPropPrefab.transform.rotation
                );
                
                // Inject Data to the Prop
                ClueObject logic = newProp.GetComponent<ClueObject>();
                if (logic != null && activeScenario.flavorPropData != null)
                {
                    logic.clueData = activeScenario.flavorPropData;
                }
            }
        }

        // 2. Pick Physics Clue
        if (physicsClues.Count > 0)
        {
            activePhysicsClue = physicsClues[Random.Range(0, physicsClues.Count)];
            if (activePhysicsClue.cluePrefab != null)
            {
                GameObject newClue = Instantiate(activePhysicsClue.cluePrefab, physicsClueSpawnPoint.position, physicsClueSpawnPoint.rotation);
                ClueObject logic = newClue.GetComponent<ClueObject>();
                if (logic != null) logic.clueData = activePhysicsClue;
            }
        }

        // 3. Pick Trace Clue
        if (traceClues.Count > 0)
        {
            activeTraceClue = traceClues[Random.Range(0, traceClues.Count)];
            if (activeTraceClue.cluePrefab != null)
            {
                GameObject newClue = Instantiate(
                    activeTraceClue.cluePrefab, 
                    traceClueSpawnPoint.position, 
                    activeTraceClue.cluePrefab.transform.rotation
                );
                
                ClueObject logic = newClue.GetComponent<ClueObject>();
                if (logic != null) logic.clueData = activeTraceClue;
            }
        }
    }

    public void CollectEvidence(EvidenceClue clue)
    {
        if (!collectedEvidence.Contains(clue))
        {
            collectedEvidence.Add(clue);
            
            // FIX 1: Tell the UI to update the buttons!
            if (MysteryUIManager.Instance != null)
            {
                // 1. Update the Notebook
                MysteryUIManager.Instance.RefreshContent(); 

                // 2. SHOW THE THOUGHT (The Fix)
                // Pass the description AND the image to the UI
                MysteryUIManager.Instance.ShowInspection("Thinking: " + clue.description, clue.inspectImage);
            }
        }
    }

    // Step 1: Player selects item from Notebook (via Keys 1-9)
    public void PresentEvidence(EvidenceClue evidence)
    {
        bool isCorrectItem = false;
        
        // Check if item is correct for current phase
        if (currentState == InvestigationState.FindingPhysics && evidence == activePhysicsClue)
        {
            isCorrectItem = true;
        }
        else if (currentState == InvestigationState.FindingTrace && evidence == activeTraceClue)
        {
            isCorrectItem = true;
        }

        if (isCorrectItem)
        {
            //If correct item, show the Argument Choices!
            if (MysteryUIManager.Instance != null)
                MysteryUIManager.Instance.ShowArgumentChoices(evidence);
        }
        else
        {
            // If wrong item, instant rejection (No choices)
            if (MysteryUIManager.Instance != null)
                MysteryUIManager.Instance.ShowDialogue($"Chief: \"A {evidence.clueName}? Irrelevant. That proves nothing.\"");
        }
    }

    // Step 2: Player selects specific argument logic
    public void ResolveArgument(EvidenceClue clue, int choiceIndex)
    {
        if (choiceIndex >= clue.arguments.Length) return;

        ArgumentOption choice = clue.arguments[choiceIndex];

        if (choice.isCorrectLogic)
        {
            // SUCCESS
            if (currentState == InvestigationState.FindingPhysics)
            {
                MysteryUIManager.Instance.ShowDialogue(new string[] { 
                    $"You: \"{choice.optionText}\"", 
                    $"Chief: \"{choice.chiefResponse}\"",
                    "Chief: \"Okay... so he didn't jump voluntarily. But maybe he panicked?\""
                });
                currentState = InvestigationState.FindingTrace;
            }
            else if (currentState == InvestigationState.FindingTrace)
            {
                MysteryUIManager.Instance.ShowDialogue(new string[] { 
                    $"You: \"{choice.optionText}\"", 
                    $"Chief: \"{choice.chiefResponse}\"",
                    "Chief: \"Wait... that means he was helpless. This was a murder. Lock down the building!\""
                });
                currentState = InvestigationState.CaseSolved;
                StartCoroutine(WinGameSequence()); // <--- CALL WIN LOGIC
            }
        }
        else
        {
            // FAILURE (Right Clue, Wrong Argument)
            MysteryUIManager.Instance.ShowDialogue(new string[] { 
                $"You: \"{choice.optionText}\"", 
                $"Chief: \"{choice.chiefResponse}\"",
                "Chief: \"Try again when you have a real theory.\""
            });
        }
    }

    // --- NEW WIN LOGIC ---
    IEnumerator WinGameSequence()
    {
        // 1. Wait for player to read the Chief's final line
        yield return new WaitForSeconds(4f);

        Debug.Log("--- CASE SOLVED! RETURNING TO HUB ---");

        // 2. Call the Main Game Manager to handle rewards and scene switching
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWorldCompleted();
        }
        else
        {
            Debug.LogError("No GameManager found! Cannot exit level.");
        }
    }
}