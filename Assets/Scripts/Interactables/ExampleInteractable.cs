using UnityEngine;

public class ExampleInteractable : MonoBehaviour, IInteractable
{
    public string prompt = "Activate Object";

    public string GetPromptText()
    {
        return prompt;
    }

    public void Interact()
    {
        Debug.Log("Interaction triggered!");
    }
}
