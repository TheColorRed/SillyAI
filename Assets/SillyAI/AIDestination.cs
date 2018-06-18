using UnityEngine;
using UnityEngine.Events;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SillyAI {

  [System.Serializable]
  public class DestinationTrigger : UnityEvent<AIBrain> { }

  [AddComponentMenu("SillyAI/Destination"), DisallowMultipleComponent]
  public class AIDestination : MonoBehaviour {

    private Collider trigger;

    public DestinationTrigger enterTrigger;

    public Vector3 position {
      get { return transform.position; }
    }

    void Awake() {
      trigger = GetComponent<Collider>();
      if (trigger) trigger.isTrigger = true;
    }

    void LateUpdate() {
      if (trigger) trigger.isTrigger = true;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected() {
      if (!transform.parent) return;
      AIWaypoints waypoint = transform.parent.GetComponent<AIWaypoints>();
      if (!waypoint) return;
      int index = transform.GetSiblingIndex() + 1;
      var gui = new GUIStyle();
      gui.fontSize = 16;
      gui.fontStyle = FontStyle.Bold;
      Handles.Label(transform.position, index.ToString(), gui);
    }
#endif
  }

}