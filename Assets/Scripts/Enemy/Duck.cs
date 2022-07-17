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
  }

  // Update is called once per frame
  void FixedUpdate() {
    if(!_triggered && enemy.IsPlayerInSight()) {
      enemy.StartAttackAnimation();
      StartCoroutine(DuckSounds());
      _triggered = true;
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

    if (enemy.GetPlayerDistance() <= 2) {
      Debug.Log("TODO: Player should have taken damage OR blocked when on side 6");
    }

    Destroy(gameObject);
  }
}
