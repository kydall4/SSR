using UnityEngine;

[System.Serializable]
public struct ArgumentOption
{
    [TextArea] public string optionText;      // What YOU say (e.g. "The screws were removed.")
    [TextArea] public string chiefResponse;   // What CHIEF says (e.g. "Removed? That takes time.")
    public bool isCorrectLogic;               // Does this win the argument?
}

// Defines the Clues (Glass, Pen, etc.)
[CreateAssetMenu(fileName = "New Clue", menuName = "Mystery/Evidence Clue")]
public class EvidenceClue : ScriptableObject
{
    public string clueName;
    [TextArea] public string description;

    public Sprite inspectImage; // Drag the 2D art here

    public bool isPhysicsLayer; // True = Physics, False = Trace
    public GameObject cluePrefab; 

    [Header("VS Battle Options")]
    // Add 3 options here in the Inspector!
    public ArgumentOption[] arguments;
}