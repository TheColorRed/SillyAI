using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace SillyAI {


  [System.Serializable]
  public class NextWaypoint : UnityEvent<AIDestination> { }

  public enum GoTo { First, Last, ClosestByAir, FurthestByAir, ClosestByGround, FurthestByGround, Specific }

  [DisallowMultipleComponent, RequireComponent(typeof(NavMeshAgent))]
  public class AITraverseWaypoints : AI {

    [Tooltip("A GameObject with child AIDestination's")]
    public AIWaypoints waypointGroup;
    [Tooltip("The distance from the waypoint to go to the next.")]
    public float waypointDistance;
    [Tooltip("Which Waypoint to start at")]
    public GoTo startingWaypoint;
    public AIDestination specificStartLocation;

    [Space]
    public NextWaypoint nextWaypointTrigger;

    public AIDestination current {
      get { return _current; }
      private set { _current = value; }
    }

    [Readonly, SerializeField]
    private AIDestination _current;
    private NavMeshPath path;
    private NavMeshAgent agent;


    new void Awake() {
      base.Awake();
      agent = GetComponent<NavMeshAgent>();
      path = new NavMeshPath();
    }

    void OnEnable() {
      AddEventListener("NextWaypoint", NextWaypoint);
    }

    void OnDisable() {
      RemoveEventListener("NextWaypoint", NextWaypoint);
    }

    void Start() {
      if (!waypointGroup) return;
      switch (startingWaypoint) {
        case GoTo.First:
          current = waypointGroup.GetFirstWaypoint();
          break;
        case GoTo.Last:
          current = waypointGroup.GetLastWaypoint();
          break;
        case GoTo.ClosestByAir:
          current = waypointGroup.GetClosestByAir(brain);
          break;
        case GoTo.FurthestByAir:
          current = waypointGroup.GetFurthestByAir(brain);
          break;
        case GoTo.ClosestByGround:
          current = waypointGroup.GetClosestByGround(brain);
          break;
        case GoTo.FurthestByGround:
          current = waypointGroup.GetFurthestByGround(brain);
          break;
        case GoTo.Specific:
          if (specificStartLocation) current = specificStartLocation;
          else current = waypointGroup.GetFirstWaypoint();
          break;
        default:
          current = waypointGroup.GetFirstWaypoint();
          break;
      }
      agent.CalculatePath(current.position, path);
      agent.SetPath(path);
    }

    void Update() {
      if (!current) return;
      float dist = Vector3.Distance(transform.position, current.position);
      if (dist <= waypointDistance) {
        DispatchEvent(new Event("NextWaypoint").SetData(current));
      }
    }

    void NextWaypoint(Event e) {
      if ((AIDestination)e.data != current) return;
      if (!waypointGroup) return;
      AIDestination dest = waypointGroup.GetNextWaypoint(current);
      if (dest != null) {
        current = dest;
        agent.CalculatePath(current.position, path);
        agent.SetPath(path);
      }
    }

    void OnDrawGizmosSelected() {
      Gizmos.color = Color.blue;
      if (current) Gizmos.DrawLine(transform.position, current.position);

      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, waypointDistance);
    }

  }
}