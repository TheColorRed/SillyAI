using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SillyAI {

  [System.Serializable]
  public class AttackType {

    public string name;
    public float damage;

  }

  public enum AttackPriority { Any, Closest, Furthest, Strongest, Weakest, Healthiest, Sickest, Oldest, Youngest }

  [AddComponentMenu("SillyAI/Attack"), DisallowMultipleComponent, RequireComponent(typeof(AIBrain))]
  public class Attack : MonoBehaviour {

    [Tooltip("The maximum distance another character can be to be attackable.")]
    public float attackDistance = 5f;

    [Header("Attack Priority")]
    [Tooltip("What type of character should this character attack as priority.")]
    public AttackPriority attackPriority;
    [Tooltip("Should this character attack until the other character is dead or attack units as they enter/leave the attack zone based on priority?")]
    public bool attackUntilDead = true;
    [Tooltip("Should this character pursue a fleeing character?")]
    public bool pursue = false;

    [Header("Attack Information")]
    [Tooltip("The types of attacks this character can perform.")]
    public List<AttackType> attackTypes = new List<AttackType>();
    [Tooltip("What type of units this character can attack.")]
    public List<UnitType> canAttack;

    private AIBrain brain;
    private AIBrain ToAttack = null;

    void Awake() {
      brain = GetComponent<AIBrain>();
    }

    void Update() {
      switch (attackPriority) {
        case AttackPriority.Closest:
          ToAttack = brain.Closest();
          break;
        case AttackPriority.Furthest:
          ToAttack = brain.Furthest();
          break;
        case AttackPriority.Weakest:
          ToAttack = brain.Weakest();
          break;
        case AttackPriority.Strongest:
          ToAttack = brain.Strongest();
          break;
        case AttackPriority.Healthiest:
          ToAttack = brain.Healthiest();
          break;
        case AttackPriority.Sickest:
          ToAttack = brain.Sickliest();
          break;
        case AttackPriority.Oldest:
          ToAttack = brain.Oldest();
          break;
        case AttackPriority.Youngest:
          ToAttack = brain.Youngest();
          break;
      }
      if (ToAttack) {
        brain.subDestinations.Add(ToAttack.GetComponent<Destination>());
      }
    }

    public bool InAttackZone(Attack other) {
      float distance = Vector3.Distance(transform.position, other.transform.position);
      return distance <= other.attackDistance;
    }

    public void ApplyDamage(Health otherHealth, float amount) {
      otherHealth.RemoveAmount(amount);
    }

    public float GetMaxAttack() {
      return attackTypes.Aggregate(Single.MinValue, (acc, elem) => acc < elem.damage ? elem.damage : acc);
    }

    public float GetMinAttack() {
      return attackTypes.Aggregate(Single.MaxValue, (acc, elem) => acc > elem.damage ? elem.damage : acc);
    }

    void OnDrawGizmosSelected() {
      // Draw attack distance
      Gizmos.color = new Color(1, 0, 0);
      // Draw the first sphere
      Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
      Gizmos.DrawWireSphere(Vector3.zero, attackDistance);
      // Draw the second sphere rotated 45 degrees around the Y axis
      Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one) * Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
      Gizmos.DrawWireSphere(Vector3.zero, attackDistance);
    }
  }

}