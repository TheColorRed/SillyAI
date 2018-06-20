using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

namespace SillyAI {

  [AddComponentMenu("SillyAI/AI Waypoints")]
  public class AIWaypoints : MonoBehaviour {

    public bool closedCircut = false;

    private List<AIDestination> waypoints { get { return GetComponentsInChildren<AIDestination>().ToList(); } }

    public readonly AIEvent events = new AIEvent();

    public AIDestination GetWaypoint(int index) {
      return waypoints[Mathf.Clamp(index, 0, waypoints.Count - 1)];
    }

    public AIDestination GetNextWaypoint(AIDestination current) {
      int idx = waypoints.FindIndex(item => item == current);
      if (idx < 0) return GetFirstWaypoint();
      if (idx + 1 >= waypoints.Count && closedCircut) {
        return GetFirstWaypoint();
      } else if (idx + 1 >= waypoints.Count && !closedCircut) {
        return GetLastWaypoint();
      }
      return waypoints[idx + 1];
    }

    public AIDestination GetPreviousWaypoint(AIDestination current) {
      int idx = waypoints.FindIndex(item => item == current);
      if (idx < 0) return GetLastWaypoint();
      if (idx - 1 < 0 && closedCircut) {
        return GetLastWaypoint();
      } else if (idx - 1 < 0 && !closedCircut) {
        return GetFirstWaypoint();
      }
      return waypoints[idx - 1];
    }

    public AIDestination GetFirstWaypoint() {
      if (waypoints.Count == 0) return null;
      return waypoints[0];
    }

    public AIDestination GetLastWaypoint() {
      if (waypoints.Count == 0) return null;
      return waypoints[waypoints.Count - 1];
    }

    public AIDestination GetRandomWaypoint() {
      if (waypoints.Count == 0) return null;
      return waypoints[Random.Range(0, waypoints.Count)];
    }

    public AIDestination GetClosestByAir(AIBrain brain) {
      return waypoints.Aggregate((curr, item) => {
        float currDist = Vector3.Distance(curr.transform.position, brain.transform.position);
        float itemDist = Vector3.Distance(item.transform.position, brain.transform.position);
        return itemDist < currDist ? item : curr;
      });
    }

    public AIDestination GetFurthestByAir(AIBrain brain) {
      return waypoints.Aggregate((curr, item) => {
        float currDist = Vector3.Distance(curr.transform.position, brain.transform.position);
        float itemDist = Vector3.Distance(item.transform.position, brain.transform.position);
        return itemDist > currDist ? item : curr;
      });
    }

    public AIDestination GetClosestByGround(AIBrain brain) {
      return GetDistances(brain).Aggregate((curr, item) => item.Value < curr.Value ? item : curr).Key;
    }

    public AIDestination GetFurthestByGround(AIBrain brain) {
      return GetDistances(brain).Aggregate((curr, item) => item.Value > curr.Value ? item : curr).Key;
    }

    public AIDestination GetWeightedWaypoint() {
      List<int> indexes = new List<int>();
      for (var i = 0; i < waypoints.Count; i++) {
        for (var j = 0; j < waypoints[i].weight; j++) {
          indexes.Add(i);
        }
      }
      return waypoints[indexes[Random.Range(0, indexes.Count)]];
    }

    public AIDestination GetLeastTrafficWaypoint() {
      return waypoints.Aggregate((curr, item) => item.brainCount < curr.brainCount ? item : curr);
    }

    public AIDestination GetMostTrafficWaypoint() {
      return waypoints.Aggregate((curr, item) => item.brainCount > curr.brainCount ? item : curr);
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

    private Dictionary<AIDestination, float> GetDistances(AIBrain brain) {
      Dictionary<AIDestination, float> distances = new Dictionary<AIDestination, float>();
      var agent = brain.GetComponent<NavMeshAgent>();
      var path = new NavMeshPath();
      foreach (var waypoint in waypoints) {
        agent.CalculatePath(waypoint.position, path);
        Vector3 previousCorner = path.corners[0];
        float dist = 0;
        foreach (var corner in path.corners) {
          dist += Vector3.Distance(previousCorner, corner);
          previousCorner = corner;
        }
        distances.Add(waypoint, dist);
      }
      return distances;
    }

    void OnDrawGizmosSelected() {
      if (transform.childCount < 2) return;
      Vector3 current = transform.GetComponentInChildren<AIDestination>().transform.position;
      Gizmos.color = new Color(0.8f, 0.4f, 0);
      foreach (Transform item in transform) {
        if (!item.gameObject.activeSelf) continue;
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