using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DiceRollMechanic))]
public class DiceRollAbilities : MonoBehaviour {
  [SerializeField, HideInInspector]
  private DiceRollMechanic _diceRollMechanic;
    Rigidbody2D rb;
    AudioSource audioSourceJetpack;
    [SerializeField, HideInInspector] float speed = 1000;
    [SerializeField] AudioClip duesenSound;

    public DiceRollMechanic DiceRollMechanic { get => _diceRollMechanic; set => _diceRollMechanic = value; }

  private void Awake() {
    _diceRollMechanic = GetComponent<DiceRollMechanic>();
    DiceRollMechanic.DiceFaceChange += DiceRollMechanic_DiceFaceChange;
        rb = GetComponent<Rigidbody2D>();
        audioSourceJetpack = GetComponent<AudioSource>();
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
    if(Input.GetKey(KeyCode.Space)) {
      UsingAbility(DiceRollMechanic.GetCurrentSide());
        }
        else
        {
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
    if(diceSide == 1 || diceSide == 6) {
      Jetpack();
    }
  }

  private void Jetpack() {
        rb.AddRelativeForce(Vector2.up * Time.deltaTime * speed);
        if (!audioSourceJetpack.isPlaying)
        {
            audioSourceJetpack.PlayOneShot(duesenSound);
        }
    }

  private void Dash() {
    Debug.Log("Dash");
  }

  private void Fire() {
    Debug.Log("Fire");
  }
  
}
