using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
  private GameObject _owner;

  public GameObject Owner { get => _owner; set => _owner = value; }

  private void FixedUpdate() {
    Vector2 pos = transform.position;
    if (pos.x < -10000 || pos.x > 10000 || pos.y < -2000 || pos.y > 2000)
      Destroy(gameObject);
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    if(collision.gameObject != Owner) {
      
    }

    Destroy(gameObject);
  }
}
