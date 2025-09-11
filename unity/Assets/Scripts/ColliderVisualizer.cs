using UnityEngine;

[ExecuteInEditMode] // draw in edit mode too
public class ColliderVisualizer : MonoBehaviour {
    public bool DrawTriggers = true;
    public bool DrawStatics = true;
    public bool DrawKinematics = true;
    public bool DrawDynamics = true;

    void OnDrawGizmos() {
        Collider[] colliders = FindObjectsOfType<Collider>();

        foreach (var col in colliders) {
            if (!col.enabled || !col.gameObject.activeInHierarchy) {
                continue;
            }

            // Decide color:
            // 🔵 Dynamic rigidbody
            // 🟡 Kinematic rigidbody
            // 🟢 Static collider
            // 🔴 Trigger collider
            Color c = Color.green;
            Rigidbody rb = col.attachedRigidbody;
            if (col.isTrigger) {
                if (!DrawTriggers) {
                    continue;
                }
                c = Color.red;
            } else if (rb != null && !rb.isKinematic) {
                if (!DrawDynamics) {
                    continue;
                }
                c = Color.blue;
            } else if (rb != null && rb.isKinematic) {
                if (!DrawKinematics) {
                    continue;
                }
                c = Color.yellow;
            } else {
                if (!DrawStatics) {
                    continue;
                }
                c = Color.green;
            }

            Gizmos.color = c;
            Gizmos.matrix = col.transform.localToWorldMatrix;

            // Draw shape
            if (col is BoxCollider box) {
                Gizmos.DrawWireCube(box.center, box.size);
            } else if (col is SphereCollider sphere) {
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            } else if (col is CapsuleCollider capsule) {
                // Approximate capsules by drawing spheres + a line
                Vector3 p1 = capsule.center;
                Vector3 p2 = capsule.center;
                float halfHeight = Mathf.Max(capsule.height / 2f - capsule.radius, 0f);

                switch (capsule.direction) {
                    case 0: // X-axis
                        p1.x -= halfHeight;
                        p2.x += halfHeight;
                        break;
                    case 1: // Y-axis
                        p1.y -= halfHeight;
                        p2.y += halfHeight;
                        break;
                    case 2: // Z-axis
                        p1.z -= halfHeight;
                        p2.z += halfHeight;
                        break;
                }

                Gizmos.DrawWireSphere(p1, capsule.radius);
                Gizmos.DrawWireSphere(p2, capsule.radius);
                Gizmos.DrawLine(p1, p2);
            } else if (col is MeshCollider meshCol && meshCol.sharedMesh != null) {
                Gizmos.DrawWireMesh(meshCol.sharedMesh, meshCol.transform.position, meshCol.transform.rotation, meshCol.transform.lossyScale);
            }
        }
    }
}
