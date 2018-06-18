using UnityEngine;
using UnityEngine.UI;
using SillyAI;
public class Spawner : MonoBehaviour {

  public GameObject player;
  public Transform parent;
  public AIWaypoints waypoints;
  public Text text;

  public float interval;
  public int spawnCount = 1;

  private int units = 0;

  void Start() {
    InvokeRepeating("Spawn", 0f, interval);
  }

  void Spawn() {
    for (var i = 0; i < spawnCount; i++) {
      var go = Instantiate(player, new Vector3(Random.Range(-50, 50), 1, Random.Range(-50, 50)), Quaternion.identity) as GameObject;
      go.transform.parent = parent;
      go.GetComponent<AITraverseWaypoints>().waypointGroup = waypoints;
      text.text = "Units: " + ++units;
    }
  }
}