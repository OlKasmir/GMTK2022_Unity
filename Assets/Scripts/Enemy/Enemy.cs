using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
  [SerializeField, HideInInspector]
  private Rigidbody2D rb;
  [SerializeField, HideInInspector]
  private SpriteRenderer sr;

  [SerializeField, Header("Graphics")]
  private bool flipScale;

  [SerializeField, Header("Movement")]
  private bool _moveTowardsPlayer = false;
  [SerializeField]
  private float _movementSpeed = 5.0f;
  [SerializeField]
  private float _acceleration = 5.0f;
  [SerializeField]
  private float _minDistance = 3.0f;

  [SerializeField, Header("Shoot")]
  private GameObject _projectilePrefab;
  [SerializeField]
  private float _projectileForce;
  [SerializeField]
  private float _shootCooldownTime;
  private Cooldown _shootCooldown;
  [SerializeField]
  private float _distanceFromEntityWhenSpawning = 1.0f;
  [SerializeField]
  private LayerMask layerMaskRaycast;
  [SerializeField]
  private float _maxViewDistance = 10.0f;
  [SerializeField]
  private Vector3 _shootOffset;
  [SerializeField]
  private string _attackSound;

  [SerializeField, Header("Animation")]
  private SpriteAnimatorSimple _attackAnim;

  public GameObject Player => GameManager.Instance.Player;


  private void Awake() {
    rb = GetComponent<Rigidbody2D>();
    sr = GetComponent<SpriteRenderer>();
    _shootCooldown = new Cooldown(_shootCooldownTime);

    if(_attackAnim != null) {
      _attackAnim.AnimatorEnd += _attackAnim_AnimatorEnd;
    }
  }

  private void _attackAnim_AnimatorEnd() {
    _attackAnim.Hide();
    sr.enabled = true;
  }

  private void StartAttackAnimation() {
    _attackAnim.Show();
    sr.enabled = false;
  }

  public void FixedUpdate() {
    TryMoveTowardsPlayer();
    TryFireProjectile();
  }

  public bool IsPlayerInSight() {
    float distance = _maxViewDistance; // Vector3.Distance(target.transform.position, transform.position);
    //Cast a ray in the direction specified in the inspector.
    RaycastHit2D hit = Physics2D.Raycast(transform.position + _shootOffset, Player.transform.position - transform.position, distance, layerMaskRaycast);

    Debug.DrawLine(transform.position + _shootOffset, Player.transform.position);

    //If something was hit.
    if (hit.collider == null)
      return false;

    if (hit.rigidbody == null)
      return false;

    if (hit.rigidbody.gameObject != Player)
      return false;

    return true;
  }

  public void TryMoveTowardsPlayer() {
    if (IsPlayerInSight()) {
      if (Vector2.Distance(Player.transform.position, transform.position) <= _minDistance)
        return;
      
      float dirX = Player.transform.position.x < transform.position.x ? -1.0f : 1.0f;
      Vector3 scale = transform.localScale;
      scale.x = flipScale ? dirX * -1 : dirX;
      transform.localScale = scale;

      if (!_moveTowardsPlayer)
        return;

      rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(dirX * _movementSpeed, rb.velocity.y), Time.deltaTime * _acceleration);
    }
  }

  public void TryFireProjectile() {
    if (!_shootCooldown.Check()) {
      return;
    }

    if (!IsPlayerInSight())
      return;

    FireProjectile();
  }

  public void FireProjectile() {
    if (!string.IsNullOrEmpty(_attackSound)) {
      AudioManager.Instance.PlaySound(_attackSound);
    }

    StartAttackAnimation();

    Vector3 dir = (Player.transform.position - transform.position).normalized;
    Vector3 projectileSpawnPos = transform.position + _shootOffset + dir * _distanceFromEntityWhenSpawning;

    GameObject go = Instantiate(_projectilePrefab, projectileSpawnPos, Quaternion.identity);
    Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
    rb.AddForce(dir * _projectileForce, ForceMode2D.Impulse);

    Physics2D.IgnoreCollision(go.GetComponent<Collider2D>(), GetComponent<Collider2D>());

    rb.gravityScale = 0.0f;

    Projectile p = go.GetComponent<Projectile>();
    if (p == null) {
      Debug.LogWarning("Can't shoot projectile since the specified Projectile prefab doesn't have a projectile component on it");
      return;
    }
  }
}
