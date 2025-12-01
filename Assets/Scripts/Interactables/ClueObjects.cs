using UnityEngine;

public class ClueObject : MonoBehaviour, IInteractable
{
    public EvidenceClue clueData; // Drag the specific ScriptableObject here

    public string GetPromptText()
    {
        return "Inspect " + clueData.clueName;
    }

    public void Interact()
    {
        // Add to inventory via Manager
        MysteryWorldManager.Instance.CollectEvidence(clueData);
        
        // Optional: Show a popup with description
        Debug.Log(clueData.description);
        
        // Disable object so you can't pick it up twice
        gameObject.SetActive(false);
    }
}