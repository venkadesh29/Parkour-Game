using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MainCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    public float rotationY;

    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //Get current camera state
        var state = vCam.State;

        //Extract the rotation of Quaternion from the state
        var rotation = state.FinalOrientation;

        //Convert the rotation to Euler angles
        var euler = rotation.eulerAngles;

        //Get Y-Axis value from Euler angle
        rotationY = euler.y;

        //Round the roatation valuie to nearest Int value
        var roundedRotationY = Mathf.RoundToInt(rotationY);
    }

    public Quaternion flatRotation => Quaternion.Euler(0 , rotationY , 0);
}
