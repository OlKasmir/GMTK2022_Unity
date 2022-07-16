using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelChangeEventArgs : EventArgs {
  public float currentFuel;
}

[RequireComponent(typeof(DiceRollMechanicSimple))]
public class DiceRollAbilities : MonoBehaviour {
  public delegate void FuelChangeEventHandler(object sender, FuelChangeEventArgs eventArgs);
  public event FuelChangeEventHandler FuelChange;

  [SerializeField, HideInInspector]
  private DiceRollMechanicSimple _diceRollMechanic;

  PlayerMovement movement;
  Rigidbody2D rb;


  /// <summary>
  /// JETPACK
  /// </summary>
  AudioSource audioSourceJetpack;
  [Header("Jetpack")]
  [SerializeField]
  float speed = 1000;
  [SerializeField]
  AudioClip duesenSound;
  [SerializeField]
  private float _fuelMax = 100;
  [SerializeField]
  private float _fuelTime = 2.0f;
  [SerializeField]
  private float _refuelDelay = 2.0f;
  [SerializeField]
  private float _refuelTime = 0.5f;
  private Countdown _refuelCountdown;
  // private bool refueling = false;
  private float _currentFuel;


  /// <summary>
  /// DASH
  /// </summary>
  [SerializeField, HideInInspector]
  private Cooldown _dashCooldown;
  [Header("Dash")]
  [SerializeField]
  private float _dashCooldownTime = 1.0f;
  [SerializeField]
  private float dashForce = 25;
  [SerializeField, Tooltip("Time until the player can control the movement again")]
  private float dashTime = 0.5f;



  /// <summary>
  /// FIRE
  /// </summary>
  [SerializeField, Header("Shoot")]
  private GameObject _projectilePrefab;
  [SerializeField]
  private float _projectileForce;
  [SerializeField]
  private float _shootCooldownTime;
  private Cooldown _shootCooldown;



  public DiceRollMechanicSimple DiceRollMechanic { get => _diceRollMechanic; set => _diceRollMechanic = value; }
  public float FuelMax { get => _fuelMax; set => _fuelMax = value; }
  public float CurrentFuel { get => _currentFuel; set => _currentFuel = value; }
  public Countdown RefuelCountdown { get => _refuelCountdown; set => _refuelCountdown = value; }

  private void Awake() {
    _diceRollMechanic = GetComponent<DiceRollMechanicSimple>();
    DiceRollMechanic.DiceFaceChange += DiceRollMechanic_DiceFaceChange;
    rb = GetComponent<Rigidbody2D>();
    audioSourceJetpack = GetComponent<AudioSource>();
    movement = GetComponent<PlayerMovement>();
    _dashCooldown = new Cooldown(_dashCooldownTime);
    RefuelCountdown = new Countdown(_refuelDelay);
    CurrentFuel = FuelMax;
    _shootCooldown = new Cooldown(_shootCooldownTime);
  }

  private void OnValidate() {
    if (_dashCooldown != null)
      _dashCooldown.Change(_dashCooldownTime);

    if (_shootCooldown != null)
      _shootCooldown.Change(_shootCooldownTime);

    if (_refuelCountdown != null)
      _refuelCountdown.Reset(_refuelDelay);
  }

  private void Update() {
    UpdateInput();

    UpdateFuel();
  }

  public void UpdateInput() {
    // Space Bar: KeyDown
    if (Input.GetKeyDown(KeyCode.Space)) {
      UseAbilityOnce(DiceRollMechanic.GetCurrentSide());
    }

    // Space Bar: Pressed
    if (Input.GetKey(KeyCode.Space)) {
      UsingAbility(DiceRollMechanic.GetCurrentSide());
    } else {
      audioSourceJetpack.Stop();
    }
  }

  public void UpdateFuel() {
    if (RefuelCountdown.Check()) {
      CurrentFuel = Mathf.Min(FuelMax, CurrentFuel + FuelMax * (Time.deltaTime / _refuelTime));
      FuelChange?.Invoke(this, new FuelChangeEventArgs() { currentFuel = CurrentFuel });
    }
  }

