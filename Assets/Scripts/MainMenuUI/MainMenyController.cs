using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject nameInputPanel;

    [Header("UI")]
    public TMP_InputField usernameInput;

    private void Start()
    {
        mainMenuPanel.SetActive(true);
        nameInputPanel.SetActive(false);
    }

    public void OnStartPressed()
    {
        mainMenuPanel.SetActive(false);
        nameInputPanel.SetActive(true);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }

    public void OnBackPressed()
    {
        mainMenuPanel.SetActive(true);
        nameInputPanel.SetActive(false);
    }

    public void OnNextPressed()
    {
        string username = usernameInput.text;

        if (string.IsNullOrWhiteSpace(username))
            return;

        Debug.Log("Username: " + username);

        PlayerData.Username = username;

        SceneManager.LoadScene("Oscar");
      
    }

    
}

public static class PlayerData
{
    public static string Username;
}