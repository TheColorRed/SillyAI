using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace SillyAI {
  [RequireComponent(typeof(AIBrain), typeof(NavMeshAgent))]
  public abstract class AI : MonoBehaviour {

    private AIBrain _brain;
    private NavMeshAgent _agent;
    private NavMeshPath _path;

    public AIBrain brain {
      get { return _brain; }
      private set { _brain = value; }
    }

    public NavMeshAgent agent {
      get { return _agent; }
      private set { _agent = value; }
    }

    public NavMeshPath path {
      get { return _path; }
      private set { _path = value; }
    }

    protected void Awake() {
      brain = GetComponent<AIBrain>();
      agent = GetComponent<NavMeshAgent>();
      path = new NavMeshPath();
    }

    public void DispatchEvent(Event evt) {
      if (!brain) return;
      brain.events.DispatchEvent(evt);
    }

    public void AddEventListener(string name, UnityAction<Event> action) {
      if (!brain) return;
      brain.events.AddEventListener(name, action);
    }

    public void RemoveEventListener(string name, UnityAction<Event> action) {
      if (!brain) return;
      brain.events.RemoveEventListener(name, action);
    }

  }
}