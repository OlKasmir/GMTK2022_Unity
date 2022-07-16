using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DiceRollMechanicSimple))]
public class DiceRollAbilities : MonoBehaviour {
  [SerializeField, HideInInspector]
  private DiceRollMechanicSimple _diceRollMechanic;

  PlayerMovement movement;
  Rigidbody2D rb;
  AudioSource audioSourceJetpack;
  [SerializeField] float speed = 1000;
  [SerializeField] AudioClip duesenSound;

  private Cooldown _dashCooldown;
  [SerializeField]
  private float _dashCooldownTime = 1.0f;
  [SerializeField]
  private float dashForce = 25;
  [SerializeField, Tooltip("Time until the player can control the movement again")]
  private float dashTime = 0.5f;

  public DiceRollMechanicSimple DiceRollMechanic { get => _diceRollMechanic; set => _diceRollMechanic = value; }

  private void Awake() {
    _diceRollMechanic = GetComponent<DiceRollMechanicSimple>();
    DiceRollMechanic.DiceFaceChange += DiceRollMechanic_DiceFaceChange;
    rb = GetComponent<Rigidbody2D>();
    audioSourceJetpack = GetComponent<AudioSource>();
    movement = GetComponent<PlayerMovement>();
    _dashCooldown = new Cooldown(_dashCooldownTime);
  }

  private void OnValidate() {
    if(_dashCooldown != null)
      _dashCooldown.Change(_dashCooldownTime);
  }

  private void Update() {
    UpdateInput();
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

  /// <summary>
  /// Add all functionality that happens when the dice face changes here
  /// </summary>
  private void DiceRollMechanic_DiceFaceChange(object sender, DiceFaceChangeEventArgs eventArgs) {
    // The previous side of the dice facing the camera
    int previousSide = eventArgs.previousSide;
    // The side of the dice facing the camera now
    int newSide = eventArgs.newSide;

    if (previousSide == 1 || previousSide == 6) {
      audioSourceJetpack.Stop();
    }
  }

  /// <summary>
  /// Add all functionality that happens when the player is pressing space here (Only on button down)
  /// </summary>
  /// <param name="diceSide">The side of the dice currently facing the camera</param>
  private void UseAbilityOnce(int diceSide) {
    if (diceSide == 2 || diceSide == 5) {
      Dash();
    }

    if (diceSide == 3 || diceSide == 4) {
      Fire();
    }
  }

  /// <summary>
  /// Add all functionality that happens while the player is holding space here
  /// </summary>
  /// <param name="diceSide">The side of the dice currently facing the camera</param>
  private void UsingAbility(int diceSide) {
    if (diceSide == 1 || diceSide == 6) {
      Jetpack();
    }
  }

  private void Jetpack() {
    rb.AddRelativeForce(Vector2.up * Time.deltaTime * speed);
    if (!audioSourceJetpack.isPlaying) {
      audioSourceJetpack.PlayOneShot(duesenSound);
    }
  }

  private void Dash() {
    if (!_dashCooldown.Check())
      return;

    float xForce = 0.0f;
    if (rb.velocity.x < 0) xForce = -1.0f;
    if (rb.velocity.x > 0) xForce = 1.0f;

    Vector2 dir = new Vector2(xForce, 0.0f);
    rb.AddForce(dir * dashForce, ForceMode2D.Impulse);
    movement.ApplyMovementBlockTime(dashTime);
  }

  private void Fire() {
    Debug.Log("Fire");
  }

}
