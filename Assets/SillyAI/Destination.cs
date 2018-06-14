using UnityEngine;
using System.Linq;

namespace SillyAI {

  [AddComponentMenu("SillyAI/Destination"), DisallowMultipleComponent]
  public class Destination : MonoBehaviour {

    public Vector3 position {
      get { return transform.position; }
    }

    // void OnDrawGizmos() {
    //   // Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one) * Matrix4x4.Rotate(Quaternion.Euler(0, 0, 45));
    //   // Gizmos.DrawIcon(transform.position, "Destination.png", false);

    //   var mesh = new Mesh();
    //   mesh.vertices = new Vector3[] {
    //     new Vector3(-0.5f, 0, -0.5f),
    //     new Vector3(-0.5f, 0, 0.5f),
    //     new Vector3(0.5f, 0, 0.5f),
    //     new Vector3(0.5f, 0, -0.5f),
    //   };
    //   mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

    //   mesh.normals = Enumerable.Repeat(Vector3.forward, 4).ToArray();

    //   Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
    // }
  }

}