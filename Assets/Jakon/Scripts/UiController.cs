using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiController : MonoBehaviour
{
    public GameObject optionMenu;
    public GameObject CreditsScreen;
    public GameObject mainMenu;
    public GameObject keybindMenu;
    public GameObject gameOverMenu;
    public GameObject winningScreen;
    public AudioSource winningTheme;
    public AudioSource mainTheme;

  private bool won = false;

    // Start is called before the first frame update
    private void Start()
    {
        mainMenu = GameObject.Find("Canvas/Main Menu");
        optionMenu = GameObject.Find("Canvas/OptionsMenu");
        keybindMenu = GameObject.Find("Canvas/KeybindMenu");
        gameOverMenu = GameObject.Find("Canvas/GameOver");
        winningScreen = GameObject.Find("Canvas/Winning Screen");
        CreditsScreen = GameObject.Find("Canvas/Credits");
        
        optionMenu.SetActive(false);
        keybindMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winningScreen.SetActive(false);
        CreditsScreen.SetActive(false);

    if (won) GameWon();
    }
    public void StartGame()
    {
      SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

  public void BackFromWinScreen() {
    won = false;
  }

  public void GameWon() {
    won = true;

    Debug.Log("GameWon");

    winningScreen.SetActive(true);
    mainMenu.SetActive(false);
    if (!winningTheme.isPlaying) {
      winningTheme.Play();
    }

    mainTheme.Stop();
  }

    public void ShowBindings()
    {
        keybindMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void Options()
    {
 
        optionMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void End()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
  }

    public void Back()
    {
        winningScreen.SetActive(false);
        gameOverMenu.SetActive(false);
        keybindMenu.SetActive(false);
        optionMenu.SetActive(false);
        CreditsScreen.SetActive(false);
        mainMenu.SetActive(true);
        if (!mainTheme.isPlaying)
        {
            mainTheme.Play();
        }
    }
    public void Credits()
    {
        winningScreen.SetActive(false);
        gameOverMenu.SetActive(false);
        keybindMenu.SetActive(false);
        optionMenu.SetActive(false);
        mainMenu.SetActive(false);
        CreditsScreen.SetActive(true);
    }
}



