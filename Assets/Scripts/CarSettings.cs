// In CarSettings.cs

using UnityEngine;

[CreateAssetMenu(fileName = "Car", menuName = "Scriptable Objects/Car")]
public class CarSettings : ScriptableObject
{
    public float acceleration = 1500f;
    public float brakePower = 3000f;
    public float maxSpeed = 88f;

    [Header("Steering")]
    public float maxSteeringAngle = 30f; // The steering angle at 0 speed
    public float minSteeringAngle = 10f; // The steering angle at max speed
    public float steerSpeed = 5f; // How fast the wheels turn

    public float slipAllowance = 0.5f;
    public Vector3 centerOfMass = new Vector3(0, -0.5f, 0);
    public float smokeYOffset = 0.1f;
}