using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    public void OnStartClicked()
    {
        SceneManager.LoadScene("CharacterSelect");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
}
