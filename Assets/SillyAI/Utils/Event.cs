using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace SillyAI {
  [System.Serializable]
  public class Event : UnityEvent<Event> {

    public string name;
    public object data {
      get { return _data; }
      private set { _data = value; }
    }

    private object _data;

    public Event(string name) {
      this.name = name;
    }

    public Event SetData<T>(T data) {
      this.data = data;
      return this;
    }
  }

  public class AIEvent {

    private Dictionary<string, List<UnityAction<Event>>> eventList = new Dictionary<string, List<UnityAction<Event>>>();

    public void AddEventListener(string name, UnityAction<Event> action) {
      List<UnityAction<Event>> list;
      if (eventList.TryGetValue(name, out list)) {
        list.Add(action);
      } else {
        list = new List<UnityAction<Event>>();
        list.Add(action);
        eventList.Add(name, list);
      }
    }

    public void RemoveEventListener(string name, UnityAction<Event> action) {
      List<UnityAction<Event>> list;
      if (eventList.TryGetValue(name, out list)) {
        list.RemoveAll(item => item == action);
      }
    }

    public void DispatchEvent(Event evt) {
      List<UnityAction<Event>> list;
      if (eventList.TryGetValue(evt.name, out list)) {
        foreach (var item in list) {
          item.Invoke(evt);
        }
      }
    }
  }
}