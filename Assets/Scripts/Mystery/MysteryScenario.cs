using UnityEngine;

[CreateAssetMenu(fileName = "New Scenario", menuName = "Mystery/Villain Scenario")]
public class MysteryScenario : ScriptableObject
{
    public string villainName;
    public GameObject villainModelPrefab;
    
    [Header("Flavor Prop")]
    public GameObject flavorPropPrefab;
    public EvidenceClue flavorPropData; 
    
    [Header("Hidden Item Details")]
    public string hiddenItemName;
    [TextArea] public string hiddenItemText; 
    
    [Header("Visual Tell")]
    public string animationTriggerName;
}