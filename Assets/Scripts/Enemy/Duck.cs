using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Duck : MonoBehaviour {
  private Enemy enemy;
  public float explosionDistance = 1.5f;
  public float explosionFuseTime = 0.5f;

  public GameObject _prefabExplosion;
  public string explosionSound;
  public string fuseSound;

  private bool _exploding = false;
  private bool _triggered = false;

  // Start is called before the first frame update
  void Awake() {
    enemy = GetComponent<Enemy>();
    enemy.AutoEndAnim = false;
    enemy.OnDeath += Enemy_OnDeath;
  }

  private void Enemy_OnDeath() {
    Explode();
  }

  // Update is called once per frame
  void FixedUpdate() {
    if(!_triggered && enemy.IsPlayerInSight()) {
      enemy.StartAttackAnimation();
      StartCoroutine(DuckSounds());
      _triggered = true;

    } else if(_triggered && !enemy.IsPlayerInSight()) {

      StopAllCoroutines();
      _triggered = false;
      _exploding = false;
    }

    if(enemy.GetPlayerDistance() <= explosionDistance) {
      if(!_exploding) {
        StartCoroutine(ExplodeTimer(explosionFuseTime));
      }
    }
  }

  public IEnumerator DuckSounds() {
    while (true) {
      if (!string.IsNullOrEmpty(enemy.AttackSound)) {
        AudioManager.Instance.PlaySound(enemy.AttackSound);
      }

      yield return new WaitForSeconds(0.5f);
    }
  }

  [HideInInspector]
  public AudioSource fuse;

  public IEnumerator ExplodeTimer(float fuseTime) {
    _exploding = true;
    // enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

    fuse = AudioManager.Instance.PlaySound(fuseSound);

    yield return new WaitForSeconds(fuseTime);

    Explode();
  }

  public void Explode() {
    if (_prefabExplosion != null)
      Instantiate(_prefabExplosion, transform.position, _prefabExplosion.transform.rotation);
    AudioManager.Instance.PlaySound(explosionSound);
    if(fuse != null)
      fuse.Stop();

    if (enemy.GetPlayerDistance() <= 2 && GameManager.Instance.Player.GetComponent<DiceRollMechanicSimple>().GetCurrentSide() != 6) {
      GameManager.Instance.GameOver();
    }

    Destroy(gameObject);
  }
}
