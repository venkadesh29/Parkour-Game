using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    public EnvironmentChecker environmentChecker;
    bool playerInAction;
    public Animator animator;

    [Header("Parkour Action Area")]
    public List<NewParkourAction> newParkourAction;

    void Update()
    {
        if(Input.GetButton("Jump") && !playerInAction)
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
    }

    IEnumerator PerformParkourAction(NewParkourAction action)
    {
        playerInAction = true;

        animator.CrossFade(action.AnimationName, 0.2f);
        yield return null;

        var animationState = animator.GetNextAnimatorStateInfo(0); 
        if(!animationState.IsName(action.AnimationName))
            Debug.Log("Animation Name is Incorrect");

        yield return new WaitForSeconds(animationState.length);

        playerInAction = false;
    }
}
