using UnityEngine;
using UnityEngine.AI;
using System;
using System.Linq;
using System.Collections.Generic;
namespace SillyAI {

  public enum UnitType { None, Ground, Air, Water }

  [AddComponentMenu("SillyAI/Brain"), DisallowMultipleComponent, RequireComponent(typeof(NavMeshAgent))]
  public class AIBrain : MonoBehaviour {

    [Header("Character Distances")]
    [Tooltip("The maximum distance another character is detectable to the current character.")]
    public Collider viewDistance;

    [Header("Character Attack Information")]
    [Tooltip("The type of unit.")]
    public UnitType unitType;

    [Header("Character Destinations")]
    [Tooltip("The main destination for the character.")]
    public Destination destination;
    [Tooltip("Sub destinations for where a character needs to perform a sub task before returning to its primary destination.")]
    public List<Destination> subDestinations = new List<Destination>();

    private NavMeshAgent navMeshAgent;
    private NavMeshPath path;
    private Health health;
    private List<AIBrain> others = new List<AIBrain>();
    private float _dob = 0f;

    public float Age {
      get { return Time.time - _dob; }
    }

    void Awake() {
      if (viewDistance) viewDistance.isTrigger = true;
      navMeshAgent = GetComponent<NavMeshAgent>();
      health = GetComponent<Health>();
      path = new NavMeshPath();
      _dob = Time.time;
      var hasDestination = GetComponent<Destination>();
      if (!hasDestination) gameObject.AddComponent<Destination>();
    }

    void Start() {
      if (destination != null) {
        navMeshAgent.CalculatePath(destination.position, path);
        navMeshAgent.SetPath(path);
      }
    }

    /// <summary>
    /// Gets the closest character within the view distance
    /// </summary>
    /// <returns></returns>
    public AIBrain Closest() {
      return others.Aggregate((curr, item) => {
        float currDistance = Vector3.Distance(transform.position, curr.transform.position);
        float itemDistance = Vector3.Distance(transform.position, item.transform.position);
        return itemDistance < currDistance ? item : curr;
      });
    }

    /// <summary>
    /// Gets the furthest character within the view distance
    /// </summary>
    /// <returns></returns>
    public AIBrain Furthest() {
      return others.Aggregate((curr, item) => {
        float currDistance = Vector3.Distance(transform.position, curr.transform.position);
        float itemDistance = Vector3.Distance(transform.position, item.transform.position);
        return itemDistance > currDistance ? item : curr;
      });
    }

    /// <summary>
    /// Gets the weakest character within the view distance.
    /// **Note:** The other character must have the "Health" component to qualify.
    /// </summary>
    /// <returns></returns>
    public AIBrain Weakest() {
      return others.Aggregate((curr, item) => {
        Attack currAttack = curr.GetComponent<Attack>();
        Attack itemAttack = item.GetComponent<Attack>();
        if (!currAttack && itemAttack) return item;
        else if (currAttack && !itemAttack) return curr;
        else if (!currAttack && !itemAttack) return null;
        if (itemAttack.GetMaxAttack() < currAttack.GetMaxAttack()) {
          return item;
        }
        return curr;
      });
    }

    /// <summary>
    /// Gets the strongest character within the view distance
    /// </summary>
    /// <returns></returns>
    public AIBrain Strongest() {
      return others.Aggregate((curr, item) => {
        Attack currAttack = curr.GetComponent<Attack>();
        Attack itemAttack = item.GetComponent<Attack>();
        if (!currAttack && itemAttack) return item;
        else if (currAttack && !itemAttack) return curr;
        else if (!currAttack && !itemAttack) return null;
        if (itemAttack.GetMaxAttack() > currAttack.GetMaxAttack()) {
          return item;
        }
        return curr;
      });
    }

    /// <summary>
    /// Gets the sickest character within the view distance.
    /// **Note:** The other character must have the "Health" component to qualify.
    /// </summary>
    /// <returns></returns>
    public AIBrain Sickliest() {
      return others.Aggregate((curr, item) => {
        Health currHealth = curr.GetComponent<Health>();
        Health itemHealth = item.GetComponent<Health>();
        if (!currHealth || !itemHealth) return curr;
        if (itemHealth.health < currHealth.health) {
          return item;
        }
        return curr;
      });
    }

    /// <summary>
    /// Gets the healthiest character within the view distance.
    /// **Note:** The other character must have the "Health" component to qualify.
    /// </summary>
    /// <returns></returns>
    public AIBrain Healthiest() {
      return others.Aggregate((curr, item) => {
        Health currHealth = curr.GetComponent<Health>();
        Health itemHealth = item.GetComponent<Health>();
        if (!currHealth || !itemHealth) return curr;
        if (itemHealth.health > currHealth.health) {
          return item;
        }
        return curr;
      });
    }

    public AIBrain Oldest() {
      return others.Aggregate((curr, item) => {
        return item.Age > curr.Age ? item : curr;
      });
    }

    public AIBrain Youngest() {
      return others.Aggregate((curr, item) => {
        return item.Age < curr.Age ? item : curr;
      });
    }

    void LateUpdate() {
      // Force the view collider to a trigger
      if (viewDistance) viewDistance.isTrigger = true;
    }

    void OnTriggerEnter(Collider other) {
      AIBrain character = other.GetComponent<AIBrain>();
      // Is this a character?
      if (character == null) return;
      if (!others.Contains(character)) {
        others.Add(character);
      }
    }

    void OnTriggerExit(Collider other) {
      AIBrain character = other.GetComponent<AIBrain>();
      others.Remove(character);
    }

  }
}