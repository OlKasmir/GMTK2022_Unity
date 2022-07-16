using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{
    // Start is called before the first frame update
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
        Debug.Log("Input Option Scene here");
    }
    public void End()
    {
        //Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}



