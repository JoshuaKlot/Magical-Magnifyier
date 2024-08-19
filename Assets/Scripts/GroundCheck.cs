
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public Transform parentTransform; // Reference to the parent object's transform
    public Vector3 localOffset; // Offset from the parent's position to the ground check position

    void Update()
    {
        // Update the position of the GroundCheck to be at the parent's position plus the offset
        transform.position = parentTransform.position + parentTransform.rotation * localOffset;
    }
}
