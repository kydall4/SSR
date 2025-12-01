using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HubPullController : MonoBehaviour
{
    [Header("HUD Texts")]
    [SerializeField] private TextMeshProUGUI normalTicketText;
    [SerializeField] private TextMeshProUGUI specialTicketText;
    [SerializeField] private TextMeshProUGUI fragmentText;
    [SerializeField] private TextMeshProUGUI currentWorldText;

    [Header("Panels & Buttons")]
    [SerializeField] private GameObject machinePanel;           // the close-up panel
    [SerializeField] private Button pullWorldButton;            // on MachinePanel
    [SerializeField] private Button pullSpecialWorldButton;     // on MachinePanel (optional)
    [SerializeField] private Button hubGoButton;                // on the main hub (outside MachinePanel)

    [Header("Reveal UI")]
    [SerializeField] private WorldRevealUI revealUI;

    private void Start()
    {
        if (pullWorldButton != null)
            pullWorldButton.onClick.AddListener(OnPullWorldClicked);

        if (pullSpecialWorldButton != null)
            pullSpecialWorldButton.onClick.AddListener(OnPullSpecialWorldClicked);

        if (hubGoButton != null)
        {
            hubGoButton.onClick.AddListener(OnGoClicked);
            hubGoButton.gameObject.SetActive(false);   // not visible until a pull is finished
            hubGoButton.interactable = false;
        }

        if (revealUI != null)
            revealUI.onRevealFinished += OnRevealFinished;

        UpdateUI();
    }

    private void UpdateUI()
    {
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("[HubPullController] GameManager.Instance is NULL in UpdateUI.");
            return;
        }

        // Simple counts for now
        normalTicketText.text = $": {gm.regularTickets}";
        specialTicketText.text = $": {gm.specialTickets}";
        fragmentText.text = $": {gm.specialFragments}/3";

        if (gm.currentWorld == WorldType.None)
            currentWorldText.text = "Current World: None";
        else
            currentWorldText.text = $"Current World: {gm.currentWorld}";

        // Only allow pulls if they have tickets
        if (pullWorldButton != null)
            pullWorldButton.interactable = gm.regularTickets > 0;

        if (pullSpecialWorldButton != null)
            pullSpecialWorldButton.interactable = gm.specialTickets > 0;
    }

    private void OnPullWorldClicked()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;
        if (gm.regularTickets <= 0)
        {
            Debug.Log("[HubPullController] No regular tickets.");
            return;
        }

        gm.PullNormalWorld();
        Debug.Log("[HubPullController] Pulled normal world: " + gm.currentWorld);

        if (hubGoButton != null)
        {
            hubGoButton.gameObject.SetActive(false);   // hide until reveal finishes
            hubGoButton.interactable = false;
        }

        revealUI.StartReveal(gm.currentWorld);
        UpdateUI();
    }

    private void OnPullSpecialWorldClicked()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;
        if (gm.specialTickets <= 0)
        {
            Debug.Log("[HubPullController] No special tickets.");
            return;
        }

        gm.PullSpecialWorld();
        Debug.Log("[HubPullController] Pulled SPECIAL world: " + gm.currentWorld);

        if (hubGoButton != null)
        {
            hubGoButton.gameObject.SetActive(false);
            hubGoButton.interactable = false;
        }

        revealUI.StartReveal(gm.currentWorld);
        UpdateUI();
    }

    private void OnGoClicked()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        Debug.Log("[HubPullController] Go clicked. Loading world: " + gm.currentWorld);
        gm.GoToCurrentWorld();
    }

    private void OnRevealFinished()
    {
        Debug.Log("[HubPullController] Reveal finished. Returning to hub, enabling Go.");

        // Hide the machine close-up panel
        if (machinePanel != null)
            machinePanel.SetActive(false);

        // Show and enable the hub Go button
        if (hubGoButton != null)
        {
            hubGoButton.gameObject.SetActive(true);
            hubGoButton.interactable = true;
        }
    }
}
