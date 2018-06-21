using UnityEngine;

public class SpawnerWonderer : MonoBehaviour {

  public GameObject group;
  public GameObject groupFollower;
  public Transform parent;

  void Start() {
    InvokeRepeating("CreateGroup", 0, 0.5f);
  }

  void CreateGroup() {
    var position = new Vector3(Random.Range(-45, 45), 1, Random.Range(-45, 45));
    var grp = Instantiate(group, position, Quaternion.identity) as GameObject;

    var groupSize = Random.Range(2, 9);
    for (var i = 0; i < groupSize; i++) {
      var follower = Instantiate(groupFollower, grp.transform.position, grp.transform.rotation) as GameObject;
      follower.transform.SetParent(grp.transform);
    }
    grp.transform.parent = parent;
  }

}