using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace SillyAI {

  public class AIWaypoints : MonoBehaviour {

    public bool closedCircut = false;

    private List<AIDestination> waypoints = new List<AIDestination>();

    void Start() {
      foreach (Transform item in transform) {
        AIDestination dest = item.GetComponent<AIDestination>();
        if (dest) waypoints.Add(dest);
      }
    }

    public AIDestination GetWaypoint(int index) {
      return waypoints[Mathf.Clamp(index, 0, waypoints.Count - 1)];
    }

    public AIDestination GetFirstWaypoint() {
      return waypoints[0];
    }

    public AIDestination GetLastWaypoint() {
      return waypoints[waypoints.Count - 1];
    }

    public void CreateWaypoint(Vector3 localPosition, int index = -1) {
      GameObject go = new GameObject("Destination");
      waypoints.Add(go.AddComponent<AIDestination>());
      go.transform.parent = transform;
      if (index > -1) go.transform.SetSiblingIndex(index);
      go.transform.localPosition = localPosition;
    }

    public void AddWaypoint(AIDestination waypoint, Vector3 localPosition, int index = -1) {
      waypoint.transform.parent = transform;
      if (index > -1) waypoint.transform.SetSiblingIndex(index);
      if (!waypoints.Contains(waypoint)) waypoints.Add(waypoint);
      waypoint.transform.localPosition = localPosition;
    }

    public void RemoveWaypoint(AIDestination waypoint) {
      waypoints.Remove(waypoint);
      waypoint.transform.parent = transform.root;
    }

    public void DestroyWaypoint(AIDestination waypoint) {
      waypoints.Remove(waypoint);
      Destroy(waypoint.gameObject);
    }

    void OnDrawGizmosSelected() {
      if (transform.childCount < 2) return;
      Vector3 current = transform.GetComponentInChildren<AIDestination>().transform.position;
      Gizmos.color = Color.green;
      foreach (Transform item in transform) {
        AIDestination dest = item.GetComponent<AIDestination>();
        if (!dest) continue;
        Gizmos.DrawLine(current, item.position);
        current = item.position;
      }
      if (closedCircut) {
        Gizmos.DrawLine(current, transform.GetComponentInChildren<AIDestination>().transform.position);
      }
    }

  }
}