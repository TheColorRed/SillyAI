using UnityEngine;

namespace SillyAI {
  public class AIFollow : AI {

    [Tooltip("The target to follow. This doesn't have to be an AI unit, it can be anything.")]
    public Transform follow;
    [Range(0.1f, 10f), Tooltip("How often should the transform be checked for a change?")]
    public float UpdateDuration = 1f;

    private Vector3 lastPosition;

    void Start() {
      if (follow) brain.SetDestination(follow.position);
      InvokeRepeating("Follow", UpdateDuration, UpdateDuration);
    }

    void Follow() {
      if (!follow) return;
      if (lastPosition != follow.position) {
        lastPosition = follow.position;
        brain.SetDestination(lastPosition);
      }
    }

    void OnDrawGizmosSelected() {
      Gizmos.color = Color.green;
      if (!follow) return;
      Gizmos.DrawLine(transform.position, follow.position);
    }

  }
}