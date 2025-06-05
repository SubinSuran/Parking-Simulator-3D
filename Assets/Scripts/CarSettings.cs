using UnityEngine;

[CreateAssetMenu(fileName = "Car", menuName = "Scriptable Objects/Car")]
public class CarSettings: ScriptableObject
{
    public float acceleration =1500f;
    public float brakePower = 3000f;
    public float maxSpeed = 88f;
    public AnimationCurve steeringCurve;
    public float slipAllowance = 0.5f;
    public Vector3 centerOfMass = new Vector3(0, -0.5f, 0);
    public float steeringClamp = 45f;
    public float smokeYOffset = 0.1f;

}
