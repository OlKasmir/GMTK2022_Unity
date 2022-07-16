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
