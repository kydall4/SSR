using UnityEngine;

// 1. This creates the "Evidence Clue" option in the menu
[CreateAssetMenu(fileName = "New Clue", menuName = "Mystery/Evidence Clue")]
public class EvidenceClue : ScriptableObject
{
    public string clueName;           // e.g., "Safety Stop"
    [TextArea] public string description; // The text shown when inspected
    public bool isPhysicsLayer;       // True = Layer 1 (Physics), False = Layer 2 (Trace)
}

// 2. This creates the "Villain Scenario" option in the menu
[CreateAssetMenu(fileName = "New Scenario", menuName = "Mystery/Villain Scenario")]
public class MysteryScenario : ScriptableObject
{
    public string villainName;
    public GameObject villainModelPrefab; // The 3D model to spawn
    public GameObject flavorPropPrefab;   // Ring/Cigar/Bracelet
    
    [Header("Hidden Item Details")]
    public string hiddenItemName;         // e.g. "Logistics Invoice"
    [TextArea] public string hiddenItemText; 
    
    [Header("Visual Tell")]
    public string animationTriggerName;   // e.g. "TouchFlower"
}