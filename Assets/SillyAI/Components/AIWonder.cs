using UnityEngine;

namespace SillyAI {
  public class AIWonder : AI {

    [Range(0, 360), Tooltip("How much of an angle does the AI have to choose the next point.")]
    public float maxRotation = 30;
    [Range(0, 1), Tooltip("The probability percentage that the AI will turn around.")]
    public float turnAroundProbability = 0.25f;

    void Update() {
      if (agent.remainingDistance < 0.2f) {
        var position = (Vector3)Random.insideUnitCircle * brain.viewDistanceRadius;
        position.z = Mathf.Clamp(brain.viewDistanceRadius * position.y, brain.viewDistanceRadius * -1, brain.viewDistanceRadius);
        position.y = 0;
        brain.SetDestination(transform.position + position);
      }
    }

  }
}