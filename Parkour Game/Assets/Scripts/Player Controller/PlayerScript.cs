using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    [Header ("Player Movement")]
    public float movementSpeed = 5f;
    public MainCameraController mainCamera;
    public float rotSpeed = 600f;
    Quaternion requiredRotation;

    [Header ("Player Animator")]
    public Animator animator;

    [Header ("Player Collision and Gravity")]
    public CharacterController cc;
    public float surfaceCheckRadius = 0.3f;
    public Vector3 surfaceCheckOffset;
    public LayerMask surfaceLayer;
    bool onSurface;

    private void Update()
    {
        PlayerMovement();
        surfaceCheck();
        Debug.Log("Player on surface" + onSurface);
    }

    void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical"); 
//check if the key is pressed
//Clamp01 value from 0 to 1 or inbetween
        float movementAmount  = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        var movementInput = (new Vector3(horizontal, 0 , vertical)).normalized;

        var movementDirection = mainCamera.flatRotation * movementInput;
//if keys pressed, move and rotate the player
        if(movementAmount > 0)
        {
            cc.Move (movementDirection * movementSpeed * Time.deltaTime);
            requiredRotation = Quaternion.LookRotation(movementDirection);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation , requiredRotation , rotSpeed * Time.deltaTime);

        animator.SetFloat("Movement Value", movementAmount, 0.2f, Time.deltaTime);
    }

    void surfaceCheck()
    {
        onSurface = Physics.CheckSphere(transform.TransformPoint(surfaceCheckOffset), surfaceCheckRadius, surfaceLayer);
    }

    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(surfaceCheckOffset), surfaceCheckRadius);
    }
}
