using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SillyAI {

  [System.Serializable]
  public class HealthEvents : UnityEvent<float, float> { }

  [AddComponentMenu("SillyAI/AI Health"), DisallowMultipleComponent]
  public class AIHealth : AI {

    [Tooltip("The minimum health until the health has been considered depleted.")]
    public float minHealth = 0f;
    [Tooltip("The maximum amount of health.")]
    public float maxHealth = 10f;
    [Tooltip("The health that will be started off with.")]
    public float startingHealth = 10f;

    public HealthEvents OnHealthChanged;
    public UnityEvent OnHealthFilled;
    public UnityEvent OnHealthDepleted;

    private float _health = 0f;

    public float health {
      get { return _health; }
      private set {
        var current = _health;
        _health = Mathf.Clamp(value, minHealth, maxHealth);
        if (current != _health) {
          OnHealthChanged.Invoke(current, _health);
          if (value <= minHealth) {
            OnHealthDepleted.Invoke();
          }
          if (value >= maxHealth) {
            OnHealthFilled.Invoke();
          }
        }
      }
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
    public void RemoveHealthPercentage(float amount) {
      RemoveHealth(health * Mathf.Clamp01(amount));
    }

    /// <summary>
    /// Amount of health to remove
    /// </summary>
    /// <param name="amount">A value from 0 to infinity</param>
    public void RemoveHealth(float amount) {
      health -= amount;
    }

    public void AddHealthPercentage(float amount) {
      AddHealth(maxHealth * Mathf.Clamp01(amount));
    }

    public void AddHealth(float amount) {
      health += amount;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected() {
      var gui = new GUIStyle();
      gui.fontSize = 16;
      gui.fontStyle = FontStyle.Bold;
      gui.alignment = TextAnchor.MiddleCenter;
      Handles.Label(transform.position, health + "/" + maxHealth, gui);
    }
#endif
  }

}