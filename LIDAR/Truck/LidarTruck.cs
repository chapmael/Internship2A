using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;

using ServerManager;
using Enumeration;
using Enumeration.Labeling;
using PCDlib;

public class LidarTruck : MonoBehaviour
{
    /*Public Attribut*/
    [Tooltip("4 Sensors in order Front, Right, Back, Left")]
    public List<Transform> p_Sensors;
    [Tooltip("Step for vertical Angle")]
    public float p_stepAngleV;
    [Tooltip("Step for horizontal Angle")]
    public float p_stepAngleH;
    [Tooltip("max value for vertical angle")]
    public int p_maxVerticalAngle;
    [Tooltip("min value for vertical angle")]
    public int p_minVerticalAngle;
    [Tooltip("max of detection measurement")]
    public float p_MeasurementRange;
    [Tooltip("Number of Lidar, 1: Front, 2: Front & Back, 4: Front & Back & Side")]
    public int p_nbLidar=1;
    public int p_ratio = 5;
    public int m_nbPoint = 0;
    public int m_count = 0;
    public Transform p_viewtarget;

    /*[HideInInspector]*/
    public int p_iVert,p_iHoriz;
    /* GUI parameters*/
    public SettingButton SB;
    public ButtonHandler m_button; 
    [HideInInspector]public bool p_indicatorSizeQueue=false;
    public ServerRequestManager SRM;
    [HideInInspector]public string m_FileName="DefaultName.txt";
    [HideInInspector]public bool m_boolLabeling=false;
   

    /* Private Attribut */
    private Vector3 m_lookdirection;
    private List<Quaternion> m_InitialPosition;
    private int m_indexSensor=0;

    private EnumStruct m_dataEnum;
    private QuickSave m_quickSave;
    private Thread m_SavingAll;
    private ConcurrentQueue<Data> m_QToSaveAll;
    private Vector3 m_CurrentPos;
    private Vector3 m_CurrentViewTarget;

    private bool m_boolSave=false;
    private readonly object LockQueue = new object();

    private string m_pathLabeling= @"C:\Users\Simulation\Documents\SavingData\Labeling";
    private string m_pathPCD= @"C:\Users\Simulation\Documents\SavingData\PCD";
    private CreateFilePath m_filePath;




    // Start is called before the first frame update
    void Start()
    {
        p_iVert = (int)Mathf.Round((p_maxVerticalAngle - p_minVerticalAngle) / p_stepAngleV);
        p_iHoriz = (int)Mathf.Round(160 / p_stepAngleH);
        m_InitialPosition = new List<Quaternion>();

        m_dataEnum = new EnumStruct();

        /*Writting of the file with all position*/
        m_FileName = PlayerPrefs.GetString("RealTimePath");                                                       // Fetch the preferences from the previous page (menu) to open a desired backup file
        m_pathLabeling = System.IO.Path.Combine(PlayerPrefs.GetString("SavePath"), "Labeling");
        m_pathPCD = System.IO.Path.Combine(PlayerPrefs.GetString("SavePath"), "PCD");

        /* Create Directory if they don't exist*/
        Directory.CreateDirectory(m_pathLabeling);
        Directory.CreateDirectory(m_pathPCD);
        
        m_filePath = new CreateFilePath(m_pathLabeling, m_pathPCD);
        m_filePath.CreateName();
        m_CurrentPos = transform.position;
        m_CurrentViewTarget = p_viewtarget.position;
        //m_CurrentAngleView =Vector3.SignedAngle(Vector3.ProjectOnPlane(transform.forward, Vector3.up), Vector3.forward, Vector3.up) ;

        m_quickSave = new QuickSave(m_filePath.GetPathPCD(), m_filePath.GetPathLabeling(), m_CurrentPos, m_CurrentViewTarget, 100) ;


        m_SavingAll =new Thread(new ThreadStart(SaveAllThread));
        m_SavingAll.Start();
        m_QToSaveAll=new ConcurrentQueue<Data>();

        SB.resetButton();


        for(int i=0;i<4;i++)
        {
            m_InitialPosition.Add(p_Sensors[i].transform.localRotation);
        }

    }

