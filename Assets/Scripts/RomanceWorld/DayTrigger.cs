using UnityEngine;

public class DayTrigger : MonoBehaviour
{
    public RomanceWorldController worldController;
    public RomanceDay dayToTrigger;
    public bool triggerOnlyOnce = true;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnlyOnce) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;
        Debug.Log("[RomanceWorld] Trigger hit for day: " + dayToTrigger);

        worldController.OnExplorationReachedDecisionPoint(dayToTrigger);
    }
}
