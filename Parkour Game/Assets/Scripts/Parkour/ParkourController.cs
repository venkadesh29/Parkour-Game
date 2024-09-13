using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    public EnvironmentChecker environmentChecker;
    public Animator animator;
    public playerScript playerScript;
    [SerializeField] NewParkourAction jumpDownParkourAction;

    [Header("Parkour Action Area")]
    public List<NewParkourAction> newParkourAction;

    void Update()
    {
        if(Input.GetButton("Jump") && !playerScript.playerInAction)
        {    
            var hitData = environmentChecker.checkObstacle();

            if(hitData.hitFound)
            {
                foreach(var action in newParkourAction)
                {
                    if(action.CheckIfAvailable(hitData, transform))
                    {
                        //perform parkour Action
                        StartCoroutine(PerformParkourAction(action));
                        break; 
                    }
                }
            }
        }

        if(playerScript.playerOnLedge && !playerScript.playerInAction && Input.GetButtonDown("Jump"))
        {
            if(playerScript.LedgeInfo.angle <= 50)
            {
                playerScript.playerOnLedge = false;
                StartCoroutine(PerformParkourAction(jumpDownParkourAction));
            }
        }
    }

    IEnumerator PerformParkourAction(NewParkourAction action)
    {
        playerScript.SetControl(false);

        CompareTargetParameter compareTargetParameter = null;
        if(action.AllowTargetMatching)
        {
            compareTargetParameter = new CompareTargetParameter()
            {
                position = action.ComparePosition,
                bodyPart = action.CompareBodyPart,
                positionWeight = action.ComparePositionWeight,
                startTime = action.CompareStartTime,
                endTime = action.CompareEndTime,
            };
        }

        yield return playerScript.PerformAction(action.AnimationName, compareTargetParameter, action.RequiredRotation, action.LookAtObstacle, action.ParkourActionDelay);

        playerScript.SetControl(true);
    }

    void CompareTarget(NewParkourAction action)
    {
        animator.MatchTarget(action.ComparePosition, transform.rotation, action.CompareBodyPart, new MatchTargetWeightMask(action.ComparePositionWeight, 0), action.CompareStartTime, action.CompareEndTime);
    }
}