using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceFuelPanel : MonoBehaviour {
  [SerializeField]
  private DiceRollAbilities _abilities;
  [SerializeField]
  private Image _fuelBarImage;
  [SerializeField]
  private Image _fuelRechargeCountdownImage;

  // Start is called before the first frame update
  void Start() {
    _abilities.FuelChange += _abilities_FuelChange;
    UpdateFuelBar(_abilities.CurrentFuel);
  }

  private void Update() {
    _fuelRechargeCountdownImage.fillAmount = Mathf.Clamp(_abilities.RefuelCountdown.TimeLeft / _abilities.RefuelCountdown.Duration, 0.0f, 1.0f);
  }

  private void _abilities_FuelChange(object sender, FuelChangeEventArgs eventArgs) {
    UpdateFuelBar(eventArgs.currentFuel);
  }

  public void UpdateFuelBar(float currentFuel) {
    _fuelBarImage.fillAmount = currentFuel / _abilities.FuelMax;
  }
}
