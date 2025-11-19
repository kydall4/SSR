using UnityEngine;
using UnityEngine.UI;

public class HubUI : MonoBehaviour
{
    public Button regularPullButton;
    public Button specialPullButton;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        regularPullButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance.regularTickets <= 0) return;

            GameManager.Instance.regularTickets--;
            GameManager.Instance.currentWorld = WorldType.RomanceCombat;
            GameManager.Instance.currentWorldRarity = WorldRarity.Common;
            GameManager.Instance.GoToCurrentWorld();
        });

        specialPullButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance.specialTickets <= 0) return;

            GameManager.Instance.specialTickets--;
            GameManager.Instance.currentWorld = WorldType.RomanceCombat;
            GameManager.Instance.currentWorldRarity = WorldRarity.SSR;
            GameManager.Instance.GoToCurrentWorld();
        });
    }
}
