using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace Enumeration{


public class LidarEnumColorObj : MonoBehaviour
{
/**** Initialization ****/

void Start()
{
    this.transform.rotation=m_objectParent.transform.rotation;                                                                      // acquire axis of the car and apply to the sphere to init the position 
    m_iVert=(int)Mathf.Round((m_maxVerticalAngle-m_minVerticalAngle)/m_stepAngleV);                                                 
    
    /* Initialization for saving Hit */
    m_nbLineTab=(int)Mathf.Round(360/m_stepAngleH);
    m_MaxCurrentHorizontalPolsitionAngle=(int)Mathf.Round(360/m_AnglePerFrame);                                                     // init the value (look at declaration for the utility )
    m_CurrentStartHorizontalAngle=0;                                                                                                // position forward = index 0

    /*Setting for rawimage*/
    m_lastImage=new Texture2D(m_nbLineTab,m_iVert,TextureFormat.RGBA32,false);
    //m_imageRendered=false;

    m_rawImage.texture=m_lastImage;
    m_offsetImage=(int)Mathf.Round(m_nbLineTab/2)+1;

    m_dataColor=new EnumStruct();

    m_savetest=new SavePoint("premiertest.txt");
    
}



/****************************************************************************************************************************************************************/
/* Rotation of the sensor from the end of the previous position by the defined angle m_AnglePerFrame and recording of collisions*/

private void SensorRotation(){
    m_iVert=(int)Mathf.Round((m_maxVerticalAngle-m_minVerticalAngle)/m_stepAngleV);                                                     // Update the number of Vertical measurement if the user modify init parameters
    m_iHoriz=(int)Mathf.Round(m_AnglePerFrame/m_stepAngleH);                                                                            // Update the number of Horizontal measurement if the user modify init parameters
    
    for(int k=0;k<m_iHoriz;k++){
        this.transform.Rotate(Vector3.up,m_stepAngleH);
        int _horizontalIndex=(m_CurrentStartHorizontalAngle)*m_iHoriz+k;                                                                                 // Rotation of the sensor around the local vertical axis, relative to the orientation of the car 
        for(int i=0;i<m_iVert;i++){
            float _dist;                                                                                                                 
            m_lookdirection= Quaternion.AngleAxis(-m_minVerticalAngle-i*m_stepAngleV, this.transform.right)*this.transform.forward;     // Rotation of the direction vector around x-Axis to set the vertical orientation of the ray
            Vector3 _positionSensor=this.transform.position;                                                                            // Get the position of the sensor
            _positionSensor.y+=m_offsetHighSensor;                                                                                      // Ad the vertical offset to be enough higher
            
            RaycastHit _hit;                                                                                                            // Create a structure for collision ray
            Ray _ray =new Ray(_positionSensor,m_lookdirection);                                                                         // Set the direction of the ray
            bool _BoolCollision =Physics.Raycast(_ray,out _hit,m_MeasurementRange);
            if (_BoolCollision){                                                                                                        // Check if there is a collision

                _dist=_hit.distance;                                                                                                    // Get the distance between the car and the collision point 
                //_dist=Mathf.Clamp(_dist,0,m_distanceAccuracy);                                                                        // Set the accuracy for the distance measurement 
                //Debug.DrawLine(_positionSensor, _hit.point, Color.green);                                                               // Draw the ray in green if there is a collision

                //Debug.DrawLine(_hit.point - Vector3.up* 0.3f, _hit.point + Vector3.up * 0.3f, Color.red, 0, false);                     // Draw the collision point in red
                //Debug.DrawLine(_hit.point - Vector3.left* 0.3f, _hit.point + Vector3.left * 0.3f, Color.red, 0, false);                 // Draw the collision point in red
                //Debug.DrawLine(_hit.point - Vector3.forward*0.3f, _hit.point + Vector3.forward * 0.3f, Color.red, 0, false);            // Draw the collision point in red
                
            }   
            else
            {
                _dist = m_MeasurementRange;                                                                                             // Set the distance to the max distance detection
                //Debug.DrawRay(_positionSensor, m_lookdirection*m_MeasurementRange, Color.gray);                                         // Draw the ray in grey 
            }
            
            /*get the distance*/
            float _distance=_dist;                                                                                                      // Get the distance information from the m_tabRayCastHit

            /*calculation of the intensity relative to the distance*/
            float _NormalizedDistance =_distance/m_MeasurementRange;                                                                    // Calculation of the normalized distance from the maximum detection distance
            float _val=1-Mathf.Exp(-m_ajustColorParameter*Mathf.Pow(_NormalizedDistance,4f));
            Color _color;
            if(_BoolCollision)
            {   
                string _nameObj=_hit.collider.gameObject.name;
                _color=m_dataColor.GetColorFromString(_nameObj);
                float _H,_S,_V;
                Color.RGBToHSV(_color,out _H, out _S, out _V);
                _V*=_val;
                _color=Color.HSVToRGB(_H,_S,_V);
                _color.a=1;

                //m_savetest.SetAttribut(_hit.point.x,_hit.point.y,_hit.point.z,Time.time,_color);
                //m_savetest.SaveAll();
            }
            else{
                _color=new Color(1,1,1,1);
            }

            m_lastImage.SetPixel(_horizontalIndex+m_offsetImage,i,_color);      // update the texture apply to the image with an offset in order to have the front of the car at the center
        }
    }
    m_lastImage.Apply();

    
}


/****************************************************************************************************************************************************************/
/* This function is called once per frame */

void Update(){
    /* update the information of hit for m_anglePerFram */
    SensorRotation();

    /* Update of How many turn of m_AnglePerFrame we have already done */
    if( m_CurrentStartHorizontalAngle==m_MaxCurrentHorizontalPolsitionAngle-1){
        m_CurrentStartHorizontalAngle=0;            // if we did 360deg, we reset the counter
    }
    else{
        m_CurrentStartHorizontalAngle+=1;           // else +1
    }

    /* update of the image rendering */
}



/****************************************************************************************************************************************************************/
/* Set by the user in unity */

public float m_stepAngleV;                          // step for vertical angle
public float m_stepAngleH;                          // step for horizontal angle
public int m_maxVerticalAngle;                      // max value for vertical angle
public int m_minVerticalAngle;                      // min value for vertical angle
public Transform m_objectParent;                    // object with the lidar
public float m_offsetHighSensor;                    // set the vertical offset for the position of the Lidar
public float m_distanceAccuracy;                    // precision for the measurement of the distance
public float m_MeasurementRange;                    // max of detection measurement 
public int m_AnglePerFrame;                         // defines the angle of the image to be analyzed per frame

public RawImage m_rawImage;                         // Display of what the Lidar sees
[Range(0f,15f)]                                    // Ajust the parameter to calculate the intensity of each color acoording to the distance
public float m_ajustColorParameter;
private int m_offsetImage;                          // Offset position of the image on the screen
private Texture2D m_lastImage=null;                 // The texture to apply on the raw image to see somethink
//private bool m_imageRendered;


private Vector3 m_lookdirection;                    // direction for ray
private int m_iVert,m_iHoriz;                       // Calculate at each frame how many vertical/Horizontal step we have to do according to m_AnglePerFrame and m_stepAngleV/H

private int m_nbLineTab;                            // Storage of how many Horizontal information we have to save 
private int m_CurrentStartHorizontalAngle;          // To know which part of the image has to be update
private int m_MaxCurrentHorizontalPolsitionAngle;   // Set at the beginning. Represent how many frame to make 360 deg


private EnumStruct m_dataColor;
private SavePoint m_savetest;

}
}