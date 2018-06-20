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
  public enum AttackableType { None, Ground, Air, Water, Building, Wall, Prop, Foliage }

  [AddComponentMenu("SillyAI/AI Attack"), DisallowMultipleComponent]
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
    public AttackableType[] attackableUnitTypes;
    [Readonly, SerializeField]
    private AIBrain unitToAttack = null;

    public AIAttackStart attackStart;


    void Update() {
      switch (attackPriority) {
        case AttackPriority.Closest:
          unitToAttack = Closest();
          break;
        case AttackPriority.Furthest:
          unitToAttack = Furthest();
          break;
        case AttackPriority.Weakest:
          unitToAttack = Weakest();
          break;
        case AttackPriority.Strongest:
          unitToAttack = Strongest();
          break;
        case AttackPriority.Healthiest:
          unitToAttack = Healthiest();
          break;
        case AttackPriority.Sickest:
          unitToAttack = Sickliest();
          break;
        case AttackPriority.Oldest:
          unitToAttack = Oldest();
          break;
        case AttackPriority.Youngest:
          unitToAttack = Youngest();
          break;
      }
      if (unitToAttack) {
        if (!brain.subDestinations.Contains(unitToAttack.GetComponent<AIDestination>())) {
          brain.subDestinations.Add(unitToAttack.GetComponent<AIDestination>());
        }
        attackStart.Invoke(unitToAttack);
      }
    }

    public AIBrain Closest() {
      return brain.Closest(GetAttackable());
    }

    public AIBrain Furthest() {
      return brain.Furthest(GetAttackable());
    }

    public AIBrain Weakest() {
      return brain.Weakest(GetAttackable());
    }

    public AIBrain Strongest() {
      return brain.Strongest(GetAttackable());
    }

    public AIBrain Healthiest() {
      return brain.Healthiest(GetAttackable());
    }

    public AIBrain Sickliest() {
      return brain.Sickliest(GetAttackable());
    }

    public AIBrain Youngest() {
      return brain.Youngest(GetAttackable());
    }

    public AIBrain Oldest() {
      return brain.Oldest(GetAttackable());
    }

    public bool InAttackZone(AIBrain otherBrain) {
      float distance = Vector3.Distance(transform.position, otherBrain.transform.position);
      return distance <= otherBrain.GetComponent<AIAttack>().attackDistance;
    }

    public bool InAttackZoneAndIsAttackable(AIBrain otherbrain) {
      return InAttackZone(otherbrain) && IsAttackable(otherbrain);
    }

    public void ApplyDamage(AIHealth otherHealth, float amount) {
      otherHealth.RemoveHealth(amount);
    }

    public float GetMaxAttack() {
      return attackTypes.Aggregate(Single.MinValue, (acc, elem) => acc < elem.damage ? elem.damage : acc);
    }

    public float GetMinAttack() {
      return attackTypes.Aggregate(Single.MaxValue, (acc, elem) => acc > elem.damage ? elem.damage : acc);
    }

    public bool IsAttackable(AIBrain brain) {
      if (this.brain == brain) return false;
      return IsAttackable(brain.unitType);
    }

    public bool IsAttackable(UnitType unitType) {
      foreach (var item in attackableUnitTypes) {
        if (unitType.ToString() == item.ToString()) return true;
      }
      return false;
    }

    IEnumerable<AIBrain> GetAttackable() {
      return from item in brain.surroundingBrains where IsAttackable(item) select item;
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