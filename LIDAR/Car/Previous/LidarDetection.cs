using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidarDetection : MonoBehaviour
{
/**** Initialization ****/

void Start()
{
    this.transform.rotation=m_objectParent.transform.rotation;      // acquire axis of the car and apply to the sphere
    m_TabCoordonate=new Vector3[m_maxHorizontalAngle*2][];          // Initialization of the coordonate tab
    m_TabDistance= new float[m_maxHorizontalAngle*2][];             // Initialization of the distance tab
}


/**** Update rotation ****/
private void UpdateRotation(int ihoriz, int ivert){
    this.transform.rotation=m_objectParent.transform.rotation;      // acquire axis of the car and apply to the sphere
    m_lookdirection= Quaternion.AngleAxis(-m_maxHorizontalAngle+ihoriz*m_stepAngleH, this.transform.up)*this.transform.forward;     // set the new Horizontal position
    m_lookdirection= Quaternion.AngleAxis(-m_minVerticalAngle-ivert*m_stepAngleV, this.transform.right)*m_lookdirection;            // set the new Vertical position
    m_lookdirection=m_lookdirection.normalized;
}


/**** Save the distance ****/
private void SaveDistanceCoordonate(RaycastHit hit,int iVertical, int iHorizontal){
    m_TabDistance[iHorizontal][iVertical]=hit.distance;     
    //m_TabCoordonate[iHorizontal][iVertical]=hit.point;
}



/**** Find a collision point and save all the coordonate ****/
private void FindCollision()
{   
    
    for(int i=0;i<99;i++){  
        
        for(int k=0;k<22;k++){
            UpdateRotation(i,k);                                                    // update the direction of the new ray 

            Debug.DrawRay(this.transform.position,m_lookdirection*5,Color.red);     // print the future ray in red 
            RaycastHit _hit;                                                        // create a structure for collision ray
            Ray _ray =new Ray(this.transform.position,m_lookdirection);             // create a ray at the sphere position in the new direction.
            //bool _detectedObj=Physics.Raycast(_ray,out _hit,Mathf.Infinity);
            //SaveDistanceCoordonate(_hit,k,i,_detectedObj);

            if (Physics.Raycast(_ray,out _hit,Mathf.Infinity))                      // looking for collision
            { 
                print("distance "+_hit.distance+"   Coordonate "+_hit.point);


                /*bug, no object reference, solution maybe create a tab with all _hit and after create the tab of distance*/
                //SaveDistanceCoordonate(_hit,k,i);                                 // update distance and coordonate tab with new value
                //m_TabDistance[i][k]=distance;
            }

        }
    }
   
}



/**** Transform the distance into a color ****/

void Update(){
    FindCollision();
}



private Vector3[][] m_TabCoordonate;    // coordonate tab
private float[][] m_TabDistance;        // distance tab

/*set by the user in unity*/
public float m_stepAngleV;              // step for vertical angle
public float m_stepAngleH;              // step for horizontal angle
public int m_maxVerticalAngle;          // max value for vertical angle
public int m_minVerticalAngle;          // min value for vertical angle
public int m_maxHorizontalAngle;        // max value for horizontal angle
public int m_maxdistanceDetection;      // max for distance detection  

public Transform m_objectParent;        // object with the lidar 
private Vector3 m_lookdirection;        // direction for ray 

}
