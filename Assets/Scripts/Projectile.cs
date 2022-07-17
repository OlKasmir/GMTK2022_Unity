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
    if (countdownDestroy.Check()) {
      Destroy(gameObject);
    }
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    if (Owner == GameManager.Instance.Player) {
      if (collision.collider.tag == "Enemy") {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        enemy.Kill();
      }
    } else {
      if (collision.collider.tag == "Player") {
        if(collision.gameObject.GetComponent<DiceRollMechanicSimple>().GetCurrentSide() == 6) {
          AudioManager.Instance.PlaySound("Block*");

        } else {
          DamagePlayer();
        }
      }

    }

    Destroy(gameObject);
  }

  private void DamagePlayer() {
    Debug.Log("Player damaged. TODO: Kill Player");
  }
}
