using UnityEngine.SceneManagement;
using UnityEngine;

public class UIUtility : MonoBehaviour
{
    public GameObject helpText;

    public void LoadLevel(int levelId)
    {
        SceneManager.LoadScene(levelId);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ShowHelpText(bool show)
    {
        helpText.SetActive(show);
    }
}

