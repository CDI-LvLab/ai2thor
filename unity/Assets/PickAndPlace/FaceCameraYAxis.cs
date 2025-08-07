using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCameraYAxis : MonoBehaviour
{
    void Update()
    {
        if (Camera.main == null) return;

        // Get direction to camera, ignoring vertical difference
        Vector3 direction = Camera.main.transform.position - transform.position;
        direction.y = 0f; // Ignore Y-axis for horizontal-only rotation

        if (direction.sqrMagnitude > 0.001f) // Avoid zero-length vector
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }
}
