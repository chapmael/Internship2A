using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint previousWaypoint;
    public Waypoint nextWaypoint;

    [Range(0f,5f)]
    public float width=2.5f;
    public List<Waypoint> branches;
    public Waypoint Reverse;
    public Waypoint Stop;
    public bool Brake=false;

    [Range(0f , 1f)]
    public float branchRatio = 0.5f; 

    public Vector3 GetPosition()
    {
        Vector3 minBound = transform.position + transform.right * width / 4f;
        Vector3 maxBound = transform.position - transform.right * width / 4f;

        return Vector3.Lerp(minBound,maxBound,Random.Range(0f,1f));

    }

    public Vector3 GetVehiclePosition()
    {
        return this.transform.position;
    }


}
