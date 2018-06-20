using UnityEngine;
using SillyAI;

public class MoveDestination : MonoBehaviour {

  public float delay = 5f;

  public AIBrain brain;

  private Vector3 lastLocation;

  void Awake() {
    lastLocation = transform.position;
    // InvokeRepeating("Move", delay, delay);
  }

  void Update() {
    if (lastLocation != transform.position) {
      // transform.position = new Vector3(Random.Range(-45, 45), 1, Random.Range(-45, 45));
      lastLocation = transform.position;
      brain.SetDestination(lastLocation);
    }
  }

}