    private void SaveAllThread()
    {
        while (Thread.CurrentThread.IsAlive)
        {
            Data _data;
            if (m_nbPoint >= 16000)
            {
                if (m_button.GetStateTrainingOrNot())
                {
                    m_filePath.CreateName();
                    lock (LockQueue)
                    {
                        m_quickSave.Save(m_boolLabeling, m_CurrentPos, m_CurrentViewTarget, m_filePath.GetPathPCD(), m_filePath.GetPathLabeling()) ;
                    }
                    
                }
                else
                {
                    m_quickSave.SaveOverWrite(m_CurrentPos, m_CurrentViewTarget, m_FileName);
                    SRM.SendAndReceiveRequest("1");
                }
                m_nbPoint = 0;
                m_count = 0;


            }
            else
            {
                if (m_QToSaveAll.TryDequeue(out _data))
                {
                    if(m_nbPoint==8000)
                    {
                        m_CurrentPos = _data.GetPos();
                        m_CurrentViewTarget = _data.GetView();
                    }
                    if (m_count < 50)
                    {
                        if (m_button.GetStateTrainingOrNot())
                        {
                            m_quickSave.AddPoint(m_boolLabeling, _data.GetPointV(), _data.GetLabel());
                        }
                        else
                        {
                            m_quickSave.AddPoint(false, _data.GetPointV());
                        }
                        m_nbPoint += 1;
                        m_count += 1;
                    }
                    else
                    {
                        m_quickSave.AddPoint(m_boolLabeling, _data.GetPointV(), _data.GetLabel());
                        m_nbPoint += 1;
                        if (m_button.GetStateTrainingOrNot())
                        {
                            m_quickSave.SavePartPoint(m_boolLabeling, m_filePath.GetPathPCD(), m_filePath.GetPathLabeling());
                        }
                        m_count = 0;
                    }
                    
                }
            }
            
        }
    }



    private void SensorRotation(int a_SensorIndex)
    {
        /*Initialisation*/
        p_Sensors[a_SensorIndex].transform.localRotation = m_InitialPosition[a_SensorIndex];
        p_Sensors[a_SensorIndex].transform.Rotate(Vector3.up, 10);

        for(int k=0; k<p_iHoriz+1;k++)
        {
            
            p_Sensors[a_SensorIndex].transform.Rotate(Vector3.up, p_stepAngleH);

            for(int i=0;i<p_iVert;i++)
            {
                float _distance = p_MeasurementRange;
                m_lookdirection = Quaternion.AngleAxis(-p_minVerticalAngle-i*p_stepAngleV, p_Sensors[a_SensorIndex].transform.right)*p_Sensors[a_SensorIndex].transform.forward;
                Vector3 _positionSensor = p_Sensors[a_SensorIndex].transform.position;

                RaycastHit _hit;
                Ray _ray = new Ray(_positionSensor,m_lookdirection);

                bool _BoolCollision = Physics.Raycast(_ray, out _hit, p_MeasurementRange);
                if(_BoolCollision)
                {
                    _distance = _hit.distance;
                    //Debug.DrawLine(_positionSensor, _hit.point, Color.green);                                                               // Draw the ray in green if there is a collision

                    //Debug.DrawLine(_hit.point - Vector3.up* 0.3f, _hit.point + Vector3.up * 0.3f, Color.red, 0, false);                     // Draw the collision point in red
                    //Debug.DrawLine(_hit.point - Vector3.left* 0.3f, _hit.point + Vector3.left * 0.3f, Color.red, 0, false);                 // Draw the collision point in red
                    //Debug.DrawLine(_hit.point - Vector3.forward*0.3f, _hit.point + Vector3.forward * 0.3f, Color.red, 0, false);            // Draw the collision point in red
                    
                    Vector3 _point =_hit.point;
                    LabelingData _labelData = null;
                    if(m_boolLabeling)
                    {
                        string _name = m_dataEnum.GetClassFromString(_hit.collider.gameObject.name);
                        BoundCreator _boundcreator=new BoundCreator();
                        if ((!_name.Contains("Terrain"))&& (!_name.Contains("Road")) && (!_name.Contains("Intersection")) && (!_name.Contains("Building")) && (!_name.Contains("UnKnown")))
                        {
                            
                            _boundcreator.SetBound(_hit.transform.gameObject,_name) ;
                            //_boundcreator.PrintBound(_boundcreator.GetBounds());
                            
                                
                        }
                        Bounds _bounds = _boundcreator.GetBounds();
                        

                        _labelData = (m_button.GetStateTrainingOrNot())?new LabelingData(this.transform.position,
                                                                                        _bounds.center,
                                                                                        _bounds.size,
                                                                                        _name,
                                                                                        _distance):null;
                    }                    
                    Data _data = new Data(_labelData, _point, transform.position, p_viewtarget.position);
                    
                    m_QToSaveAll.Enqueue(_data);
                }
                /*   
                else
                {
                    Debug.DrawRay(_positionSensor, m_lookdirection*p_MeasurementRange, Color.gray);                                         // Draw the ray in grey 
                }
                */

                    
            }
        }
    }
    


    // Update is called once per frame
    void Update()
    {
        

        if (p_indicatorSizeQueue){
            SB.UpdateIndicator(m_QToSaveAll.Count);
        }
        if(m_button.GetState()){
            SensorRotation(m_indexSensor);
            m_indexSensor=(m_indexSensor<p_nbLidar-1)?m_indexSensor+1:0;
            m_boolSave=(m_indexSensor==p_nbLidar-1)?true:false;
        }

    }
}
