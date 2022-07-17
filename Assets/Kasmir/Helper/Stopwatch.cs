using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopwatch {
  [SerializeField]
  private float _startTime;

  public float Elapsed { get => Time.time - _startTime; set => _startTime = Time.time - value; }

  public Stopwatch() {
    _startTime = Time.time;
  }

  public void Reset() {
    _startTime = Time.time;
  }
}
