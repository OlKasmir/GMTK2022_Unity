using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiController : MonoBehaviour
{
    public GameObject optionMenu;
    public GameObject mainMenu;
    public GameObject keybindMenu;
    // Start is called before the first frame update
    private void Start()
    {
        mainMenu = GameObject.Find("Canvas/Main Menu");
        optionMenu = GameObject.Find("Canvas/OptionsMenu");
        keybindMenu = GameObject.Find("Canvas/KeybindMenu");
        optionMenu.SetActive(false);
        keybindMenu.SetActive(false);
    }
    public void StartGame()
    {
        Debug.Log("Input First Game Scene here");
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
        keybindMenu.SetActive(false);
        optionMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}



