using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace SillyAI {

  public enum UnitType { Ground, Air, Water, Building, Wall, Prop, Foliage }

  [AddComponentMenu("SillyAI/AI Brain"), DisallowMultipleComponent]
  public class AIBrain : AI {

    [Header("Character Distances")]
    [Tooltip("The maximum distance another character is detectable to the current character.")]
    public float viewDistanceRadius;

    [Header("Character Information")]
    [Tooltip("The type of unit.")]
    public UnitType unitType;

    [Header("Character Destinations")]
    [Tooltip("The main destination for the character.")]
    public AIDestination destination;
    [Tooltip("Sub destinations for where a character needs to perform a sub task before returning to its primary destination.")]
    public List<AIDestination> subDestinations = new List<AIDestination>();

    public UnityEvent destinationComplete;

    // private NavMeshAgent agent;
    private SphereCollider viewTrigger;
    private Rigidbody aiRigidbody;
    private AIHealth health;
    private List<AIBrain> _otherBrains = new List<AIBrain>();
    private float dob = 0f;
    private bool destDone = false;

    public readonly AIEvent events = new AIEvent();

    public ReadOnlyCollection<AIBrain> surroundingBrains {
      get { return _otherBrains.AsReadOnly(); }
    }

    public float Age {
      get { return Time.time - dob; }
    }

    new void Awake() {
      base.Awake();

      // Add the view trigger
      viewTrigger = gameObject.AddComponent<SphereCollider>();
      viewTrigger.isTrigger = true;
      viewTrigger.radius = viewDistanceRadius;

      // Add a kinematic rigidbody for triggers
      aiRigidbody = GetComponent<Rigidbody>();
      if (!aiRigidbody) aiRigidbody = gameObject.AddComponent<Rigidbody>();
      aiRigidbody.isKinematic = true;

      // Setup the nav mesh agent
      health = GetComponent<AIHealth>();

      // Set the objects date of birth
      dob = Time.time;

      var hasDestination = GetComponent<AIDestination>();
      if (!hasDestination) gameObject.AddComponent<AIDestination>();
    }

    void Start() {
      if (destination != null) {
        SetDestination(destination.position);
      }
    }

    void Update() {
      float dist = agent.remainingDistance;
      if (!destDone && dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && dist == 0) {
        destDone = true;
        destinationComplete.Invoke();
      }
    }

    void LateUpdate() {
      // Force the view collider to a trigger
      if (viewTrigger) viewTrigger.isTrigger = true;
    }

    void OnTriggerEnter(Collider other) {
      // If we collide with ourself don't add
      if (other.transform.IsChildOf(transform)) return;
      AIBrain otherBrain = other.GetComponent<AIBrain>();
      // Is this a character?
      if (otherBrain == null) return;
      if (!_otherBrains.Contains(otherBrain)) {
        _otherBrains.Add(otherBrain);
      }
    }

    void OnTriggerExit(Collider other) {
      AIBrain character = other.GetComponent<AIBrain>();
      _otherBrains.Remove(character);
    }

    public void SetDestination(Vector3 target, bool calculatePath = true) {
      if (calculatePath) {
        agent.CalculatePath(target, path);
        agent.SetPath(path);
      } else {
        agent.SetDestination(target);
      }
    }

    public void StoppingDistance(float distance) {
      agent.stoppingDistance = distance;
    }

    /// <summary>
    /// Gets the closest character within the view distance
    /// </summary>
    /// <returns></returns>
    public AIBrain Closest(IEnumerable<AIBrain> list = null) {
      var items = list == null ? surroundingBrains : list;
      if (items.Count() == 0) return null;
      return items
        .Aggregate((AIBrain)null, (curr, item) => {
          if (curr == null && item != null) return item;
          float currDistance = Vector3.Distance(transform.position, curr.transform.position);
          float itemDistance = Vector3.Distance(transform.position, item.transform.position);
          return itemDistance < currDistance ? item : curr;
        });
    }

    /// <summary>
    /// Gets the furthest character within the view distance
    /// </summary>
    /// <returns></returns>
    public AIBrain Furthest(IEnumerable<AIBrain> list = null) {
      var items = list == null ? surroundingBrains : list;
      if (items.Count() == 0) return null;
      return items
        .Aggregate((AIBrain)null, (curr, item) => {
          if (curr == null && item != null) return item;
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
    public AIBrain Weakest(IEnumerable<AIBrain> list = null) {
      var items = list == null ? surroundingBrains : list;
      if (items.Count() == 0) return null;
      return items
        .Aggregate((AIBrain)null, (curr, item) => {
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
    public AIBrain Strongest(IEnumerable<AIBrain> list = null) {
      var items = list == null ? surroundingBrains : list;
      if (items.Count() == 0) return null;
      return items
        .Aggregate((AIBrain)null, (curr, item) => {
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
    public AIBrain Sickliest(IEnumerable<AIBrain> list = null) {
      var items = list == null ? surroundingBrains : list;
      if (items.Count() == 0) return null;
      return items
        .Aggregate((AIBrain)null, (curr, item) => {
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
    public AIBrain Healthiest(IEnumerable<AIBrain> list = null) {
      var items = list == null ? surroundingBrains : list;
      if (items.Count() == 0) return null;
      return items
        .Aggregate((AIBrain)null, (curr, item) => {
          AIHealth currHealth = curr.GetComponent<AIHealth>();
          AIHealth itemHealth = item.GetComponent<AIHealth>();
          if (!currHealth || !itemHealth) return curr;
          if (itemHealth.health > currHealth.health) {
            return item;
          }
          return curr;
        });
    }

    public AIBrain Oldest(IEnumerable<AIBrain> list = null) {
      var items = list == null ? surroundingBrains : list;
      if (items.Count() == 0) return null;
      return items
        .Aggregate((AIBrain)null, (curr, item) => {
          return item.Age > curr.Age ? item : curr;
        });
    }

    public AIBrain Youngest(IEnumerable<AIBrain> list = null) {
      var items = list == null ? surroundingBrains : list;
      if (items.Count() == 0) return null;
      return items
        .Aggregate((AIBrain)null, (curr, item) => {
          return item.Age < curr.Age ? item : curr;
        });
    }

    void OnDrawGizmosSelected() {
      if (viewDistanceRadius <= 0) return;
      // Draw attack distance
      Gizmos.color = new Color(0, 1, 0);
      // Draw the first sphere
      Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
      Gizmos.DrawWireSphere(Vector3.zero, viewDistanceRadius);
      // Draw the second sphere rotated 45 degrees around the Y axis
      Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one) * Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
      Gizmos.DrawWireSphere(Vector3.zero, viewDistanceRadius);
    }

  }
}