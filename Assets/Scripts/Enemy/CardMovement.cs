using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMovement : MonoBehaviour {
  public float moveSpeed = 5;
  public float height = 1;
  private float baseY;

  // Start is called before the first frame update
  void Start() {
    baseY = transform.position.y;
  }

  // Update is called once per frame
  void Update() {
    float y = Mathf.Sin(Time.time * moveSpeed) * height;
    transform.SetPositionY(baseY + y);
  }
}
