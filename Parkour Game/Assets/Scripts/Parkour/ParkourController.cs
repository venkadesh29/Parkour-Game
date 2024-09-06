using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    public EnvironmentChecker environmentChecker;

    
    void Update()
    {
        environmentChecker.checkObstacle();    
    }
}
