using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPoint : MonoBehaviour {
  private void OnTriggerEnter2D(Collider2D collision) {
    if(collision.gameObject == GameManager.Instance.Player) {
      GameManager.Instance.FinishScene();
    }
  }
}
