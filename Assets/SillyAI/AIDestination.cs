using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SillyAI {

  [System.Serializable]
  public class DestinationTrigger : UnityEvent<AIBrain> { }

  [AddComponentMenu("SillyAI/Destination"), DisallowMultipleComponent]
  public class AIDestination : MonoBehaviour {

    [Tooltip("The amount of influence this destination has.")]
    public int weight = 1;
    private Collider trigger;

    private List<AIBrain> _brains = new List<AIBrain>();

    public DestinationTrigger enterTrigger;

    public Vector3 position {
      get { return transform.position; }
    }

    public int brainCount { get { return _brains.Count; } }

    private Vector3 lastPosition;

    void Awake() {
      trigger = GetComponent<Collider>();
      if (trigger) trigger.isTrigger = true;
      if (weight < 0) weight = 0;
      lastPosition = transform.position;
    }

    void Update() {
      if (lastPosition != transform.position) {
        AIWaypoints waypoints = transform.parent.GetComponent<AIWaypoints>();
        if (waypoints) {
          waypoints.events.DispatchEvent(new Event("AIDestinationMoved").SetData(this));
          lastPosition = transform.position;
        }
      }
    }

    void LateUpdate() {
      if (trigger) trigger.isTrigger = true;
      if (weight < 0) weight = 0;
    }

    void OnTriggerEnter(Collider other) {
      AIBrain otherBrain = other.GetComponent<AIBrain>();
      if (otherBrain) _brains.Add(otherBrain);
    }

    void OnTriggerExit(Collider other) {
      AIBrain otherBrain = other.GetComponent<AIBrain>();
      if (otherBrain) _brains.RemoveAll(item => item == otherBrain);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected() {
      if (!transform.parent || !gameObject.activeSelf) return;
      AIWaypoints waypoints = transform.parent.GetComponent<AIWaypoints>();
      if (!waypoints) return;
      int index = transform.GetSiblingIndex() + 1;
      var gui = new GUIStyle();
      gui.fontSize = 16;
      gui.fontStyle = FontStyle.Bold;
      Handles.Label(transform.position, index.ToString(), gui);
    }
#endif
  }

}