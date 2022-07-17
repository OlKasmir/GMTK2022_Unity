using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource[] AudioSource;
    private float musicVolume = 1f;
    // Start is called before the first frame update
    void Start()
    {
    if (PlayerPrefs.HasKey("Volume")) {
      musicVolume = PlayerPrefs.GetFloat("Volume");
    }
  }

    // Update is called once per frame
    void Update()
    {
        foreach (AudioSource source in AudioSource)
        {
            source.volume = musicVolume;
        }
    }

    public void UpdateVolume(float volume)
    {


        musicVolume = volume;
    PlayerPrefs.SetFloat("Volume", musicVolume);
    }
}
