using UnityEngine;
using UnityEngine.UI;

public class HubUI : MonoBehaviour
{
    public Button regularPullButton;
    public Button specialPullButton;

    void Start()
    {
        regularPullButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance.regularTickets <= 0) return;

            GameManager.Instance.regularTickets--;
            GameManager.Instance.currentWorldId = "WorldB_Romance";
            GameManager.Instance.currentWorldRarity = WorldRarity.Common;
            GameManager.Instance.GoToWorld();
        });

        specialPullButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance.specialTickets <= 0) return;

            GameManager.Instance.specialTickets--;
            GameManager.Instance.currentWorldId = "WorldD_Mythic";
            GameManager.Instance.currentWorldRarity = WorldRarity.Mythic;
            GameManager.Instance.GoToWorld();
        });
    }
}