  /// <summary>
  /// Add all functionality that happens when the dice face changes here
  /// </summary>
  private void DiceRollMechanic_DiceFaceChange(object sender, DiceFaceChangeEventArgs eventArgs) {
    // The previous side of the dice facing the camera
    int previousSide = eventArgs.previousSide;
    // The side of the dice facing the camera now
    int newSide = eventArgs.newSide;

    if (previousSide == 1) {
      audioSourceJetpack.Stop();
    }

    if (previousSide == 2) {
      StopStick();
    }

    if (newSide == 2) {
      Stick();
    }

    AudioManager.Instance.PlaySound("SwitchModes");
  }

  /// <summary>
  /// Add all functionality that happens when the player is pressing space here (Only on button down)
  /// </summary>
  /// <param name="diceSide">The side of the dice currently facing the camera</param>
  private void UseAbilityOnce(int diceSide) {
    if (diceSide == 5) {
      Dash();
    }

    if (diceSide == 3) {
      Fire();
    }
  }

  /// <summary>
  /// Add all functionality that happens while the player is holding space here
  /// </summary>
  /// <param name="diceSide">The side of the dice currently facing the camera</param>
  private void UsingAbility(int diceSide) {
    if (diceSide == 1) {
      Jetpack();
    }
  }

  public float GetDirection() {
    return transform.localScale.x < 0 ? -1.0f : 1.0f; // rb.velocity.x < 0 ? -1.0f : 1.0f;
  }

  private void Jetpack() {
    if (CurrentFuel < 0) {
      if (audioSourceJetpack.isPlaying) {
        audioSourceJetpack.Stop();
      }

      return;
    }

    CurrentFuel -= FuelMax * (Time.deltaTime / _fuelTime);
    FuelChange?.Invoke(this, new FuelChangeEventArgs() { currentFuel = CurrentFuel });
    RefuelCountdown.Reset();

    rb.AddRelativeForce(Vector2.up * Time.deltaTime * speed);
    if (!audioSourceJetpack.isPlaying) {
      audioSourceJetpack = AudioManager.Instance.PlaySound(duesenSound, transform.position); //audioSourceJetpack.PlayOneShot(duesenSound);
    }
  }

  private void Dash() {
    if (!_dashCooldown.Check())
      return;

    Vector2 dir = new Vector2(GetDirection(), 0.0f);
    rb.AddForce(dir * dashForce, ForceMode2D.Impulse);
    movement.ApplyMovementBlockTime(dashTime);

    AudioManager.Instance.PlaySound("Dash2");
  }

  private void Fire() {
    if (!_shootCooldown.Check()) {
      return;
    }

    GameObject go = Instantiate(_projectilePrefab, transform.position + new Vector3(GetDirection() * 1.0f, 0.0f), Quaternion.identity);
    Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
    rb.AddForce(new Vector2(_projectileForce * GetDirection(), 0.0f), ForceMode2D.Impulse);

    Physics2D.IgnoreCollision(go.GetComponent<Collider2D>(), GetComponent<Collider2D>());

    Projectile p = go.GetComponent<Projectile>();
    if (p == null) {
      Debug.LogWarning("Can't shoot projectile since the specified Projectile prefab doesn't have a projectile component on it");
      return;
    }
    p.Owner = gameObject;

    AudioManager.Instance.PlaySound("Gun1");
  }


  private GameObject _currentPlatform;
  private float _previousGravity;
  private bool _sticking = false;

  private void OnCollisionEnter2D(Collision2D collision) {
    if (collision.collider.tag == "Platform") {
      _currentPlatform = collision.gameObject;
    }

    if (DiceRollMechanic.GetCurrentSide() == 2) {
      Stick();
    }
  }

  private void OnCollisionExit2D(Collision2D collision) {
    if (collision.gameObject == _currentPlatform) {
      _currentPlatform = null;
    }
  }

  private void Stick() {
    if (_sticking)
      return;



    if (_currentPlatform == null)
      return;

    _previousGravity = rb.gravityScale;
    rb.gravityScale = 0.0f;
    movement.ApplyMovementBlockTime(999.0f);
    _sticking = true;
  }

  private void StopStick() {
    if (!_sticking)
      return;

    rb.gravityScale = _previousGravity;
    movement.ApplyMovementBlockTime(0.0f);

    _sticking = false;
  }
}

