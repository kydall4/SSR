using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(() =>
        {
            GameManager.Instance.GoToHub();
        });
    }
}

