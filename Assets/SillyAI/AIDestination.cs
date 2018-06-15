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

    void OnTriggerEnter(Collider other) {
      AIWaypoints waypoint = transform.parent.GetComponent<AIWaypoints>();
      if (waypoint) {
        // waypoint.Next(other.GetComponent<>());
      }
      enterTrigger.Invoke(other.GetComponent<AIBrain>());
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected() {
      AIWaypoints waypoint = transform.parent.GetComponent<AIWaypoints>();
      if (!waypoint) return;
      int index = transform.GetSiblingIndex() + 1;
      var gui = new GUIStyle();
      gui.fontSize = 16;
      gui.fontStyle = FontStyle.Bold;
      Handles.Label(transform.position, index.ToString(), gui);
    }
#endif
    // void OnDrawGizmos() {
    //   // Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one) * Matrix4x4.Rotate(Quaternion.Euler(0, 0, 45));
    //   // Gizmos.DrawIcon(transform.position, "Destination.png", false);

    //   var mesh = new Mesh();
    //   mesh.vertices = new Vector3[] {
    //     new Vector3(-0.5f, 0, -0.5f),
    //     new Vector3(-0.5f, 0, 0.5f),
    //     new Vector3(0.5f, 0, 0.5f),
    //     new Vector3(0.5f, 0, -0.5f),
    //   };
    //   mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

    //   mesh.normals = Enumerable.Repeat(Vector3.forward, 4).ToArray();

    //   Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
    // }
  }

}