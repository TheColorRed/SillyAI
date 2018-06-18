using UnityEngine;

namespace SillyAI {

  [AddComponentMenu("SillyAI/Health"), DisallowMultipleComponent]
  public class AIHealth : AI {

    public float maxHealth = 10f;
    public float startingHealth = 10f;

    private float _health = 0f;

    public float health {
      get { return _health; }
      private set { _health = value; }
    }

    public float percentage {
      get { return health / maxHealth; }
    }

    new void Awake() {
      base.Awake();
      _health = startingHealth;
    }

    /// <summary>
    /// Percentage of health to remove
    /// </summary>
    /// <param name="amount">A value from 0 to 1</param>
    public void RemovePercentage(float amount) {
      RemoveAmount(health * Mathf.Clamp01(amount));
    }

    /// <summary>
    /// Amount of health to remove
    /// </summary>
    /// <param name="amount">A value from 0 to infinity</param>
    public void RemoveAmount(float amount) {
      health -= amount;
    }
  }

}