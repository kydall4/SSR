using UnityEngine;

public enum RomanceTriggerType
{
    Day1_GateApproach,
    Day1_StealthEnd,
    Day1_PersonalCamp,

    Day2_EvidenceTower,
    Day2_ReturnStreet,
    Day2_PersonalCamp,

    Day3_ConfrontationStart,
    Day3_FinalEscape,
    Day3_PersonalFinal
}

public class RomanceTrigger : MonoBehaviour
{
    public RomanceWorldController controller;
    public RomanceTriggerType triggerType;
    public bool triggerOnce = true;

    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && used) return;
        if (!other.CompareTag("Player")) return;

        used = true;
//        controller.OnWorldTrigger(triggerType);
    }
}
