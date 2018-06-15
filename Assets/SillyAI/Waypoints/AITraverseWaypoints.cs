using UnityEngine;
using UnityEngine.Events;

namespace SillyAI {
  [System.Serializable]
  public class NextWaypoint : UnityEvent<AIDestination> { }

  public class AITraverseWaypoints : MonoBehaviour {

    public AIWaypoints waypoints;

    [Readonly, SerializeField]
    private AIDestination next;

    [Space]
    public NextWaypoint nextWaypoint;

    void Update() {
      if (next == null) {
        next = waypoints.GetFirstWaypoint();
      }
    }

    public void Next() {

    }

  }
}