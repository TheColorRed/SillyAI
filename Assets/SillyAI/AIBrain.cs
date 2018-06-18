using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SillyAI {

  public enum UnitType { None, Ground, Air, Water }

  [AddComponentMenu("SillyAI/Brain"), DisallowMultipleComponent, RequireComponent(typeof(NavMeshAgent))]
  public class AIBrain : AI {

    [Header("Character Distances")]
    [Tooltip("The maximum distance another character is detectable to the current character.")]
    public Collider viewDistanceTrigger;

    [Header("Character Attack Information")]
    [Tooltip("The type of unit.")]
    public UnitType unitType;

    [Header("Character Destinations")]
    [Tooltip("The main destination for the character.")]
    public AIDestination destination;
    [Tooltip("Sub destinations for where a character needs to perform a sub task before returning to its primary destination.")]
    public List<AIDestination> subDestinations = new List<AIDestination>();

    public UnityEvent destinationComplete;

    private NavMeshAgent navMeshAgent;
    private NavMeshPath path;
    private AIHealth health;
    private List<AIBrain> others = new List<AIBrain>();
    private float dob = 0f;
    private bool destDone = false;

    public readonly AIEvent events = new AIEvent();

    public float Age {
      get { return Time.time - dob; }
    }

    new void Awake() {
      base.Awake();
      if (viewDistanceTrigger) viewDistanceTrigger.isTrigger = true;
      navMeshAgent = GetComponent<NavMeshAgent>();
      health = GetComponent<AIHealth>();
      path = new NavMeshPath();
      dob = Time.time;
      var hasDestination = GetComponent<AIDestination>();
      if (!hasDestination) gameObject.AddComponent<AIDestination>();
    }

    void Start() {
      if (destination != null) {
        navMeshAgent.CalculatePath(destination.position, path);
        navMeshAgent.SetPath(path);
      }
    }

    void Update() {
      float dist = navMeshAgent.remainingDistance;
      if (!destDone && dist != Mathf.Infinity && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && dist == 0) {
        destDone = true;
        destinationComplete.Invoke();
      }
    }

    void LateUpdate() {
      // Force the view collider to a trigger
      if (viewDistanceTrigger) viewDistanceTrigger.isTrigger = true;
    }

    void OnTriggerEnter(Collider other) {
      // If we collide with ourself don't add
      if (other.transform.IsChildOf(transform)) return;
      AIBrain otherBrain = other.GetComponent<AIBrain>();
      // Is this a character?
      if (otherBrain == null) return;
      if (!others.Contains(otherBrain)) {
        others.Add(otherBrain);
      }
    }

    void OnTriggerExit(Collider other) {
      AIBrain character = other.GetComponent<AIBrain>();
      others.Remove(character);
    }






    /// <summary>
    /// Gets the closest character within the view distance
    /// </summary>
    /// <returns></returns>
    public AIBrain Closest(params UnitType[] unitTypes) {
      return others.Aggregate((curr, item) => {
        float currDistance = Vector3.Distance(transform.position, curr.transform.position);
        float itemDistance = Vector3.Distance(transform.position, item.transform.position);
        if (unitTypes.Length > 0) {
          return itemDistance < currDistance && unitTypes.Contains(item.unitType) ? item : curr;
        }
        return itemDistance < currDistance ? item : curr;
      });
    }

    /// <summary>
    /// Gets the furthest character within the view distance
    /// </summary>
    /// <returns></returns>
    public AIBrain Furthest(params UnitType[] unitTypes) {
      return others.Aggregate((curr, item) => {
        float currDistance = Vector3.Distance(transform.position, curr.transform.position);
        float itemDistance = Vector3.Distance(transform.position, item.transform.position);
        if (unitTypes.Length > 0) {
          return itemDistance < currDistance && unitTypes.Contains(item.unitType) ? item : curr;
        }
        return itemDistance > currDistance ? item : curr;
      });
    }

    /// <summary>
    /// Gets the weakest character within the view distance.
    /// **Note:** The other character must have the "Health" component to qualify.
    /// </summary>
    /// <returns></returns>
    public AIBrain Weakest(params UnitType[] unitTypes) {
      return others.Aggregate((curr, item) => {
        AIAttack currAttack = curr.GetComponent<AIAttack>();
        AIAttack itemAttack = item.GetComponent<AIAttack>();
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
    public AIBrain Strongest(params UnitType[] unitTypes) {
      return others.Aggregate((curr, item) => {
        AIAttack currAttack = curr.GetComponent<AIAttack>();
        AIAttack itemAttack = item.GetComponent<AIAttack>();
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
    public AIBrain Sickliest(params UnitType[] unitTypes) {
      return others.Aggregate((curr, item) => {
        AIHealth currHealth = curr.GetComponent<AIHealth>();
        AIHealth itemHealth = item.GetComponent<AIHealth>();
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
    public AIBrain Healthiest(params UnitType[] unitTypes) {
      return others.Aggregate((curr, item) => {
        AIHealth currHealth = curr.GetComponent<AIHealth>();
        AIHealth itemHealth = item.GetComponent<AIHealth>();
        if (!currHealth || !itemHealth) return curr;
        if (itemHealth.health > currHealth.health) {
          return item;
        }
        return curr;
      });
    }

    public AIBrain Oldest(params UnitType[] unitTypes) {
      return others.Aggregate((curr, item) => {
        return item.Age > curr.Age ? item : curr;
      });
    }

    public AIBrain Youngest(params UnitType[] unitTypes) {
      return others.Aggregate((curr, item) => {
        return item.Age < curr.Age ? item : curr;
      });
    }

  }
}