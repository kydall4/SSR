using System.Collections.Generic;
using UnityEngine;

public class MysteryWorldManager : MonoBehaviour
{
    public static MysteryWorldManager Instance;

    [Header("--- Data Pools ---")]
    public List<MysteryScenario> allScenarios;
    public List<EvidenceClue> physicsClues;
    public List<EvidenceClue> traceClues;

    [Header("--- Spawn Points ---")]
    public Transform villainSpawnPoint;
    public Transform flavorPropSpawnPoint;
    public Transform physicsClueSpawnPoint;
    public Transform traceClueSpawnPoint;

    [Header("--- Current Game State ---")]
    public MysteryScenario activeScenario;
    public EvidenceClue activePhysicsClue;
    public EvidenceClue activeTraceClue;
    
    // Inventory
    public List<EvidenceClue> collectedEvidence = new List<EvidenceClue>();

    public enum InvestigationState { FindingPhysics, FindingTrace, CaseSolved }
    public InvestigationState currentState = InvestigationState.FindingPhysics;

    void Awake() { Instance = this; }

    void Start() { SetupMystery(); }

    void SetupMystery()
    {
        // A. Pick Villain & Prop
        if (allScenarios.Count > 0)
        {
            activeScenario = allScenarios[Random.Range(0, allScenarios.Count)];
            Debug.Log("SCENARIO LOADED: " + activeScenario.villainName);

            if (activeScenario.villainModelPrefab != null)
                Instantiate(activeScenario.villainModelPrefab, villainSpawnPoint.position, villainSpawnPoint.rotation);

            // Spawn the Flavor Prop (Ring/Cigar)
            if (activeScenario.flavorPropPrefab != null)
            {
                GameObject newProp = Instantiate(activeScenario.flavorPropPrefab, flavorPropSpawnPoint.position, flavorPropSpawnPoint.rotation);
                
                // Inject Data
                ClueObject logic = newProp.GetComponent<ClueObject>();
                if (logic != null)
                {
                    if (activeScenario.flavorPropData != null)
                        logic.clueData = activeScenario.flavorPropData;
                    else
                        Debug.LogError("SetupMystery: Scenario '" + activeScenario.villainName + "' is missing Flavor Prop Data!");
                }
            }
        }

        // B. Pick Physics Clue
        if (physicsClues.Count > 0)
        {
            activePhysicsClue = physicsClues[Random.Range(0, physicsClues.Count)];
            Debug.Log("PHYSICS CLUE: " + activePhysicsClue.clueName);
            
            if (activePhysicsClue.cluePrefab != null)
            {
                GameObject newClue = Instantiate(activePhysicsClue.cluePrefab, physicsClueSpawnPoint.position, physicsClueSpawnPoint.rotation);
                ClueObject logic = newClue.GetComponent<ClueObject>();
                if (logic != null) logic.clueData = activePhysicsClue;
            }
        }

        // C. Pick Trace Clue
        if (traceClues.Count > 0)
        {
            activeTraceClue = traceClues[Random.Range(0, traceClues.Count)];
            Debug.Log("TRACE CLUE: " + activeTraceClue.clueName);

            if (activeTraceClue.cluePrefab != null)
            {
                GameObject newClue = Instantiate(activeTraceClue.cluePrefab, traceClueSpawnPoint.position, traceClueSpawnPoint.rotation);
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
            Debug.Log("COLLECTED: " + clue.clueName);
        }
    }
}