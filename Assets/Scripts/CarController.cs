using UnityEngine;

public class CarController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Wheel Colliders")]
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider RRWheel;
    public WheelCollider RLWheel;

    [Header("Wheel Meshes")]
    public Transform FRMesh;
    public Transform FLMesh;
    public Transform RRMesh;
    public Transform RLMesh;

    [Header("Car Settings")]

    public CarSettings settings;
    //public float acceleration = 1500f;
    //public float brakePower = 3000f;
    //public float maxSteerAngle = 30f;
    //public float steerSpeed = 5f;
    //public float maxSpeed = 100f;
    //public Vector3 centerOfMass;

    [Header("Drift Settings")]
    public float handbrakeTorque = 4000f;
    public float driftStiffness = 0.5f; // 0.5 = more slip
    private bool isDrifting = false;
    private float originalSideFriction;


    private float steerInput;
    private float gasInput;
    private float brakeInput;
    private float currentSteer;

    public enum GearState { Reverse, Neutral, Drive }
    public GearState gear = GearState.Neutral;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = settings.centerOfMass;

        // Cache original sideways friction
        originalSideFriction = RRWheel.sidewaysFriction.stiffness;
    }


    void Update()
    {
        GetInputs();
        isDrifting = Input.GetKey(KeyCode.Space);

    }

    void FixedUpdate()
    {
        ApplySteering();
        ApplyMotor();
        ApplyBrakes();
        AnimateWheels();
        ApplyHandbrakeDrift();

    }

    void GetInputs()
    {
        // Gear shifting
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            gear = gear == GearState.Reverse ? GearState.Neutral : GearState.Drive;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            gear = gear == GearState.Drive ? GearState.Neutral : GearState.Reverse;
        }

        // Inputs
        gasInput = Input.GetKey(KeyCode.W) ? 1f : 0f;
        brakeInput = Input.GetKey(KeyCode.S) ? 1f : 0f;
        steerInput = Input.GetAxis("Horizontal");
    }

    void ApplyMotor()
    {
        float speedKmh = rb.linearVelocity.magnitude * 3.6f;

        if (gasInput > 0f && speedKmh < settings.maxSpeed)
        {
            float motorTorque = gasInput * settings.acceleration;
            motorTorque *= (gear == GearState.Drive) ? 1f : (gear == GearState.Reverse) ? -1f : 0f;

            RRWheel.motorTorque = motorTorque;
            RLWheel.motorTorque = motorTorque;
        }
        else
        {
            RRWheel.motorTorque = 0f;
            RLWheel.motorTorque = 0f;
        }
    }

    void ApplyBrakes()
    {
        float brakeTorque = brakeInput * settings.brakePower;

        FRWheel.brakeTorque = brakeTorque;
        FLWheel.brakeTorque = brakeTorque;
        RRWheel.brakeTorque = brakeTorque;
        RLWheel.brakeTorque = brakeTorque;

        if (brakeInput == 0 && gasInput == 0)
        {
            RRWheel.brakeTorque = settings.brakePower * 0.1f;
            RLWheel.brakeTorque = settings.brakePower * 0.1f;
        }
    }
    void ApplyHandbrakeDrift()
    {
        if (isDrifting && rb.linearVelocity.magnitude > 5f) // only drift if car is moving
        {
            WheelFrictionCurve driftFriction = RRWheel.sidewaysFriction;
            driftFriction.stiffness = driftStiffness;

            RLWheel.brakeTorque = handbrakeTorque;
            RRWheel.brakeTorque = handbrakeTorque;

            RLWheel.sidewaysFriction = driftFriction;
            RRWheel.sidewaysFriction = driftFriction;
        }
        else
        {
            WheelFrictionCurve normalFriction = RRWheel.sidewaysFriction;
            normalFriction.stiffness = originalSideFriction;

            RLWheel.sidewaysFriction = normalFriction;
            RRWheel.sidewaysFriction = normalFriction;
        }
    }

    // In CarController.cs

    void ApplySteering()
    {
        // 1. Calculate the current speed as a value between 0 and 1
        float speedFactor = rb.linearVelocity.magnitude / settings.maxSpeed;

        // 2. Use that value to find the perfect steering angle between min and max
        // At 0 speed, it will be maxSteeringAngle. At maxSpeed, it will be minSteeringAngle.
        float currentMaxSteer = Mathf.Lerp(settings.maxSteeringAngle, settings.minSteeringAngle, speedFactor);

        // 3. Apply the steering input
        float targetSteer = steerInput * currentMaxSteer;
        currentSteer = Mathf.Lerp(currentSteer, targetSteer, Time.deltaTime * settings.steerSpeed);

        FRWheel.steerAngle = currentSteer;
        FLWheel.steerAngle = currentSteer;
    }

    void AnimateWheels()
    {
        UpdateWheelPose(FRWheel, FRMesh);
        UpdateWheelPose(FLWheel, FLMesh);
        UpdateWheelPose(RRWheel, RRMesh);
        UpdateWheelPose(RLWheel, RLMesh);
    }

    void UpdateWheelPose(WheelCollider collider, Transform mesh)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);

        mesh.position = pos;
        mesh.rotation = rot;
    }
}
