using UnityEngine;

// 1. Defines the Clues (Glass, Pen, etc.)
[CreateAssetMenu(fileName = "New Clue", menuName = "Mystery/Evidence Clue")]
public class EvidenceClue : ScriptableObject
{
    public string clueName;
    [TextArea] public string description;
    public bool isPhysicsLayer; // True = Physics, False = Trace
    public GameObject cluePrefab; 
}