using UnityEngine;
using UnityEngine.SceneManagement;

public enum WorldRarity
{
    Common,
    Rare,
    Super_Rare,
    Super_Super_Rare,
    Ultra_Rare,
    Mythic
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Tickets
    public int regularTickets = 1;
    public int specialTickets = 0;
    public int ticketPieces = 0;

    // Current world info
    public string currentWorldId;
    public WorldRarity currentWorldRarity;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GoToHub()
    {
        SceneManager.LoadScene("Hub");
    }

    public void GoToWorld()
    {
        SceneManager.LoadScene("World");
    }

    public void GoToResults()
    {
        SceneManager.LoadScene("Results");
    }
}
