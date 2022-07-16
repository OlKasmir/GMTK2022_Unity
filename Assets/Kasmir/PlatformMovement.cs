using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlatformMovement : MonoBehaviour {
  [SerializeField, HideInInspector]
  private Vector2 _startPoint;
  [SerializeField]
  private Vector2 _targetPoint;
  [SerializeField]
  private float _movementSpeed;
  [SerializeField]
  private float _timeStayAtTarget;
  [SerializeField]
  private float _targetReachedThreshold = 0.1f;
  
  private bool _isMovingTowardsTarget = true;
  private Countdown _waitCountdown;

  private GameObject _playerSticking;

#if (UNITY_EDITOR)
  public void OnDrawGizmos() {
    if(!EditorApplication.isPlayingOrWillChangePlaymode) {
      Gizmos.color = Color.cyan;
      Gizmos.DrawLine(transform.position, _targetPoint);

      BoxCollider2D collider = GetComponent<BoxCollider2D>();
      if(collider) {
        Gizmos.DrawWireCube(_targetPoint, collider.size * transform.localScale);
      }
    }
  }
#endif

  private void OnCollisionEnter2D(Collision2D collision) {
    if(collision.gameObject == GameManager.Instance.Player) {
      _playerSticking = GameManager.Instance.Player;
    }
  }

  private void OnCollisionExit2D(Collision2D collision) {
    if (collision.gameObject == _playerSticking) {
      _playerSticking = null;
    }
  }

  private void OnValidate() {
    if(_waitCountdown != null)
      _waitCountdown.Reset(_timeStayAtTarget);
  }

  private void Awake() {
    _waitCountdown = new Countdown(_timeStayAtTarget);
    _startPoint = transform.position;
  }

  // Update is called once per frame
  void FixedUpdate() {
    if (!_waitCountdown.Check())
      return;

    // Vector2 dir = (_targetPointRelative - (Vector2)transform.position).normalized;
    Vector2 moveTarget = _targetPoint;
    if (!_isMovingTowardsTarget)
      moveTarget = _startPoint;

    Vector2 oldPosition = transform.position;
    transform.position = Vector2.MoveTowards(transform.position, moveTarget, _movementSpeed * Time.fixedDeltaTime);
    Vector2 moveDelta = (Vector2) transform.position - oldPosition;

    if(_playerSticking != null) {
      _playerSticking.transform.position = (Vector2)_playerSticking.transform.position + moveDelta;
    }

    if (Vector2.Distance(transform.position, moveTarget) <= _targetReachedThreshold) {
      _isMovingTowardsTarget = !_isMovingTowardsTarget;
      _waitCountdown.Reset();
    }
  }
}
