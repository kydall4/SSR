using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public Button startButton;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        startButton.onClick.AddListener(() =>
        {
            GameManager.Instance.GoToHub();
        });
    }
}

