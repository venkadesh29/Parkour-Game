using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentChecker : MonoBehaviour
{
    public Vector3 rayOffset = new Vector3(0,0.2f,0);
    public float rayLength = 0.9f;
    public LayerMask obstacleLayer;

    public void checkObstacle()
    {
        var rayOrigin = transform.position + rayOffset;
        bool hitFound = Physics.Raycast(rayOrigin, transform.forward, out RaycastHit hitInfo, rayLength, obstacleLayer);

        Debug.DrawRay(rayOrigin, transform.forward * rayLength, (hitFound) ? Color.red : Color.green);
    }
}
