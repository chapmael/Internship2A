using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LidarDetectionRotation : MonoBehaviour
{
/**** Initialization ****/

void Start()
{
    this.transform.rotation=m_objectParent.transform.rotation;                                                                      // acquire axis of the car and apply to the sphere to init the position 
    m_iVert=(int)Mathf.Round((m_maxVerticalAngle-m_minVerticalAngle)/m_stepAngleV);                                                 
    
    /* Initialization for saving Hit */
    m_nbLineTab=(int)Mathf.Round(360/m_stepAngleH);
    m_tabRayCastHit=new RaycastHit[m_iVert,m_nbLineTab];                                                                            // tab with m_iVert column and m_nb... line
    m_MaxCurrentHorizontalPolsitionAngle=(int)Mathf.Round(360/m_AnglePerFrame);                                                     // init the value (look at declaration for the utility )
    m_CurrentStartHorizontalAngle=0;                                                                                                // position forward = index 0
    m_HitInformation=new LazerInformation[m_iVert,m_nbLineTab];

    /*Setting for rawimage*/
    m_lastImage=new Texture2D(m_nbLineTab,m_iVert,TextureFormat.RGBA32,false);
    //m_imageRendered=false;

    m_rawImage.texture=m_lastImage;
    m_offsetImage=(int)Mathf.Round(m_nbLineTab/2)+1;
    

}



/****************************************************************************************************************************************************************/
/* Rotation of the sensor from the end of the previous position by the defined angle m_AnglePerFrame and recording of collisions*/

private void SensorRotation(){
    m_iVert=(int)Mathf.Round((m_maxVerticalAngle-m_minVerticalAngle)/m_stepAngleV);                                                     // Update the number of Vertical measurement if the user modify init parameters
    m_iHoriz=(int)Mathf.Round(m_AnglePerFrame/m_stepAngleH);                                                                            // Update the number of Horizontal measurement if the user modify init parameters

    for(int k=0;k<m_iHoriz;k++){
        this.transform.Rotate(Vector3.up,m_stepAngleH);                                                                                 // Rotation of the sensor around the local vertical axis, relative to the orientation of the car 
        
        for(int i=0;i<m_iVert;i++){
            float dist;                                                                                                                 
            m_lookdirection= Quaternion.AngleAxis(-m_minVerticalAngle-i*m_stepAngleV, this.transform.right)*this.transform.forward;     // Rotation of the direction vector around x-Axis to set the vertical orientation of the ray
            Vector3 _positionSensor=this.transform.position;                                                                            // Get the position of the sensor
            _positionSensor.y+=m_offsetHighSensor;                                                                                      // Ad the vertical offset to be enough higher
            
            RaycastHit _hit;                                                                                                            // Create a structure for collision ray
            Ray _ray =new Ray(_positionSensor,m_lookdirection);                                                                         // Set the direction of the ray

            if (Physics.Raycast(_ray,out _hit,m_MeasurementRange)){                                                                     // Check if there is a collision

                dist=_hit.distance;                                                                                                     // Get the distance between the car and the collision point 
                dist=Mathf.Clamp(dist,0,m_distanceAccuracy);                                                                            // Set the accuracy for the distance measurement 
                Debug.DrawLine(_positionSensor, _hit.point, Color.green);                                                               // Draw the ray in green if there is a collision

                Debug.DrawLine(_hit.point - Vector3.up* 0.3f, _hit.point + Vector3.up * 0.3f, Color.red, 0, false);                     // Draw the collision point in red
                Debug.DrawLine(_hit.point - Vector3.left* 0.3f, _hit.point + Vector3.left * 0.3f, Color.red, 0, false);                 // Draw the collision point in red
                Debug.DrawLine(_hit.point - Vector3.forward*0.3f, _hit.point + Vector3.forward * 0.3f, Color.red, 0, false);            // Draw the collision point in red
            }   
            else
            {
                dist = m_MeasurementRange;                                                                                              // Set the distance to the max distance detection
                Debug.DrawRay(_positionSensor, m_lookdirection*m_MeasurementRange, Color.gray);                                         // Draw the ray in grey 
            }

            m_tabRayCastHit[i,m_CurrentStartHorizontalAngle*m_iHoriz+k]=_hit;                                                           // Update the tab of hit
        }
    }

    
}



/****************************************************************************************************************************************************************/
/* Get the color of the pixel that was hit */
/* DOESN'T WORK */

private void setColorIntensityScale(RaycastHit a_hit,float a_intensity,int a_iV, int a_iH){
    /* Getting the color of the pixel which is hitting */
    Renderer _renderer = a_hit.collider.GetComponent<MeshRenderer>();
    Texture2D _texture2D = _renderer.sharedMaterial.mainTexture as Texture2D;
    Vector2 _pCoord = a_hit.textureCoord;
    _pCoord.x *= _texture2D.width;
    _pCoord.y *= _texture2D.height;
    Vector2 _tiling = _renderer.sharedMaterial.mainTextureScale;
    Color _color = _texture2D.GetPixel(Mathf.FloorToInt(_pCoord.x * _tiling.x) , Mathf.FloorToInt(_pCoord.y * _tiling.y));

    /*Change the color intensity */
    _color.a=a_intensity;
    m_HitInformation[a_iV,a_iH].colorRGBA=_color;

}



/****************************************************************************************************************************************************************/
/* Update the tab private m_HitInformation */
private void SetTabIntensityAndDistance(){
    for(int k=0;k<m_iHoriz;k++){
        for(int i=0;i<m_iVert;i++){
            int _horizontalIndex=(m_CurrentStartHorizontalAngle)*m_iHoriz+k;                                                            // Take the information of which part of the image we have to change
            
            /*get the distance*/
            float _distance=m_tabRayCastHit[i,_horizontalIndex].distance;                                                               // Get the distance information from the m_tabRayCastHit
            m_HitInformation[i,_horizontalIndex].distance=_distance;                                                                    // Save the information in the tab of personale struct m_HitInformation

            /*calculation of the intensity relative to the distance*/
            float _NormalizedDistance =_distance/m_MeasurementRange;                                                                    // Calculation of the normalized distance from the maximum detection distance
            float _intensityParameter =_NormalizedDistance;                                                                             // We can modify the calculation of the intensity parameter by another calculation
            m_HitInformation[i,_horizontalIndex].intensityParameter=_intensityParameter;                                                // Save the intensity parameter in the same struct tab
            //setColorIntensityScale(m_tabRayCastHit[i,_horizontalIndex],_intensityParameter,i,_horizontalIndex);                       // Get the the color of the pixel that was hit /*Not use here because it doesn't work*/

            /*Change the color intensity */
            _intensityParameter=_intensityParameter*m_ajustColorParameter;                                                              // Adjustment of the intensity parameter by an adjustable weighting 
            float _val=1-Mathf.Exp(-_intensityParameter);                                                                               // Calculation of the level of the Grey color representing the distance
            Color _color=new Color(_val,_val,_val,1);                                                                                   // Creation of the new color for this point 
            m_HitInformation[i,_horizontalIndex].colorRGBA=_color;                                                                      // Save the Color of the pixel 
        }
    }
}



/****************************************************************************************************************************************************************/
/* Update of the image rendering */

private void ImageRendered(){
    for(int k=0;k<m_iHoriz;k++){
        for(int i=0;i<m_iVert;i++){
            int _horizontalIndex=(m_CurrentStartHorizontalAngle)*m_iHoriz+k;                                            // Take the information of which part of the image we have to change
            m_lastImage.SetPixel(_horizontalIndex+m_offsetImage,i,m_HitInformation[i,_horizontalIndex].colorRGBA);      // update the texture apply to the image with an offset in order to have the front of the car at the center
        }
    }
    m_lastImage.Apply();                                                                                                // Apply the new texture
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

    /* Update the tab private m_HitInformation */
    SetTabIntensityAndDistance();

    /* update of the image rendering */
    ImageRendered();
}



/****************************************************************************************************************************************************************/
/* Struct perso */
public struct LazerInformation{
    public float distance;
    public float intensityParameter;
    public Color colorRGBA;
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
[Range(0f,1.2f)]                                    // Ajust the parameter to calculate the intensity of each color acoording to the distance
public float m_ajustColorParameter;
private int m_offsetImage;                          // Offset position of the image on the screen
private Texture2D m_lastImage=null;                 // The texture to apply on the raw image to see somethink
//private bool m_imageRendered;


private Vector3 m_lookdirection;                    // direction for ray
private int m_iVert,m_iHoriz;                       // Calculate at each frame how many vertical/Horizontal step we have to do according to m_AnglePerFrame and m_stepAngleV/H

private RaycastHit[,] m_tabRayCastHit;              // Storage of each RaycastHit in order to have a persistent ref
private int m_nbLineTab;                            // Storage of how many Horizontal information we have to save 
private int m_CurrentStartHorizontalAngle;          // To know which part of the image has to be update
private int m_MaxCurrentHorizontalPolsitionAngle;   // Set at the beginning. Represent how many frame to make 360 deg
private LazerInformation[,] m_HitInformation;       // Personal structure, for point provide by the sensor in 360 deg, we storage the distance, the intensity Parameter and the color of the pixel



}
