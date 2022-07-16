using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
  private GameObject _owner;

  public GameObject Owner { get => _owner; set => _owner = value; }


  private Countdown countdownDestroy;
  private void Awake() {
    countdownDestroy = new Countdown(5.0f);
  }

  private void FixedUpdate() {
    if(countdownDestroy.Check()) {
      Debug.Log("Projectile time was over. Destroying...");
      Destroy(gameObject);
    }
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    //if(collision.gameObject != Owner) {
      if(Owner == GameManager.Instance.Player) {
        if(collision.collider.tag == "Enemy") {
          Enemy enemy = collision.gameObject.GetComponent<Enemy>();
          enemy.Kill();
        }
      }
    //}

    Debug.Log("Collision of Projectile");
    Destroy(gameObject);
  }
}
