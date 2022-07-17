using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour {
  [HideInInspector]
  public GameObject target;
  public float PixelsPerUnit;

  private void Awake() {
    target = Camera.main.gameObject;
  }

  void Update() {
    transform.position = PixelPerfectClamp(target.transform.position, PixelsPerUnit);
  }

  private Vector3 PixelPerfectClamp(Vector3 moveVector, float pixelsPerUnit) {
    Vector2 vectorInPixels = new Vector3(Mathf.CeilToInt(moveVector.x * pixelsPerUnit), Mathf.CeilToInt(moveVector.y * pixelsPerUnit), Mathf.CeilToInt(moveVector.z * pixelsPerUnit));
    return vectorInPixels / pixelsPerUnit;
  }
}