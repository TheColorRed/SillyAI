using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace SillyAI {


  [System.Serializable]
  public class NextWaypoint : UnityEvent<AIDestination> { }

  public enum StartType {
    First, Last,
    Random, RoundRobin,
    ClosestByAir, FurthestByAir,
    ClosestByGround, FurthestByGround,
    LeastTraffic, MostTraffic,
    Weighted, Specific
  }

  [DisallowMultipleComponent, RequireComponent(typeof(NavMeshAgent))]
  public class AITraverseWaypoints : AI {

    [Tooltip("A GameObject with child AIDestination's.")]
    public AIWaypoints waypointGroup;
    [Tooltip("The distance from the waypoint to go to the next.")]
    public float waypointDistance;
    [Tooltip("Which Waypoint to start at.")]
    public StartType startingWaypoint;
    [Tooltip("The starting waypoint. Requires \"Starting Waypoint\" to be set as \"Specific\".")]
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
    public static AIDestination roundRobinLastLocation;


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
      waypointGroup.events.RemoveEventListener("AIDestinationMoved", DestinationMoved);
    }

    void Start() {
      if (!waypointGroup) return;
      waypointGroup.events.AddEventListener("AIDestinationMoved", DestinationMoved);
      switch (startingWaypoint) {
        case StartType.First:
          current = waypointGroup.GetFirstWaypoint();
          break;
        case StartType.Last:
          current = waypointGroup.GetLastWaypoint();
          break;
        case StartType.Random:
          current = waypointGroup.GetRandomWaypoint();
          break;
        case StartType.RoundRobin:
          current = waypointGroup.GetNextWaypoint(roundRobinLastLocation);
          roundRobinLastLocation = current;
          break;
        case StartType.ClosestByAir:
          current = waypointGroup.GetClosestByAir(brain);
          break;
        case StartType.FurthestByAir:
          current = waypointGroup.GetFurthestByAir(brain);
          break;
        case StartType.ClosestByGround:
          current = waypointGroup.GetClosestByGround(brain);
          break;
        case StartType.FurthestByGround:
          current = waypointGroup.GetFurthestByGround(brain);
          break;
        case StartType.Weighted:
          current = waypointGroup.GetWeightedWaypoint();
          break;
        case StartType.LeastTraffic:
          current = waypointGroup.GetLeastTrafficWaypoint();
          break;
        case StartType.MostTraffic:
          current = waypointGroup.GetMostTrafficWaypoint();
          break;
        case StartType.Specific:
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

    void DestinationMoved(Event e) {
      AIDestination destination = (AIDestination)e.data;
      if (!destination) return;
      if (current != destination) return;
      agent.CalculatePath(destination.position, path);
      agent.SetPath(path);
    }

    void OnDrawGizmosSelected() {
      Gizmos.color = Color.blue;
      if (current) Gizmos.DrawLine(transform.position, current.position);

      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, waypointDistance);
    }

  }
}