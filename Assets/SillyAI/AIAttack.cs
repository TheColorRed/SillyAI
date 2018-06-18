using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SillyAI {

  [System.Serializable]
  public class AttackType {

    public string name;
    public float damage;

  }

  [System.Serializable]
  public class AIAttackStart : UnityEvent<AIBrain> { }

  public enum AttackPriority { Any, Closest, Furthest, Strongest, Weakest, Healthiest, Sickest, Oldest, Youngest }

  [AddComponentMenu("SillyAI/Attack"), DisallowMultipleComponent, RequireComponent(typeof(AIBrain))]
  public class AIAttack : AI {

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
    public UnitType[] attackableUnitTypes;

    public AIAttackStart attackStart;

    private AIBrain toAttack = null;

    void Update() {
      switch (attackPriority) {
        case AttackPriority.Closest:
          toAttack = brain.Closest(attackableUnitTypes);
          break;
        case AttackPriority.Furthest:
          toAttack = brain.Furthest(attackableUnitTypes);
          break;
        case AttackPriority.Weakest:
          toAttack = brain.Weakest(attackableUnitTypes);
          break;
        case AttackPriority.Strongest:
          toAttack = brain.Strongest(attackableUnitTypes);
          break;
        case AttackPriority.Healthiest:
          toAttack = brain.Healthiest(attackableUnitTypes);
          break;
        case AttackPriority.Sickest:
          toAttack = brain.Sickliest(attackableUnitTypes);
          break;
        case AttackPriority.Oldest:
          toAttack = brain.Oldest(attackableUnitTypes);
          break;
        case AttackPriority.Youngest:
          toAttack = brain.Youngest(attackableUnitTypes);
          break;
      }
      if (toAttack) {
        if (!brain.subDestinations.Contains(toAttack.GetComponent<AIDestination>())) {
          brain.subDestinations.Add(toAttack.GetComponent<AIDestination>());
        }
        attackStart.Invoke(toAttack);
      }
    }

    public bool InAttackZone(AIBrain otherBrain) {
      float distance = Vector3.Distance(transform.position, otherBrain.transform.position);
      return distance <= otherBrain.GetComponent<AIAttack>().attackDistance;
    }

    public void ApplyDamage(AIHealth otherHealth, float amount) {
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