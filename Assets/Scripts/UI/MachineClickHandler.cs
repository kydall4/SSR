using UnityEngine;

public class MachineClickHandler : MonoBehaviour
{
    [SerializeField] private GameObject machinePanel;

    public void OnMachineClicked()
    {
        if (machinePanel != null)
        {
            machinePanel.SetActive(true);
        }
        else
        {
            Debug.LogError("[MachineClickHandler] machinePanel is not assigned in the Inspector.");
        }
    }
}
