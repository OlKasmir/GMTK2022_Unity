using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DiceRoll : MonoBehaviour {
  [SerializeField, HideInInspector]
  private SpriteRenderer _spriteRenderer;

  [SerializeField, Tooltip("All 6 sprites of the dice sides ordered from 1 to 6")]
  private List<Sprite> _diceSprites;


  [SerializeField, Tooltip("Assign to an unused transform to keep 3D Rotation information")]
  private Transform _orentiationHelper;

  public SpriteRenderer SpriteRenderer { get => _spriteRenderer; set => _spriteRenderer = value; }
  public List<Sprite> DiceSprites { get => _diceSprites; set => _diceSprites = value; }
  private Transform OrentiationHelper { get => _orentiationHelper; set => _orentiationHelper = value; }


  private void Awake() {
    SpriteRenderer = GetComponent<SpriteRenderer>();
  }

  void Update() {
    UpdateInput();
  }

  private void UpdateInput() {
    bool keyPress = false;

    // Up
    if(Input.GetKeyDown(KeyCode.W)) {
      OrentiationHelper.RotateAround(OrentiationHelper.position, Vector3.right, 90.0f);
      keyPress = true;
    }

    // Right
    if (Input.GetKeyDown(KeyCode.D)) {
      OrentiationHelper.RotateAround(OrentiationHelper.position, Vector3.down, 90.0f);
      keyPress = true;
    }

    // Down
    if (Input.GetKeyDown(KeyCode.S)) {
      OrentiationHelper.RotateAround(OrentiationHelper.position, Vector3.left, 90.0f);
      keyPress = true;
    }

    // Left
    if (Input.GetKeyDown(KeyCode.A)) {
      OrentiationHelper.RotateAround(OrentiationHelper.position, Vector3.up, 90.0f);
      keyPress = true;
    }

    // Update graphics only when necessary (On Key Press)
    if(keyPress) {
      UpdateGraphics();
    }
  }

  private void UpdateGraphics() {
    SpriteRenderer.sprite = DiceSprites[(int) GetFaceToward(OrentiationHelper, new Vector3(0, 0, -5)) - 1];
  }

  /// <summary>
  /// https://gamedev.stackexchange.com/questions/187075/how-to-tell-which-face-of-a-cube-is-most-visible-to-the-camera
  /// </summary>
  public enum CubeFace {
    Left   = 2, // 2
    Bottom = 6, // 6
    Back   = 4, // 4
    Right  = 5, // 5
    Top    = 1, // 1
    Front  = 3  // 3
  }

  public static CubeFace GetFaceToward(Transform cube, Vector3 observerPosition) {
    var toObserver = cube.InverseTransformPoint(observerPosition);

    var absolute = new Vector3(
                      Mathf.Abs(toObserver.x),
                      Mathf.Abs(toObserver.y),
                      Mathf.Abs(toObserver.z)
                   );

    if (absolute.x >= absolute.y) {
      if (absolute.x >= absolute.z) {
        return toObserver.x > 0 ? CubeFace.Right : CubeFace.Left;
      } else {
        return toObserver.z > 0 ? CubeFace.Front : CubeFace.Back;
      }
    } else if (absolute.y >= absolute.z) {
      return toObserver.y > 0 ? CubeFace.Top : CubeFace.Bottom;
    } else {
      return toObserver.z > 0 ? CubeFace.Front : CubeFace.Back;
    }
  }
}
