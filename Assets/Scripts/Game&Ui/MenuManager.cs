using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes!
public class MenuManager : MonoBehaviour
{

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelsPanel;


    public void OnLevelsButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        levelsPanel.SetActive(true);
    }

    public void OnBackButtonClicked()
    {
        levelsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("MainGame");
    }
    public void OnExitButtonClicked()
    {
        Debug.Log("Exiting game..."); // This log shows up in the editor
        Application.Quit(); // This only works in a built game, not in the editor
    }
}
