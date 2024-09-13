using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    [Header ("Player Movement")]
    public float movementSpeed = 5f;
    public MainCameraController mainCamera;
    public EnvironmentChecker environmentChecker;
    public float rotSpeed = 600f;
    Quaternion requiredRotation;
    bool playerControl = true;
    public bool playerInAction{get; private set;}

    [Header ("Player Animator")]
    public Animator animator;

    [Header ("Player Collision and Gravity")]
    public CharacterController cc;
    public float surfaceCheckRadius = 0.3f;
    public Vector3 surfaceCheckOffset;
    public LayerMask surfaceLayer;
    bool onSurface;
    public bool playerOnLedge {get; set;}
    public bool playerHanging{get; set;}
    public LedgeInfo LedgeInfo {get; set;}
    [SerializeField] float fallingSpeed;
    [SerializeField]Vector3 moveDir;
    [SerializeField]Vector3 requiredMoveDir;
    Vector3 velocity;

    private void Update()
    {
        PlayerMovement();

        if(!playerControl)
            return;

        if(playerHanging)
            return;

        velocity = Vector3.zero;
        
        if(onSurface)
        {
            fallingSpeed = 0f;
            velocity = moveDir * movementSpeed;

            playerOnLedge = environmentChecker.CheckLedge(moveDir, out LedgeInfo ledgeInfo);

            if(playerOnLedge)
            {
                LedgeInfo = ledgeInfo;
                playerLedgeMovement();
                Debug.Log("PLayer is on ledge");
            }

            animator.SetFloat("Movement Value", velocity.magnitude / movementSpeed, 0.2f, Time.deltaTime);

        }
        else
        {
            fallingSpeed += Physics.gravity.y*Time.deltaTime; 

            velocity = transform.forward * movementSpeed / 2;
        }

        velocity.y = fallingSpeed;

        surfaceCheck();
        animator.SetBool("onSurface" , onSurface);
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

       requiredMoveDir = mainCamera.flatRotation * movementInput;

//if keys pressed, move and rotate the player
        cc.Move (velocity * Time.deltaTime);

        if(movementAmount > 0 &&moveDir.magnitude > 0.2f)
        {
            requiredRotation = Quaternion.LookRotation(moveDir);
        }

        moveDir = requiredMoveDir;

        transform.rotation = Quaternion.RotateTowards(transform.rotation , requiredRotation , rotSpeed * Time.deltaTime);

    }

    void surfaceCheck()
    {
        onSurface = Physics.CheckSphere(transform.TransformPoint(surfaceCheckOffset), surfaceCheckRadius, surfaceLayer);
    }

    void playerLedgeMovement()
    {
        float angle = Vector3.Angle(LedgeInfo.surfaceHit.normal, requiredMoveDir);

        if(angle < 90)
        {
            velocity = Vector3.zero;
            moveDir = Vector3.zero;

        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(surfaceCheckOffset), surfaceCheckRadius);
    }

    public IEnumerator PerformAction(string AnimationName, CompareTargetParameter ctp, Quaternion RequiredRotation, 
    bool LookAtObstacle = false, float ParkourActionDelay = 0f)
    {
        playerInAction = true;

        animator.CrossFade(AnimationName, 0.2f);
        yield return null;

        var animationState = animator.GetNextAnimatorStateInfo(0); 
        if(!animationState.IsName(AnimationName))
            Debug.Log("Animation Name is Incorrect");

        float timerCounter =0f;

        while(timerCounter <= animationState.length)
        {
            timerCounter += Time.deltaTime;

            //Make player look towards the obstacle
            if(LookAtObstacle)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, RequiredRotation, rotSpeed * Time.deltaTime);
            }

            if(ctp != null)
            {
                CompareTarget(ctp);
            }

            if(animator.IsInTransition(0) && timerCounter > 0.5f)
            {
                break;
            }

            yield return null;
        }
        
        yield return new WaitForSeconds(ParkourActionDelay);
        playerInAction = false;
    }

    void CompareTarget(CompareTargetParameter compareTargetParameter)
    {
        animator.MatchTarget(compareTargetParameter.position, transform.rotation, compareTargetParameter.bodyPart, new MatchTargetWeightMask(compareTargetParameter.positionWeight, 0), compareTargetParameter.startTime, compareTargetParameter.endTime);
    }


    public void SetControl(bool hasControl)
    {
        this.playerControl = hasControl;
        cc.enabled = hasControl;

        if(!hasControl)
        {
            animator.SetFloat("movement Value" , 0f);
            requiredRotation = transform.rotation;
        }
    }

    public bool HasPlayerControl
    {
        get => playerControl;
        set => playerControl = value;
    }
}

public class CompareTargetParameter
{
    public Vector3 position;
    public AvatarTarget bodyPart;
    public Vector3 positionWeight;
    public float startTime;
    public float endTime;
}
