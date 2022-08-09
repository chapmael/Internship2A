using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Threading;



namespace Enumeration{
public class LidarSensorMultithread : MonoBehaviour
{

    void Start()
    {
        this.transform.rotation=m_objectParent.transform.rotation;                                          // acquire axis of the car and apply to the sphere to init the position 
        m_iVert=(int)Mathf.Round((m_maxVerticalAngle-m_minVerticalAngle)/m_stepAngleV);                                                 
        m_iHoriz=(int)Mathf.Round(m_AnglePerFrame/m_stepAngleH);

        /*Struct of color*/
        m_dataColor=new EnumStruct();

        /*Writting of the file with all position*/
        m_FileName=PlayerPrefs.GetString("SavePath");
        m_savetest=new SavePoint(m_FileName);

        m_ReceiveSaveThread=new Thread(new ThreadStart(ReceiveSaveThread));
        m_ReceiveSaveThread.Start();

        m_QToSave=new Queue();
        SB.resetButton();
    }

private void ReceiveSaveThread()
    {
        while(Thread.CurrentThread.IsAlive)
        {
            if (m_QToSave.Count>0)
            {   
                lock(this){
                    string _eleQueue=(string)m_QToSave.Dequeue();
                    //print(m_QToSave.Count);
                    m_savetest.SaveEleQueue(_eleQueue);
                }
            }
        }
    }


/****************************************************************************************************************************************************************/
/* Rotation of the sensor from the end of the previous position by the defined angle m_AnglePerFrame and recording of collisions*/

private void SensorRotation(){

    for(int k=0;k<m_iHoriz;k++){
        this.transform.Rotate(Vector3.up,m_stepAngleH);
        if(m_button.GetState()){
            if(k%(int)(m_iHoriz/m_ratioSave)==0){
                m_boolSave=true;
            }
            
        }
        for(int i=0;i<m_iVert;i++){
            float _distance=m_MeasurementRange;
            Color _color=new Color(1,1,1,1);
            m_lookdirection= Quaternion.AngleAxis(-m_minVerticalAngle-i*m_stepAngleV, this.transform.right)*this.transform.forward;     // Rotation of the direction vector around x-Axis to set the vertical orientation of the ray
            Vector3 _positionSensor=this.transform.position;                                                                            // Get the position of the sensor
            _positionSensor.y+=m_offsetHighSensor;                                                                                      // Ad the vertical offset to be enough higher
            
            RaycastHit _hit;                                                                                                            // Create a structure for collision ray
            Ray _ray =new Ray(_positionSensor,m_lookdirection);                                                                         // Set the direction of the ray
            bool _BoolCollision =Physics.Raycast(_ray,out _hit,m_MeasurementRange);
            
            if (_BoolCollision){                                                                                                        // Check if there is a collision

                _distance=_hit.distance;
                float _NormalizedDistance =_distance/m_MeasurementRange;                                                                // Calculation of the normalized distance from the maximum detection distance
                float _val=1-Mathf.Exp(-m_ajustColorParameter*Mathf.Pow(_NormalizedDistance,4f));
                
                //_color=m_dataColor.GetColorFromString(_hit.collider.gameObject.name);   
                //float _H,_S,_V;
                //Color.RGBToHSV(_color,out _H, out _S, out _V);
                //_V*=_val;
                //_color=Color.HSVToRGB(_H,_S,_V);
                //_color.a=1;
                if(m_button.GetState()){
                    string _info =_hit.point.x+" "+_hit.point.y+" "+_hit.point.z+" "+m_frame+" "+_color.r+" "+_color.g+" "+_color.b+"\n";
                    m_info+=_info;

                    if( m_boolSave)
                    {   
                        m_QToSave.Enqueue(m_info);
                        m_info="";
                        m_boolSave=false;
                    }
                    
                }
            }

        }
    }
}




    void Update()
    {   
        if(m_button.GetState()){
            m_frame+=1;            
        }
        if(m_indicatorSizeQueue){
            SB.UpdateIndicator(m_QToSave.Count);
        }

        SensorRotation();
    }



    /* Public Setting*/
    public float m_stepAngleV;                          // step for vertical angle
    public float m_stepAngleH;                          // step for horizontal angle
    public int m_maxVerticalAngle;                      // max value for vertical angle
    public int m_minVerticalAngle;                      // min value for vertical angle
    public float m_offsetHighSensor;                    // set the vertical offset for the position of the Lidar
    public int m_AnglePerFrame;                         // defines the angle of the image to be analyzed per frame
    public float m_MeasurementRange;                    // max of detection measurement
    [Range(0f,15f)]                                     // Ajust the parameter to calculate the intensity of each color acoording to the distance
    public float m_ajustColorParameter=2;
    public Transform m_objectParent;                    // object with the lidar
    public ButtonHandler m_button;
    public SettingButton SB;
    [HideInInspector]public bool m_indicatorSizeQueue=false;
    
    /*Private Setting*/
    private Vector3 m_lookdirection;                    // direction for ray
    [HideInInspector]public int m_iVert,m_iHoriz;       // Calculate at each frame how many vertical/Horizontal step we have to do according to m_AnglePerFrame and m_stepAngleV/H
    
    private bool m_SaveOrNot;
    private int m_frame=0;

    private EnumStruct m_dataColor;
    private SavePoint m_savetest;

    private Thread m_ReceiveSaveThread;
    private Queue m_QToSave;

    private string m_info;
    private bool m_boolSave=false;
    [HideInInspector]public string m_FileName="DefaultName.txt";
    
    [Range(1,180)]
    public int m_ratioSave=54;
}
}