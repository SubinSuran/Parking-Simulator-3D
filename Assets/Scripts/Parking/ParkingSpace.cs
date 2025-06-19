using UnityEngine;
using System;

public class ParkingSpace : MonoBehaviour
{
    [Tooltip("How long the car must be stationary inside the trigger to win.")]
    public float timeToPark = 2f;
    [Tooltip("The maximum speed the car can have to be considered 'stopped'.")]
    public float maxSpeedThreshold = 0.1f;

    public static event Action OnParkedSuccess;

    private float stationaryTimer = 0f;
    private bool isCarInZone = false;
    private Rigidbody carRigidbody;
    private bool hasWon = false;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isCarInZone = true;
            carRigidbody = other.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (hasWon) return;

        if (isCarInZone && carRigidbody != null)
        {
            if (carRigidbody.linearVelocity.magnitude < maxSpeedThreshold)
            {
                stationaryTimer += Time.deltaTime;
                if (stationaryTimer >= timeToPark)
                {
                    hasWon = true;
                    Debug.Log("PARKED SUCCESSFULLY!");
                    OnParkedSuccess?.Invoke();
                }
            }
            else
            {
                stationaryTimer = 0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isCarInZone = false;
            carRigidbody = null;
            stationaryTimer = 0f;
        }
    }
}