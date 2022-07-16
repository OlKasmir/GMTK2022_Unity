using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiController : MonoBehaviour
{
    public GameObject optionMenu;
    public GameObject mainMenu;
    public GameObject canvas;
    // Start is called before the first frame update
    private void Start()
    {
        mainMenu = GameObject.Find("Canvas/Main Menu");
        optionMenu = GameObject.Find("Canvas/OptionsMenu");
        optionMenu.SetActive(false);
    }
    public void StartGame()
    {
        Debug.Log("Input First Game Scene here");
    }

    public void ShowBindings()
    {
        Debug.Log("Input Keybind Scene here");
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
        optionMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}



