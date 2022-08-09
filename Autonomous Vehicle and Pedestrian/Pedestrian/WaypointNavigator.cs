using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class WaypointNavigator : MonoBehaviour
{

    private AICharacterControl controller;
    private int direction;

    public Waypoint currentWaypoint;


    private void Awake()
    {
        controller = GetComponent<AICharacterControl>();

    }
    // Start is called before the first frame update
    void Start()
    {
        direction= Mathf.RoundToInt(Random.Range(0f,1f));
        controller.SetTarget(currentWaypoint.GetPosition());
    }

    // Update is called once per frame
    public void UpdatePosition()
    {
        bool shouldBranch = false;
        
        if(currentWaypoint.branches != null && currentWaypoint.branches.Count >0 )
        {
            shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRatio ? true : false;
        }
            
        if(shouldBranch)
        {
            currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count -1)];
        }
        else
        {
            if(direction==0)
            {
                if(currentWaypoint.nextWaypoint != null)
                {
                    currentWaypoint = currentWaypoint.nextWaypoint;
                }
                else
                {
                    currentWaypoint = currentWaypoint.previousWaypoint;
                    direction=1;                    
                }  
            }
            else
            {
                if(currentWaypoint.previousWaypoint != null)
                {
                    currentWaypoint = currentWaypoint.previousWaypoint;
                }
                else
                {
                    currentWaypoint = currentWaypoint.nextWaypoint;
                    direction=0;
                } 
            }
        }
        controller.SetTarget(currentWaypoint.GetPosition());
        
    }
}
