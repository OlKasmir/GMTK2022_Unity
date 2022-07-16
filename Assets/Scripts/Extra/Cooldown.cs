using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a Timer which can be used to check if intervals have been reached. 
/// Use Check() for that (Check also resets the timer when interval was reached)
/// </summary>
[System.Serializable]
public class Cooldown {
  #region Variables
  [SerializeField]
  protected float _startTime;
  [SerializeField]
  protected float _cooldown;
  #endregion

  #region Getter and Setter
  public bool IsReady { get => Time.time > _startTime + _cooldown; }
  public bool OnCooldown { get => !IsReady; }
  public float CooldownTime { get => _cooldown; set => _cooldown = value; }
  public float CooldownLeft { get => Mathf.Max(0, _cooldown - (Time.time - _startTime)); }
  #endregion

  public Cooldown(float cooldown) {
    _startTime = Time.time - cooldown;
    _cooldown = cooldown;
  }

  #region Methods
  /// <summary>
  /// Returns true if the cooldown is over and starts the cooldown if it was ready
  /// </summary>
  /// <returns>true if countdown is over</returns>
  public bool Check() {
    float time = Time.time;
    if (time > _startTime + _cooldown) {
      Trigger();
      return true;
    }

    return false;
  }

  /// <summary>
  /// Triggers the cooldown
  /// </summary>
  public void Trigger() {
    _startTime = Time.time;
  }

  /// <summary>
  /// Resets the cooldown
  /// </summary>
  public void Reset() {
    _startTime = Time.time - _cooldown;
  }

  /// <summary>
  /// Changes the cooldown
  /// </summary>
  public void Change(float cooldown) {
    _cooldown = cooldown;
  }
  #endregion
}