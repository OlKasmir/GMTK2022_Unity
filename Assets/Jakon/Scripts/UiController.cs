using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiController : MonoBehaviour
{
    public GameObject optionMenu;
    public GameObject mainMenu;
    public GameObject keybindMenu;
    public GameObject gameOverMenu;
    public GameObject winningScreen;
    public AudioSource winningTheme;
    public AudioSource mainTheme;
    // Start is called before the first frame update
    private void Start()
    {
        mainMenu = GameObject.Find("Canvas/Main Menu");
        optionMenu = GameObject.Find("Canvas/OptionsMenu");
        keybindMenu = GameObject.Find("Canvas/KeybindMenu");
        gameOverMenu = GameObject.Find("Canvas/GameOver");
        winningScreen = GameObject.Find("Canvas/Winning Screen");
        
        optionMenu.SetActive(false);
        keybindMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winningScreen.SetActive(false);
    }
    public void StartGame()
    {
        winningScreen.SetActive(true);
        mainMenu.SetActive(false);
        if (!winningTheme.isPlaying)
        {
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
        //Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void Back()
    {
        winningScreen.SetActive(false);
        gameOverMenu.SetActive(false);
        keybindMenu.SetActive(false);
        optionMenu.SetActive(false);
        mainMenu.SetActive(true);
        if (!mainTheme.isPlaying)
        {
            mainTheme.Play();
        }
    }
}



