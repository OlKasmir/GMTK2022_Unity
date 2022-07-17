using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using System.Text.RegularExpressions;
using Pooling;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AudioManager : MonoBehaviour {
  #region Singleton Pattern
  private static AudioManager _instance;
  public static AudioManager Instance { 
    get {
      if (_instance != null)
        return _instance;

      _instance = FindObjectOfType<AudioManager>();
      return _instance;
    }
  }

  private void Awake() {
    if (_instance != null && _instance != this) {
      Destroy(this.gameObject);
      return;
    } else {
      _instance = this;
    }

    // regex = new Regex(".*", RegexOptions.Compiled);

#if UNITY_EDITOR
    LoadSounds();
#endif
  }
  #endregion

  #region References
  [Header("Main")]
  public string soundFilesPath;
  public string soundFilesExtension;
  [SerializeField] // TODO: Remove [SerializeField] when loading is implemented
  protected AudioCollection audioCollection;
  public GameObject masterAudioSourcePrefab;
  public GameObject audioSourceParent;
  protected ObjectPool _soundPool;

  [Header("Audio Mixer")]
  public AudioMixer audioMixer;
  public AudioMixerGroup audioMixerGroupMusic;
  public AudioMixerGroup audioMixerGroupInterface;
  #endregion


  #region Variables
  [Header("3D Sound")]
  public AudioListener audioListener;
  public AnimationCurve audioVolumeFalloff;
  public float minDistance;
  public float maxDistance;

  [Header("Music")]
  protected AudioSource _audioSourceMusic;
  [SerializeField]
  protected bool _musicEnabled = true;


  private Dictionary<string, List<AudioClip>> audioClipsFromRegex = new Dictionary<string, List<AudioClip>>();
  // private Regex regex;
  #endregion

  //[Header("Queuing")]
  //// Saves all sounds commanded to play
  //protected List<string> soundQueue;
  //[SerializeField]
  //[Tooltip("If enabled sounds will be queued and duplicates will be discarded")]
  //protected bool soundQueueEnabled;
  //[SerializeField]
  //[Range(0.001f, 1.0f)]
  //[Tooltip("Time between clearing the sound queue and playing sounds")]
  //protected float _timeBetweenQueueTicks = 0.05f;


  [SerializeField]
  [Tooltip("The minimum time between playing the same sound")]
  protected float _timeBetweenSameSound = 0.1f;

  protected Dictionary<string, float> _dictionaryLastTimePlayed = new Dictionary<string, float>();



  #region Getter and Setter
  //public float TimeBetweenQueueTicks => Mathf.Clamp(_timeBetweenQueueTicks, 0.001f, 1.0f);
  public bool MusicEnabled { get => _musicEnabled; set {
      _musicEnabled = value;

      if(!_musicEnabled)
        StopMusic();
    } 
  }

  public ObjectPool SoundPool => _soundPool ??= GetComponent<ObjectPool>();
  #endregion


  #region UnityMethods
  public void Start() {
    if(audioListener == null)
      audioListener = FindObjectOfType<AudioListener>();

    //if(soundQueueEnabled)
    //  StartCoroutine(PlaySoundQueue());
  }

  public void OnDestroy() {
    StopAllCoroutines();
  }
  #endregion

  #region Methods
  /// <summary>
  /// Todo: Play sounds delayed. (There will be a constant delay for every sound but they won't play all at the same time)
  /// </summary>
  /// <returns></returns>
  //protected IEnumerator PlaySoundQueue() {
  //  soundQueue = new List<string>();

  //  while (true) {
  //    yield return new WaitForSeconds(_timeBetweenQueueTicks);

  //    soundQueue.RemoveEmpties(); // TODO: Add empty string "" to method
  //    soundQueue.RemoveDuplicates();

  //    foreach(string audioName in soundQueue) {
  //      PlaySound(audioName, true);
  //    }

  //    soundQueue.Clear();
  //  }
  //}

#if UNITY_EDITOR
  /// <summary>
  /// Loads all sounds from the project and saves references in a scriptable object file (ONLY WORKS IN EDITOR)
  /// </summary>
  protected void LoadSounds() {
    var assets = AssetDatabase.FindAssets($"t:{typeof(AudioClip).Name}");
    foreach (var asset in assets) {
      string path = AssetDatabase.GUIDToAssetPath(asset);
      audioCollection.audioClips.AddUnique(AssetDatabase.LoadAssetAtPath<AudioClip>(path));
    }

    audioCollection.audioClips.RemoveEmpties();

    Debug.Log("<color=#99ff66>Loaded " + audioCollection.audioClips.Count + " Sounds ...</color>");
  }
#endif

  public Vector3 GetAudioListenerPosition() {
    return audioListener.transform.position;
  }
  #endregion

  #region Music
  public void SetMusicEnabled(bool enabled) {
    MusicEnabled = enabled;
  }

  public bool IsMusicPlaying() {
    return _audioSourceMusic != null;
  }

  public void PlayMusic(AudioClip audioClip) {
    if (!_musicEnabled)
      return;

    if (IsMusicPlaying())
      StopMusic();

    _audioSourceMusic = Instantiate(masterAudioSourcePrefab, Vector3.zero, Quaternion.identity).GetComponent<AudioSource>();
    _audioSourceMusic.clip = audioClip;
    _audioSourceMusic.volume = 1.0f;
    _audioSourceMusic.outputAudioMixerGroup = audioMixerGroupMusic;
    _audioSourceMusic.loop = true;
    _audioSourceMusic.Play();

    audioMixer.SetFloat("MusicVolume", -80.0f);
    StartCoroutine(MethodExtensions.StartFade(audioMixer, "MusicVolume", 2.0f, 1.0f));
  }

  public void StopMusic(float fadeDuration = 0.0f) {
    if (_audioSourceMusic == null) {
      return;
    }

    if (fadeDuration > 0.0f) {
      StartCoroutine(MethodExtensions.StartFade(audioMixer, "MusicVolume", fadeDuration, -80.0f, _audioSourceMusic));
    } else {
      Destroy(_audioSourceMusic.gameObject);
    }
  }
  #endregion


  #region Sounds
  /// <summary>
  /// Plays a sound by sound name (Using regex).
  /// </summary>
  /// <param name="soundName">Sound* will play a random sound containing Sound in front (Regex)</param>
  /// <param name="forcePlay">If true the sound will play instantly (Not queuing if enabled)</param>
  public AudioSource PlaySound(string soundName, bool forcePlay = false, float volumeScale = 1f) {
    // Debug.Log($"Playing Sound {soundName}");

    if (soundName == null || soundName == "")
      return null;

    //if(!forcePlay && soundQueueEnabled) {
    //  soundQueue.AddUnique(soundName);
    //  return;
    //}


    // Check if sound is allowed to play right now
    // if timeBetweenSameSound is 0 we dont need this check
    if (_timeBetweenSameSound > 0.0f) {
      // The current time
      float currentTime = Time.unscaledTime;
      // Get the last time that sound played
      if(_dictionaryLastTimePlayed.TryGetValue(soundName, out float timeLastPlayed)) {
        // Too close to last time played
        if (currentTime < timeLastPlayed + _timeBetweenSameSound) {
          // Don't play Sound
          return null;

        // Set last time played to current time and continue
        } else {
          _dictionaryLastTimePlayed[soundName] = currentTime;
        }

      // First time playing that sound. Add to dictionary
      } else {
        _dictionaryLastTimePlayed.Add(soundName, currentTime);
      }
    }


    // Debug.Log("Playing Sound: " + soundName);
    List<AudioClip> clips;

    // Already saved from previous regex?
    if (audioClipsFromRegex.TryGetValue(soundName, out List<AudioClip> audioClips)) {
      clips = audioClips;

    // First Time Regex. Search and save in dictionary
    } else {
      clips = new List<AudioClip>();
      List<AudioClip> clipsForRegex = new List<AudioClip>();

      foreach (AudioClip clip in audioCollection.audioClips) {
        if (clip == null)
          continue;

        if (Regex.IsMatch(clip.name, soundName, RegexOptions.IgnoreCase)) {
          // Debug.Log("Clip Name: " + clip.name);
          clipsForRegex.Add(clip);
          clips.Add(clip);
        }
      }

      audioClipsFromRegex.Add(soundName, clipsForRegex);
    }

    if (clips.Count > 0) {
      return PlaySound(clips, volumeScale: volumeScale);
    } else {
      Debug.Log("No sound found for name: " + soundName);
      return null;
    }
  }

  public AudioSource PlaySound(AudioClip audioClip, Vector3 position, bool attached = false, GameObject obj = null, float volumeScale = 1.0f) {
    // position.z = -1f; Tried to get 3D sound to work but it was kinda buggy

    //AudioSource source;
    //if (obj == null && attached == false && audioSourceParent != null) {
    //  source = Instantiate(masterAudioSourcePrefab, position, Quaternion.identity, audioSourceParent.transform).GetComponent<AudioSource>();
    //} else {
    //  source = Instantiate(masterAudioSourcePrefab, position, Quaternion.identity).GetComponent<AudioSource>();
    //}

    GameObject soundObj = SoundPool.Fetch();
    if(soundObj == null) {
      Debug.LogWarning("Couldn't create Sound. SoundPool was full");
      return null;
    }

    soundObj.transform.position = position;

    AudioSource source;
    source = soundObj.GetComponent<AudioSource>();

    if (attached) {
      if (obj == null) {
        Debug.Log("Can't attach AudioSource if no GameObject is delivered");
      } else {
        source.gameObject.transform.parent = obj.transform;
      }
    }
    /*
    // Using Vector2 to ignore z
    float distance = Vector2.Distance(source.transform.position, GetAudioListenerPosition());
    // The volume depending on distance
    float volume = 1.0f;
    if(distance < minDistance) {
      volume = 1.0f;
    } else if(distance > maxDistance) {
      volume = 0.0f;
    } else { // 3 - 30 => 0 - 27
      volume = audioVolumeFalloff.Evaluate(1.0f - Mathf.Clamp((distance - minDistance) / (maxDistance - minDistance), 0, 1.0f));
    }

    // Apply multiplied by distance
    volumeScale *= volume;

    */
    // Debug.Log($"Playing AudioClip with length {audioClip.length}");

    float clipLength = audioClip.length;
    if(clipLength == 0.0f) {
      Debug.LogWarning("Playing Sound with clipLength of 0. Setting default time to 3 seconds. Might cause unwanted behaviour.");
      clipLength = 3.0f;
    }

    AudioSourcePoolObject poolObject = soundObj.GetComponent<AudioSourcePoolObject>();
    poolObject.Init(audioClip.length);

    source.volume = 1.0f;
    source.outputAudioMixerGroup = audioMixerGroupInterface;
    source.clip = audioClip;
    source.volume = volumeScale;
    source.Play();

    // source.PlayOneShot(audioClip, volumeScale);
    // Destroy(source.gameObject, source.clip.length);

    return source;
  }

  public AudioSource PlaySound(AudioClip[] audioClips, Vector3 position = default, bool attached = false, GameObject obj = null, float volumeScale = 1.0f) {
    if (audioClips == null || audioClips.Length == 0) {
      Debug.Log("Can't play random clip from array. Array doesn't exist or is empty!");
      return null;
    }

    return PlaySound(audioClips[UnityEngine.Random.Range(0, audioClips.Length)], position, attached, obj, volumeScale);
  }

  public AudioSource PlaySound(List<AudioClip> audioClips, Vector3 position = default, bool attached = false, GameObject obj = null, float volumeScale = 1.0f) {
    if (audioClips == null || audioClips.Count == 0) {
      Debug.Log("Can't play random clip from list. List doesn't exist or is empty!");
      return null;
    }

    return PlaySound(audioClips[UnityEngine.Random.Range(0, audioClips.Count)], position, attached, obj, volumeScale);
  }
  #endregion
}
