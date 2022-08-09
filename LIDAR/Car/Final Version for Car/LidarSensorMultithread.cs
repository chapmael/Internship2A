using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Threading;
using PCDlib;
using ServerManager;
using System.Collections.Concurrent;
using Enumeration.Labeling;
using System.IO;
using UnityEngine.SceneManagement;




namespace Enumeration{
public class LidarSensorMultithread : MonoBehaviour
{
/****************************************************************************************************************************************************************/
/* Beginning of the code */
    void Start()
    {
        this.transform.rotation=m_objectParent.transform.rotation;                                              // acquire axis of the car and apply to the sphere to init the position 
        m_iVert=(int)Mathf.Round((m_maxVerticalAngle-m_minVerticalAngle)/m_stepAngleV);                         // Number of vertical points per image                   
        m_iHoriz=(int)Mathf.Round(m_AnglePerFrame/m_stepAngleH);                                                // Number of horizontal points per image

        /*Struct of color*/
        m_dataEnum=new EnumStruct();                                                                           // Makes the link between object and color

        /*Writting of the file with all position*/                  
        m_FileName=PlayerPrefs.GetString("RealTimePath");                                                       // Fetch the preferences from the previous page (menu) to open a desired backup file
        m_pathLabeling=System.IO.Path.Combine(PlayerPrefs.GetString("SavePath"),"Labeling");                    
        m_pathPCD=System.IO.Path.Combine(PlayerPrefs.GetString("SavePath"),"PCD");

        /* Create Directory if they don't exist*/
        Directory.CreateDirectory(m_pathLabeling);
        Directory.CreateDirectory(m_pathPCD);

        m_saveAll=new SaveAll(m_FileName);
        m_filePath=new CreateFilePath(m_pathLabeling,m_pathPCD);


        m_SavingAll=new Thread(new ThreadStart(SaveAllThread));
        m_SavingAll.Start();
        m_QToSaveAll=new ConcurrentQueue<Data>();

        SB.resetButton();                                                                                       // Initialization of variable 
        m_nbScene=(SceneManager.GetActiveScene()).buildIndex;
    }



/****************************************************************************************************************************************************************/
/* Saving All Thread */
private void SaveAllThread()
{
    while(Thread.CurrentThread.IsAlive)
    {
        Data _data;
        lock(LockQueue){
            if(m_QToSaveAll.TryDequeue(out _data))
            {
                m_saveAll.Add(_data.GetLabel(),_data.GetPoint());
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
            if(k%(int)(m_iHoriz/(m_AnglePerFrame/m_ratioSave))==0){
                m_boolSave=true;                                                                                                            // Saving Data at a certain ratio set by the user 
            }
            
        }
        for(int i=0;i<m_iVert;i++){
            float _distance=m_MeasurementRange;
            Color _color=new Color(1,1,1,1);                                                                                                // Color by default
            m_lookdirection= Quaternion.AngleAxis(-m_minVerticalAngle-i*m_stepAngleV, this.transform.right)*this.transform.forward;         // Rotation of the direction vector around x-Axis to set the vertical orientation of the ray
            Vector3 _positionSensor=this.transform.position;                                                                                // Get the position of the sensor
            _positionSensor.y+=m_offsetHighSensor;                                                                                          // Add the vertical offset to be enough higher
            
            RaycastHit _hit;                                                                                                                // Create a structure for collision ray
            Ray _ray =new Ray(_positionSensor,m_lookdirection);                                                                             // Set the direction of the ray
            
            bool _BoolCollision =Physics.Raycast(_ray,out _hit,m_MeasurementRange);                                                         // Get the statu of the collision (collision or not in the Measurement range)
            if (_BoolCollision){                                                                                                            // Check if there is a collision

                _distance=_hit.distance;                                                                                                    // Get the distance of the collision point from the Lidar
                
                
                /*code to manage the acquisition and saving of the object's color (requires significant resources)*/
                //float _NormalizedDistance =_distance/m_MeasurementRange;                                                                  // Calculation of the normalized distance from the maximum detection distance
                //float _val=1-Mathf.Exp(-m_ajustColorParameter*Mathf.Pow(_NormalizedDistance,4f));                                         // Set a coeff depending of the distance of the point 

                //_color=m_dataEnum.GetColorFromString(_hit.collider.gameObject.name);                                                     // Get the color of the object from enumClass
                //float _H,_S,_V;
                //Color.RGBToHSV(_color,out _H, out _S, out _V);
                //_V*=_val;                                                                                                                 // Apply the distance coefficient on the color 
                //_color=Color.HSVToRGB(_H,_S,_V);
                //_color.a=1;
                /*End of the color part*/ 
                if(m_button.GetState()){                                                                                                    // checks if the user wants to save the data or not 
                    bool _training=m_button.GetStateTrainingOrNot();
                    Point _point = new Point(_hit.point.x, _hit.point.y, _hit.point.z);
                    LabelingData _labelData;
                    switch(m_nbScene)
                    {
                        case 1:
                            _labelData=(_training)?new LabelingData(this.transform.position,
                                                            _hit.point,
                                                            new AssignClass(_hit.collider.gameObject.name),
                                                            _distance):null;
                            break;
                        case 2: 
                            _labelData=(_training)?new LabelingData(this.transform.position,
                                                            _hit.point,
                                                            m_dataEnum.GetClassFromString(_hit.collider.gameObject.name),
                                                            _distance):null;
                            break;
                        
                        default:
                            _labelData=null;
                            break;

                    }
                    

                    Data _data=new Data(_labelData,_point);
                    m_QToSaveAll.Enqueue(_data);

                    if ( m_boolSave)                                                                                                        // save when the signal is given, after enough points in memory not to save all the time and thus slow down the simulation
                    {   
                        
                        if(_training)
                        {   
                            m_filePath.CreateName();                                                                                        // Create a new File Name
                            lock(LockQueue){                                                                                                // lock access to the Message Queue so that values are not added during saving
                                m_saveAll.Save(true,m_filePath.GetPathPCD(),m_filePath.GetPathLabeling());                                  // Saving Data
                            }
                        }

                        else
                        {   
                            lock(LockQueue){                                                                                                // Lock Access
                                m_saveAll.Save(false);
                            }
                            SRM.SendAndReceiveRequest("1");
                            
                        }
                        m_boolSave=false;                                                                                                   // Reset status 
                    }
                    
                }
            }

        }
    }
}

/****************************************************************************************************************************************************************/
/* Call at each frame */


    void Update()
    {  
        
        /*Update the display to set correctly the ratio to save */
        if(m_indicatorSizeQueue){
            SB.UpdateIndicator(m_QToSaveAll.Count);
        }

        SensorRotation();
    }



    /* Public Setting*/
    public float m_stepAngleV;                                                      // step for vertical angle
    public float m_stepAngleH;                                                      // step for horizontal angle
    public int m_maxVerticalAngle;                                                  // max value for vertical angle
    public int m_minVerticalAngle;                                                  // min value for vertical angle
    public float m_offsetHighSensor;                                                // set the vertical offset for the position of the Lidar
    public int m_AnglePerFrame;                                                     // defines the angle of the image to be analyzed per frame
    public float m_MeasurementRange;                                                // max of detection measurement
    [Range(0f,15f)]                                                                 // Ajust the parameter to calculate the intensity of each color acoording to the distance
    public float m_ajustColorParameter=2;                                           // Ajust the coefficient to see the effect of the distance on the color 
    public Transform m_objectParent;                                                // object with the lidar
    public ButtonHandler m_button;                                                  // User interface 
    public SettingButton SB;                                                        // User interface
    [HideInInspector]public bool m_indicatorSizeQueue=false;                        // User interface 
    public ServerRequestManager SRM;
    /*Private Setting*/
    private Vector3 m_lookdirection;                                                // direction for ray
    [HideInInspector]public int m_iVert,m_iHoriz;                                   // Calculate at each frame how many vertical/Horizontal step we have to do according to m_AnglePerFrame and m_stepAngleV/H
    
    private int m_frame=0;                                                          // Number of the saving frame

    private EnumStruct m_dataEnum;                                                 // Color information depending of the object 
    private SaveAll m_saveAll;                                                      // Reference to the class create to save both pointcloud and labeldata

    private Thread m_SavingAll;                                                     // Thread to add a new point and a new data for label
    private ConcurrentQueue<Data> m_QToSaveAll;                                     // Queue to communicate between main thread and the saving thread 
        
    private bool m_boolSave=false;                                                  // Time to save or not
    [HideInInspector]public string m_FileName="DefaultName.txt";                    // Name of the saving data file by default 
    private readonly object LockQueue=new object();
    
    [Range(1,180)]
    public int m_ratioSave=1;                                                       //Ratio to save data 

    private string m_pathLabeling=@"C:\Users\chapm\Documents\SavingData\Labeling";  // Path for the saving data for labeling 
    private string m_pathPCD=@"C:\Users\chapm\Documents\SavingData\PCD";            // Path for point cloud storage
    private CreateFilePath m_filePath;                                              // Ref to class to generate fileName
    private int m_nbScene;
}
}