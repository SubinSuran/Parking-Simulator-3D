using UnityEngine;

public class RotateWithMouse : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The object we want to rotate.")]
    public Transform targetToRotate;

    [Header("Rotation Speeds")]
    [Tooltip("How fast the object rotates on its own.")]
    public float autoRotationSpeed = 2.0f;

    [Tooltip("How much mouse movement affects rotation when dragging.")]
    public float manualRotationSpeed = 10.0f;

    [Header("Dampening")]
    [Tooltip("How quickly the manual rotation slows down after letting go. Higher is faster.")]
    public float inertiaDampening = 5.0f;

    // Private variable to store the current rotational velocity from the mouse drag
    private float currentManualVelocity = 0f;

    void Update()
    {
        if (targetToRotate == null)
        {
            return;
        }

        float rotationThisFrame;

        // --- Check if we are in Manual Mode or Auto Mode ---
        if (Input.GetMouseButton(0))
        {
            // --- MANUAL DRAGGING MODE ---
            // The player is in control.

            // Get mouse input and set the velocity for this frame
            float mouseX = Input.GetAxis("Mouse X");
            currentManualVelocity = -mouseX * manualRotationSpeed;

            // The rotation for this frame is ONLY the manual rotation.
            rotationThisFrame = currentManualVelocity;
        }
        else
        {
            // --- AUTO-ROTATION & INERTIA MODE ---
            // The player has let go.

            // Smoothly dampen the leftover velocity from the manual drag.
            currentManualVelocity = Mathf.Lerp(currentManualVelocity, 0, inertiaDampening * Time.deltaTime);

            // The rotation for this frame is the base auto-rotation plus the fading inertia.
            rotationThisFrame = autoRotationSpeed + currentManualVelocity;
        }

        // --- Apply the final, calculated rotation ---
        targetToRotate.Rotate(Vector3.up, rotationThisFrame * Time.deltaTime, Space.World);
    }
}