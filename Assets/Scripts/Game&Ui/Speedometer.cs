using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    public Rigidbody target;
    public float maxSpeed = 0.0f; // The maximum speed of the target IN KM/H

    public float minSpeedArrowAngle;
    public float maxSpeedArrowAngle;

    [Header("UI")]
    public TextMeshProUGUI speedLabel; // The label that displays the speed
    public RectTransform arrow; // The arrow in the speedometer
    public TextMeshProUGUI gearLabel; // The label for the gear display

    private float speed = 0.0f;
    private CarController carController;

    private void Start()
    {
        if (target != null)
        {
            carController = target.GetComponent<CarController>();
        }
    }

    private void Update()
    {
        speed = target.linearVelocity.magnitude * 3.6f;

        if (speedLabel != null)
            speedLabel.text = ((int)speed) + " km/h";

        if (arrow != null)
            arrow.localEulerAngles =
                new Vector3(0, 0, Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, speed / maxSpeed));

        CheckGear();
    }

    public void CheckGear()
    {
        if (carController != null && gearLabel != null)
        {
            string gearText;

            switch (carController.gear)
            {
                case CarController.GearState.Drive:
                    gearText = "GEAR: D";
                    break;
                case CarController.GearState.Neutral:
                    gearText = "GEAR: N";
                    break;
                case CarController.GearState.Reverse:
                    gearText = "GEAR: R";
                    break;
                default:
                    gearText = "?";
                    break;
            }

            gearLabel.text = gearText;
        }
    }
}