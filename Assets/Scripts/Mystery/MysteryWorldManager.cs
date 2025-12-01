using System.Collections.Generic;
using UnityEngine;

public class MysteryWorldManager : MonoBehaviour
{
    public static MysteryWorldManager Instance;

    [Header("--- Data Pools (Drag from Project) ---")]
    public List<MysteryScenario> allScenarios;   // Drag CEO, General, Heiress here
    public List<EvidenceClue> physicsClues;      // Drag SafetyStop, Gouges, Drapes
    public List<EvidenceClue> traceClues;        // Drag Pen, Glasses, Outline

    [Header("--- Spawn Points (Drag from Scene) ---")]
    public Transform villainSpawnPoint;
    public Transform flavorPropSpawnPoint;
    public Transform physicsClueSpawnPoint; // Near Window
    public Transform traceClueSpawnPoint;   // Inside Room

    [Header("--- Current Game State (Read Only) ---")]
    public MysteryScenario activeScenario;
    public EvidenceClue activePhysicsClue; // The correct Layer 1 answer
    public EvidenceClue activeTraceClue;   // The correct Layer 2 answer
    
    [Header("--- Player Inventory (Read Only) ---")]
    public List<EvidenceClue> collectedClues = new List<EvidenceClue>();

    // State Machine for the "VS" Battle
    public enum InvestigationState { FindingPhysics, FindingTrace, CaseSolved }
    public InvestigationState currentState = InvestigationState.FindingPhysics;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetupMystery();
    }

    // 1. The Randomizer: Picks the Villain and the Clues for this run
    void SetupMystery()
    {
        // A. Pick the Villain (Flavor)
        if (allScenarios.Count > 0)
        {
            activeScenario = allScenarios[Random.Range(0, allScenarios.Count)];
            Debug.Log("SCENARIO LOADED: " + activeScenario.villainName);

            // Spawn the Villain Model (if assigned)
            if (activeScenario.villainModelPrefab != null)
                Instantiate(activeScenario.villainModelPrefab, villainSpawnPoint.position, villainSpawnPoint.rotation);

            // Spawn the Flavor Prop (Ring/Cigar) (if assigned)
            if (activeScenario.flavorPropPrefab != null)
                Instantiate(activeScenario.flavorPropPrefab, flavorPropSpawnPoint.position, flavorPropSpawnPoint.rotation);
        }

        // B. Pick Layer 1 Clue (Physics)
        if (physicsClues.Count > 0)
        {
            activePhysicsClue = physicsClues[Random.Range(0, physicsClues.Count)];
            Debug.Log("PHYSICS CLUE: " + activePhysicsClue.clueName);
            // In the future, you will spawn the specific 3D object here. 
            // For now, we rely on the greybox object having the right data assigned.
        }

        // C. Pick Layer 2 Clue (Trace)
        if (traceClues.Count > 0)
        {
            activeTraceClue = traceClues[Random.Range(0, traceClues.Count)];
            Debug.Log("TRACE CLUE: " + activeTraceClue.clueName);
        }
    }

    // Called by ClueObjects when they are interacted with
    public void CollectEvidence(EvidenceClue evidence)
    {
        if (!collectedClues.Contains(evidence))
        {
            collectedClues.Add(evidence);
            Debug.Log("COLLECTED: " + evidence.clueName);
            // In the future, you could update a UI inventory display here
        }
    }

    // 2. The Logic Battle: Called when you show evidence to the Police Chief
    public void PresentEvidence(EvidenceClue evidence)
    {
        if (currentState == InvestigationState.FindingPhysics)
        {
            if (evidence == activePhysicsClue)
            {
                Debug.Log("CORRECT! Chief: 'Okay, the window wasn't forced... but maybe he panicked?'");
                currentState = InvestigationState.FindingTrace; // Advance to Phase 2
            }
            else
            {
                Debug.Log("WRONG! Chief: 'That doesn't prove anything.'");
                // Optional: Penalty Logic
            }
        }
        else if (currentState == InvestigationState.FindingTrace)
        {
            if (evidence == activeTraceClue)
            {
                Debug.Log("CORRECT! Chief: 'Wait... he was incapacitated? Lock down the building!'");
                currentState = InvestigationState.CaseSolved; // WIN STATE
                WinGame();
            }
            else
            {
                Debug.Log("WRONG! Chief: 'He could have still jumped.'");
            }
        }
    }

    void WinGame()
    {
        Debug.Log("--- CASE OPEN. YOU WIN. ---");
        // Here you would trigger the "Win Screen" or the Villain's closing line
    }
}