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
        if (clueData == null)
        {
            Debug.LogError("ERROR: This Clue Object has no 'Clue Data' assigned! Did you place it manually?");
            return;
        }

        if (MysteryWorldManager.Instance == null)
        {
            Debug.LogError("ERROR: MysteryWorldManager Not Found! Is '_SYSTEMS' in the scene?");
            return;
        }
        
        // Add to inventory via Manager
        MysteryWorldManager.Instance.CollectEvidence(clueData);
        
        // Optional: Show a popup with description
        Debug.Log(clueData.description);
        
        // Disable object so you can't pick it up twice
        gameObject.SetActive(false);
    }
}