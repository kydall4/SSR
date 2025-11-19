using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HubPullController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI ticketInfoText;
    [SerializeField] private TextMeshProUGUI specialTicketInfoText;
    [SerializeField] private TextMeshProUGUI currentWorldText;
    [SerializeField] private Button pullButton;
    [SerializeField] private Button goButton;

    private void Start()
    {
        pullButton.onClick.AddListener(OnPullClicked);
        goButton.onClick.AddListener(OnGoClicked);

        goButton.interactable = false; // Locked until player pulls
        UpdateUI();
    }

    private void UpdateUI()
    {
        var gm = GameManager.Instance;

        ticketInfoText.text = gm.regularTickets == 1
            ? "Regular Ticket: YES"
            : "Regular Ticket: NO";

        specialTicketInfoText.text =
            $"Special Tickets: {gm.specialTickets}  Fragments: {gm.specialFragments}/3";

        if (gm.currentWorld == WorldType.None)
            currentWorldText.text = "Current World: None";
        else
            currentWorldText.text = $"Current World: {gm.currentWorld}";
    }

    private void OnPullClicked()
    {
        var gm = GameManager.Instance;

        if (gm.specialTickets > 0)
            gm.PullSpecialWorld();
        else if (gm.regularTickets > 0)
            gm.PullNormalWorld();
        else
        {
            Debug.Log("No tickets available.");
            return;
        }

        goButton.interactable = true;
        UpdateUI();
    }

    private void OnGoClicked()
    {
        GameManager.Instance.GoToCurrentWorld();
    }
}
