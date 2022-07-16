using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRollMechanicSimple : MonoBehaviour {
  public delegate void DiceFaceChangeEventHandler(object sender, DiceFaceChangeEventArgs eventArgs);
  public event DiceFaceChangeEventHandler DiceFaceChange;

  [SerializeField, HideInInspector]
  private SpriteRenderer _spriteRenderer;

  [SerializeField, Tooltip("All 6 sprites of the dice sides ordered from 1 to 6")]
  private List<Sprite> _diceSprites;

  [SerializeField, HideInInspector]
  private PlayerMovement movement;

  [SerializeField, HideInInspector]
  private int _currentSide = 1;

  public SpriteRenderer SpriteRenderer { get => _spriteRenderer; set => _spriteRenderer = value; }
  public List<Sprite> DiceSprites { get => _diceSprites; set => _diceSprites = value; }

  private void Awake() {
    SpriteRenderer = GetComponent<SpriteRenderer>();
    movement = GetComponent<PlayerMovement>();
  }

  void Update() {
    UpdateInput();
  }

  public void UpdateInput() {
    bool sideChange = false;
    int currentSide = GetCurrentSide();

    // Up
    if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.UpArrow)) {
      if (_currentSide == 4) {
        _currentSide = 1;
        sideChange = true;
      } else if (_currentSide == 1) {
        _currentSide = 3;
        sideChange = true;
      } else if (_currentSide == 3) {
        _currentSide = 6;
        sideChange = true;
      }
    }

    // Right
    if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.RightArrow)) {
      if (_currentSide == 2) {
        _currentSide = 1;
        sideChange = true;

      } else if (_currentSide == 1) {
        _currentSide = 5;
        sideChange = true;
      }
    }

    // Down
    if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.DownArrow)) {
      if (_currentSide == 6) {
        _currentSide = 3;
        sideChange = true;

      } else if (_currentSide == 3) {
        _currentSide = 1;
        sideChange = true;

      } else if (_currentSide == 1) {
        _currentSide = 4;
        sideChange = true;
      }
    }

    // Left
    if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.LeftArrow)) {
      if (_currentSide == 5) {
        _currentSide = 1;
        sideChange = true;

      } else if (_currentSide == 1) {
        _currentSide = 2;
        sideChange = true;
      }
    }

    // Update graphics only when necessary (On Key Press)
    if (sideChange) {
      UpdateGraphics();

      DiceFaceChange?.Invoke(this, new DiceFaceChangeEventArgs() { previousSide = currentSide, newSide = GetCurrentSide() });
    }
  }

  private void UpdateGraphics() {
    SpriteRenderer.sprite = DiceSprites[GetCurrentSide() - 1];
  }

  /// <summary>
  /// Returns the side of the dice currently facing the camera
  /// </summary>
  public int GetCurrentSide() {
    return _currentSide;
  }

}
