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
            activeScenario = allScenarios[Random.Range(0, allScenarios.Count)];
            
            if (activeScenario.villainModelPrefab != null)
                Instantiate(activeScenario.villainModelPrefab, villainSpawnPoint.position, villainSpawnPoint.rotation);

            if (activeScenario.flavorPropPrefab != null)
            {
                GameObject newProp = Instantiate(activeScenario.flavorPropPrefab, flavorPropSpawnPoint.position, flavorPropSpawnPoint.rotation);
                
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
            
            // FIX 1: Tell the UI to update the buttons!
            if (MysteryUIManager.Instance != null)
            {
                // 1. Update the Notebook
                MysteryUIManager.Instance.RefreshContent(); 

                // 2. SHOW THE THOUGHT (The Fix)
                // We use the 'description' field from the data file
                MysteryUIManager.Instance.ShowDialogue("Thinking: " + clue.description);
            }
        }
    }

    public void PresentEvidence(EvidenceClue evidence)
    {
        // FIX 2: Send text to the UI, not just Debug.Log
        if (currentState == InvestigationState.FindingPhysics)
        {
            if (evidence == activePhysicsClue)
            {
                if (MysteryUIManager.Instance != null)
                    MysteryUIManager.Instance.ShowDialogue("Chief: 'Okay, the window wasn't forced... but maybe he panicked?'");
                
                currentState = InvestigationState.FindingTrace;
            }
            else
            {
                if (MysteryUIManager.Instance != null)
                    MysteryUIManager.Instance.ShowDialogue("Chief: 'That proves nothing. Stop wasting my time.'");
            }
        }
        else if (currentState == InvestigationState.FindingTrace)
        {
            if (evidence == activeTraceClue)
            {
                if (MysteryUIManager.Instance != null)
                    MysteryUIManager.Instance.ShowDialogue("Chief: 'Wait... incapacitated? Lock down the building!'");
                
                currentState = InvestigationState.CaseSolved;
            }
            else
            {
                if (MysteryUIManager.Instance != null)
                    MysteryUIManager.Instance.ShowDialogue("Chief: 'He could have still jumped.'");
            }
        }
    }
    
    // Debug keys for testing without clicking
    void Update()
    {
        // Key 1: Present Physics Clue (Only if collected)
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            if (collectedEvidence.Contains(activePhysicsClue))
            {
                PresentEvidence(activePhysicsClue);
            }
            else
            {
                Debug.Log("DEBUG: You haven't found the Physics Clue (" + activePhysicsClue.clueName + ") yet!");
            }
        }

        // Key 2: Present Trace Clue (Only if collected)
        if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            if (collectedEvidence.Contains(activeTraceClue))
            {
                PresentEvidence(activeTraceClue);
            }
            else
            {
                Debug.Log("DEBUG: You haven't found the Trace Clue (" + activeTraceClue.clueName + ") yet!");
            }
        }
    }
}