using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windmill : MonoBehaviour {
  [SerializeField]
  private float sqrt = 3;
  [SerializeField]
  private float _windStrengthMultiplier = 10.0f;
  [SerializeField]
  private float _timeMultipler = 0.001f;
  [SerializeField]
  private float _currentWindStrength;

  private void Awake() {
    sqrt = Mathf.Sqrt(sqrt);
  }

  // Update is called once per frame
  void Update() {
    float time = Time.time * _timeMultipler;
    _currentWindStrength = Mathf.Sin(time) + Mathf.Cos(sqrt * time);

    //Quaternion rot = Quaternion.Euler(new Vector3(0.0f, 0.0f, _currentWindStrength * _windStrengthMultiplier));
    //transform.rotation = transform.rotation * rot;

    transform.Rotate(new Vector3(0.0f, 0.0f, _currentWindStrength * _windStrengthMultiplier));
  }
}
