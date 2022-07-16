using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceUnfoldPanel : MonoBehaviour {
  [SerializeField]
  private DiceRollMechanicSimple _mech;

  [SerializeField]
  private RectTransform _diceSideHighlightTransform;

  [SerializeField]
  private List<Vector2> _diceSideHighlightPositions;

  [SerializeField]
  private Vector2 positionDice1;
  [SerializeField]
  private Vector2 positionDice2;
  [SerializeField]
  private Vector2 positionDice3;
  [SerializeField]
  private Vector2 positionDice4;
  [SerializeField]
  private Vector2 positionDice5;
  [SerializeField]
  private Vector2 positionDice6;


  public void Start() {
    _mech.DiceFaceChange += _mech_DiceFaceChange;
  }

  private void _mech_DiceFaceChange(object sender, DiceFaceChangeEventArgs eventArgs) {
    int currentSide = eventArgs.newSide;

    UpdateSide(currentSide);
  }

  public void UpdateSide(int currentSide) {
    _diceSideHighlightTransform.anchoredPosition = _diceSideHighlightPositions[currentSide - 1];
  }
}
