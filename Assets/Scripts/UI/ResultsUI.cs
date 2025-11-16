using UnityEngine;
using UnityEngine.UI;

public class ResultsUI : MonoBehaviour
{
    public Button returnButton;

    void Start()
    {
        returnButton.onClick.AddListener(() =>
        {
            GameManager.Instance.GoToHub();
        });
    }
}
