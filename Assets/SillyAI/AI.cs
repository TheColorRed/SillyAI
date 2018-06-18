using UnityEngine;
using UnityEngine.Events;

namespace SillyAI {
  [RequireComponent(typeof(AIBrain))]
  public abstract class AI : MonoBehaviour {

    private AIBrain _brain;

    public AIBrain brain {
      get { return _brain; }
      private set { _brain = value; }
    }

    protected void Awake() {
      brain = GetComponent<AIBrain>();
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