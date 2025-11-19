using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float interactDistance = 5f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI crosshairText;
    [SerializeField] private TextMeshProUGUI promptText;

    private IInteractable currentInteractable;

    private void Update()
    {
        HandleRaycast();
        HandleInteractInput();
    }

    private void HandleRaycast()
    {
        currentInteractable = null;
        if (promptText != null)
            promptText.text = "";

        Transform cam = transform;

        Debug.DrawRay(cam.position, cam.forward * interactDistance, Color.green);

        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, interactDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;

                if (promptText != null)
                {
                    string prompt = interactable.GetPromptText();
                    promptText.text = $"[E] {prompt}";
                }
            }
        }
    }

    private void HandleInteractInput()
    {
        if (currentInteractable == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }
}
