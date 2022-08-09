using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigatorVehicle : MonoBehaviour
{
    public AIVehicleControl controller;
    private int direction;

    public Waypoint currentWaypoint;
    private bool Reverse=false;
    public bool Stop=false;
    public bool Brake=false;

    // Start is called before the first frame update
    void Start()
    {
        //controller = GetComponent<AIVehicleControl>();
        direction=0;
        controller.SetTarget(currentWaypoint.GetVehiclePosition(),Stop,Reverse,Brake);
    }

    // Update is called once per frame
    public void UpdatePosition()
    {
        if(direction==0)
        {
            if(currentWaypoint.Stop != null) 
            {
                //print("trajet retour");
                currentWaypoint = currentWaypoint.Stop;
                direction=1;
                Stop=true;
                Reverse=false;
                Brake=currentWaypoint.Brake;
            }
            else
            {
                //print("next");
                currentWaypoint = currentWaypoint.nextWaypoint;
                Stop=false;
                Reverse=false;
                Brake=currentWaypoint.Brake;
            }
        }
        else
        {
            if(currentWaypoint.Stop != null)
            {   
                //print("trajet alle");
                currentWaypoint = currentWaypoint.Stop;
                direction=0;
                Stop=true;
                Reverse=false;
                Brake=currentWaypoint.Brake;
            }
            else
            {
                if(currentWaypoint.Reverse != null)
                {   
                    //print("passage en reverse ok");
                    currentWaypoint = currentWaypoint.Reverse;
                    Stop=false;
                    Reverse=true;
                    Brake=currentWaypoint.Brake;
                }
                else
                {
                    //print("previous");
                    currentWaypoint = currentWaypoint.previousWaypoint;
                    Stop=false;
                    Reverse=false;
                    Brake=currentWaypoint.Brake;
                }
            }
        }
        controller.SetTarget(currentWaypoint.GetVehiclePosition(),Stop,Reverse,Brake);
    }
}
