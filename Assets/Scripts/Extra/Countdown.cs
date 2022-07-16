using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a Timer which can be used to check if intervals have been reached. 
/// Use Check() for that (Check also resets the timer when interval was reached)
/// </summary>
[System.Serializable]
public class Countdown {
  #region Variables
  [SerializeField]
  protected float _startTime;
  [SerializeField]
  protected float _duration;
  #endregion

  #region Getter and Setter
  public float StartTime { get => _startTime; }
  public float Duration { get => _duration; set => _duration = value; }
  public float TimeLeft { get => Mathf.Max(0, _duration - (Time.time - _startTime)); }
  #endregion

  public Countdown(float duration) {
    _startTime = Time.time;
    _duration = duration;
  }

  #region Methods
  /// <summary>
  /// Returns true if the countdown is over
  /// </summary>
  /// <returns>true if countdown is over</returns>
  public bool Check() {
    float time = Time.time;
    if (time > _startTime + _duration) {
      return true;
    }

    return false;
  }

  /// <summary>
  /// Resets the countdown
  /// </summary>
  public void Reset() {
    _startTime = Time.time;
  }

  /// <summary>
  /// Resets the countdown and sets a new duration
  /// </summary>
  public void Reset(float duration) {
    _startTime = Time.time;
    _duration = duration;
  }
  #endregion
}