using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
namespace SillyAI {

  public enum UnitType { Ground, Air, Water }
  // public enum AttackType { None, Ground, Air, Water }
  public enum AttackPriority { Any, Closest, Furthest, Strongest, Weakest }

  [RequireComponent(typeof(NavMeshAgent))]
  public class Character : MonoBehaviour {

    [Header("Character Distances")]
    [Tooltip("The maximum distance another character is detectable to the current character.")]
    public Collider viewDistance;
    [Tooltip("The maximum distance another character can be to be attackable.")]
    public float attackDistance = 5f;

    [Header("Character Attack Information")]
    [Tooltip("The type of unit.")]
    public UnitType unitType;
    [Tooltip("What type of units this character can attack.")]
    public List<UnitType> canAttack;
    [Tooltip("What type of character should this character attack as priority.")]
    public AttackPriority attackPriority;
    [Tooltip("Should the priority of the character be within the attack zone only?")]
    public bool attackZoneOnly;
    [Tooltip("Should this character attack until the unit is dead or attack units as they enter/leave the attack zone based on priority?")]
    public bool attackUntilDead = true;

    [Header("Character Destinations")]
    [Tooltip("The main destination for the character.")]
    public Destination destination;
    [Tooltip("Sub destinations for where a character needs to perform a sub task before returning to its primary destination.")]
    public List<Destination> subDestinations = new List<Destination>();

    private NavMeshAgent navMeshAgent;
    private NavMeshPath path;
    private List<GameObject> others = new List<GameObject>();

    void Awake() {
      if (viewDistance) viewDistance.isTrigger = true;
      navMeshAgent = GetComponent<NavMeshAgent>();
      path = new NavMeshPath();
    }

    void Start() {
      if (destination != null) {
        navMeshAgent.CalculatePath(destination.position, path);
        navMeshAgent.SetPath(path);
      }
    }

    void Update() {
      if (others.Count == 0) return;
      GameObject objectToAttack;
      switch (attackPriority) {
        case AttackPriority.Closest:
          objectToAttack = Closest(attackZoneOnly);
          break;
        case AttackPriority.Furthest:
          objectToAttack = Furthest(attackZoneOnly);
          break;
        case AttackPriority.Weakest:
          objectToAttack = Weakest(attackZoneOnly);
          break;
        case AttackPriority.Strongest:
          objectToAttack = Strongest(attackZoneOnly);
          break;
      }
    }

    /// <summary>
    /// Gets the closest character within the view distance
    /// </summary>
    /// <param name="attackZoneOnly">Should the character be within the attack zone?</param>
    /// <returns></returns>
    public GameObject Closest(bool attackZoneOnly = false) {
      float distance = Mathf.Infinity;
      GameObject closest = null;
      foreach (var item in others) {
        float itemDistance = Vector3.Distance(transform.position, item.transform.position);
        if (itemDistance < distance) {
          if (attackZoneOnly && itemDistance > attackDistance) continue;
          distance = itemDistance;
          closest = item.gameObject;
        }
      }
      return closest;
    }

    /// <summary>
    /// Gets the furthest character within the view distance
    /// </summary>
    /// <param name="attackZoneOnly">Should the character be within the attack zone?</param>
    /// <returns></returns>
    public GameObject Furthest(bool attackZoneOnly = false) {
      float distance = 0;
      GameObject closest = null;
      foreach (var item in others) {
        float itemDistance = Vector3.Distance(transform.position, item.transform.position);
        if (itemDistance > distance) {
          if (attackZoneOnly && itemDistance > attackDistance) continue;
          distance = itemDistance;
          closest = item.gameObject;
        }
      }
      return closest;
    }

    public GameObject Weakest(bool attackZoneOnly = false) {
      return null;
    }

    public GameObject Strongest(bool attackZoneOnly = false) {
      return null;
    }

    void LateUpdate() {
      // Force the view collider to a trigger
      if (viewDistance) viewDistance.isTrigger = true;
    }

    void OnTriggerEnter(Collider other) {
      Character character = other.GetComponent<Character>();
      // Is this a character?
      if (character == null) return;
      if (!canAttack.Contains(character.unitType)) return;
      if (!others.Contains(character.gameObject)) {
        others.Add(character.gameObject);
      }
    }

    void OnTriggerExit(Collider other) {
      if (others.Contains(other.gameObject)) {
        others.Remove(other.gameObject);
      }
    }

    void OnDrawGizmosSelected() {
      // Draw view distance
      // Gizmos.color = new Color(1, 1, 0);
      // Gizmos.matrix = Matrix4x4.Rotate(transform.rotation);
      // Gizmos.DrawWireSphere(transform.position, viewDistance);

      // Gizmos.matrix = Matrix4x4.Rotate(Quaternion.Euler(transform.rotation.x, transform.rotation.y + 45, transform.rotation.z));
      // Gizmos.DrawWireSphere(transform.position, viewDistance);

      // // Draw attack distance
      Gizmos.color = new Color(1, 0, 0);
      Gizmos.matrix = Matrix4x4.Rotate(transform.rotation);
      Gizmos.DrawWireSphere(transform.position, attackDistance);

      // Gizmos.matrix = Matrix4x4.Rotate(Quaternion.Euler(transform.rotation.x, transform.rotation.y + 45, transform.rotation.z));
      // Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

  }
}