using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingController : MonoBehaviour
{
    EnvironmentChecker ec;
    public playerScript playerScript;
    void Awake()
    {
        ec= GetComponent<EnvironmentChecker>();
    }

    void Update()
    {
        if(!playerScript.playerHanging)
        {
            if(Input.GetButton("Jump") && !playerScript.playerInAction)
            {
                if(ec.CheckClimbing(transform.forward, out RaycastHit climbInfo))
                {
                    playerScript.SetControl(false);
                    StartCoroutine(ClimbToLedge("Idle To climb", climbInfo.transform, 0.40f, 54f));
                }
            }
        }
        else
        {
            //Ledge to Parkour action
        }
    }

    IEnumerator ClimbToLedge(string animationName, Transform ledgePoint, float compareStartTime, float compareEndTime)
    {
        var comapreParams = new CompareTargetParameter()
        {
            position = ledgePoint.position,
            bodyPart = AvatarTarget.RightHand,
            positionWeight = Vector3.one,
            startTime = compareStartTime,
            endTime = compareEndTime,
        };

        var reqiredRot = Quaternion.LookRotation(-ledgePoint.forward);

        yield return playerScript.PerformAction(animationName, comapreParams, reqiredRot, true);

        playerScript.playerHanging = true;
    }
}
