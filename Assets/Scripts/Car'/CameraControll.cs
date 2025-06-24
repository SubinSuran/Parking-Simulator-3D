using Unity.Cinemachine;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    

    public CinemachineCamera forwardCam;
    public CinemachineCamera reverseCam;

 
    

    void Start()
    {
        // On start, ensure the forward camera is the active one by default.
        // The camera with the highest priority is the one Cinemachine uses.
        SetForwardCameraActive();
    }

    void Update()
    {
        // Check if the '1' key was pressed on the top row
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Number 1 key was pressed!");
            SetForwardCameraActive();
        }
        // Check if the '2' key was pressed on the top row
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Number 2 key was pressed!");
            SetReverseCameraActive();
        }
    }

    void SetForwardCameraActive()
    {
        if (forwardCam != null && reverseCam != null)
        {
            forwardCam.Priority = 20;
            reverseCam.Priority = 10;
        }
    }

    void SetReverseCameraActive()
    {
        if (forwardCam != null && reverseCam != null)
        {
            reverseCam.Priority = 20;
            forwardCam.Priority = 10;
        }
    }
}
