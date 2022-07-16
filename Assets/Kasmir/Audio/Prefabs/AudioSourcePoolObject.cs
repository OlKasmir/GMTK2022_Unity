using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;

public class AudioSourcePoolObject : MonoBehaviour, Pooling.IPoolable {
  #region Events
  public event Pooling.IPoolable.FinishedEvent Finished;
  #endregion

  #region References
  [SerializeField]
  protected AudioSource _audioSource;
  #endregion

  #region Variables
  private Countdown countdown;
  private bool countdownRunning = false;
  #endregion

  #region Getter and Setter
  public AudioSource AudioSource { get {
      if (_audioSource == null)
        _audioSource = GetComponent<AudioSource>();

      return _audioSource;
    }
  }
  #endregion

  #region Unity Methods
  public void Awake() {
    countdown = new Countdown(0);
  }

  public void FixedUpdate() {
    if (countdownRunning) {
      if (countdown.Check()) {
        Finished?.Invoke(gameObject);
        countdownRunning = false;
      }
    }
  }
  #endregion

  #region Methods
  public void Init(float clipLength) {
    //StartCoroutine(DisableAfterTime(clipLength));

    countdown.Reset(clipLength);
    countdownRunning = true;
  }

  public void ResetPoolable() {
    // If a audio Clip is still playing, stop playing
    if(AudioSource.isPlaying)
      AudioSource.Stop();
  }
  #endregion

  #region Coroutines
  protected IEnumerator DisableAfterTime(float time) {
    yield return new WaitForSeconds(time);
    Finished?.Invoke(gameObject);
  }
  #endregion

  #region Operators
  #endregion

}