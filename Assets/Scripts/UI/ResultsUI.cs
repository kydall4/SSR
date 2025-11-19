using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ResultsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button returnButton;

    private void Start()
    {
        var gm = GameManager.Instance;

        rewardText.text =
            $"Regular Ticket: {(gm.regularTickets == 1 ? "YES" : "NO")}\n" +
            $"Special Tickets: {gm.specialTickets}\n" +
            $"Fragments: {gm.specialFragments}/3";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        returnButton.onClick.AddListener(ReturnToHub);
    }

    private void ReturnToHub()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("Hub");
    }
}
