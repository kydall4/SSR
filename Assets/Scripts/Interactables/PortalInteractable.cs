using UnityEngine;

public class PortalInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string promptText = "Exit World";

    public void Interact()
    {
        Debug.Log("[Portal] Player exited world.");
        GameManager.Instance.OnWorldCompleted();
    }

    public string GetPromptText() => promptText;
}


