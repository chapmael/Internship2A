using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour
{
        //Transform player;
    public WheelCollider p_FrontLeftWheel;
    public WheelCollider p_FrontRightWheel;
    public WheelCollider p_RearLeftWheel;
    public WheelCollider p_RearRightWheel;
    public Transform p_FrontLeftT;
    public Transform p_FrontRightT;

    public float p_maxSteerAngle=45f;
    public float p_motorForce = 450;

    public float p_distanceSlowTarget=30;
    public float p_VmaxTurn=25;
    public float p_VminTurn=20;

    public Transform p_FrontDetection;
    public Transform p_BackDetection;
    [Range(3,9)]
    public int p_numberRayDetection;
    public float p_maxdistanceDetection=5;
    public float p_offsetSensorFront=3.8f;
    public float p_offsetSensorBack=5.2f;

    private Rigidbody m_rigidbody;
    private Vector3 m_direction;

    public float motor;
    public float brake;



    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }
    private bool ObjectDetection(bool a_Reverse)
    {
        RaycastHit _hit=new RaycastHit();
        bool _BoolCollisionvar=false;
        if(a_Reverse)
        {
            Vector3 _lookdirection;
            for(int i=0;i<p_numberRayDetection;i++)
            {
                Vector3 _positionSensor=p_BackDetection.localPosition;
                _positionSensor.x+=-1f+(float)i*2/(p_numberRayDetection-1);
                _positionSensor.z+=p_offsetSensorBack;
                _positionSensor=p_BackDetection.TransformPoint(_positionSensor);
                _lookdirection=Quaternion.AngleAxis(-210+(p_numberRayDetection-1-(float)i)*60f/(p_numberRayDetection-1),transform.up)*p_FrontDetection.forward;
                
                Ray _ray = new Ray(_positionSensor,_lookdirection);
                bool _BoolCollision = Physics.Raycast(_ray,out _hit,p_maxdistanceDetection);
                Debug.DrawLine(_positionSensor,_positionSensor+_lookdirection*p_maxdistanceDetection,Color.red);
                if(_BoolCollision)
                {
                    string _name = _hit.collider.gameObject.name;
                    if(_name.Contains("ThirdP"))
                    {
                        _BoolCollisionvar= true;
                    }
                    if(_name.Contains("bodyHTR"))
                    {
                        _BoolCollisionvar= true;
                    }
                    break;
                }
            }
            if(_BoolCollisionvar){return true;}
            else{return false;}
        }
        else
        {
            Vector3 _lookdirection;
            for(int i=0;i<p_numberRayDetection;i++)
            {
                Vector3 _positionSensor=p_FrontDetection.localPosition;
                _positionSensor.x+=-1f+(float)i*2/(p_numberRayDetection-1);
                _positionSensor.z-=p_offsetSensorFront;
                _positionSensor=p_FrontDetection.TransformPoint(_positionSensor);
                _lookdirection=Quaternion.AngleAxis(-30f+(float)i*60f/(p_numberRayDetection-1),transform.up)*p_FrontDetection.forward;

                Ray _ray = new Ray(_positionSensor,_lookdirection);
                bool _BoolCollision = Physics.Raycast(_ray,out _hit,p_maxdistanceDetection);
                Debug.DrawLine(_positionSensor,_positionSensor+_lookdirection*p_maxdistanceDetection,Color.red);
                
                if(_BoolCollision)
                {
                    string _name = _hit.collider.gameObject.name;
                    if(_name.Contains("ThirdP"))
                    {
                        _BoolCollisionvar= true;
                    }
                    if(_name.Contains("bodyHTR"))
                    {
                        _BoolCollisionvar= true;
                    }
                    break;
                }
            }
            if(_BoolCollisionvar){return true;}
            else{return false;}
        }
    }
    public void Move(Vector3 a_target, bool a_Reverse, bool a_Stop, bool a_Brake)
    {
        if(a_Stop)
        {
            StopTruck();
        }
        else
        {
            if(a_Brake)
            {
                StopTruck();
            }
        
            else
            {
                if(ObjectDetection(a_Reverse))
                {
                    StopTruck();
                }
            
                else
                {
                    //m_direction=Vector3.RotateTowards(transform.forward, (a_target - transform.position),p_maxSteerAngle * Mathf.Deg2Rad,0f);
                    float _dist=Vector3.Distance(a_target,transform.position);
                    //Debug.Log(_dist);
                    m_direction=a_target - transform.position;
                    if(UpdateWheelSteer(a_Reverse))
                    {
                        Slow(a_Reverse);
                    }
                    else
                    {
                        if(_dist<p_distanceSlowTarget){Break(a_Reverse);}
                        else
                        {
                            if(m_rigidbody.velocity.magnitude*3.6<p_VmaxTurn+2){MotorWheel(a_Reverse);}
                            else{Break(a_Reverse);}
                        }
                    }
                    motor=p_FrontLeftWheel.motorTorque;
                    brake=p_FrontLeftWheel.brakeTorque;

                }
            }
        }

    }
    
    public void StopTruck()
    {
        p_FrontLeftWheel.motorTorque=p_FrontRightWheel.motorTorque = 0;
        m_rigidbody.drag = m_rigidbody.velocity.magnitude;
        p_FrontLeftWheel.brakeTorque=p_FrontRightWheel.brakeTorque = 40000000;
    }

    private void Break(bool a_Reverse)
    {
        if(m_rigidbody.velocity.magnitude*3.6f>p_VmaxTurn)
        {
            p_FrontLeftWheel.brakeTorque=p_FrontRightWheel.brakeTorque = 350;
            m_rigidbody.drag = m_rigidbody.velocity.magnitude/10f;
        }
        else
        {
            p_FrontLeftWheel.brakeTorque=p_FrontRightWheel.brakeTorque = 0;
            m_rigidbody.drag = 0;
        }
        
        
        if(a_Reverse)
        {
            if(m_rigidbody.velocity.magnitude*3.6<5)
            {
                p_FrontLeftWheel.motorTorque=p_FrontRightWheel.motorTorque = -p_motorForce*2f;
            }
            else
            {
                p_FrontLeftWheel.motorTorque=p_FrontRightWheel.motorTorque = -p_motorForce/2f;
            }
        }
        else
        {
            if(m_rigidbody.velocity.magnitude*3.6<5)
            {
                p_FrontLeftWheel.motorTorque=p_FrontRightWheel.motorTorque = p_motorForce*2f;
            }
            else
            {
                p_FrontLeftWheel.motorTorque=p_FrontRightWheel.motorTorque = p_motorForce/2f;
            }
        }
    }

    private void Slow(bool a_Reverse)
    {
        m_rigidbody.drag = 0;
        if(a_Reverse)
        {
            p_FrontLeftWheel.brakeTorque=p_FrontRightWheel.brakeTorque = (m_rigidbody.velocity.magnitude*3.6f<p_VminTurn)?0:50;
            p_FrontLeftWheel.motorTorque=p_FrontRightWheel.motorTorque = -p_motorForce;
        }
        else
        {
            p_FrontLeftWheel.brakeTorque=p_FrontRightWheel.brakeTorque = (m_rigidbody.velocity.magnitude*3.6f<p_VminTurn)?0:400;
            p_FrontLeftWheel.motorTorque=p_FrontRightWheel.motorTorque = p_motorForce;
        }
    }

    private bool UpdateWheelSteer(bool a_Reverse)
    {
        //float _steerAngle =  38*(m_direction.x/m_direction.magnitude);

        float _steerAngle=(a_Reverse)?-Mathf.Clamp(Vector3.SignedAngle(-transform.forward,m_direction,transform.up),-p_maxSteerAngle,p_maxSteerAngle):Mathf.Clamp(Vector3.SignedAngle(transform.forward,m_direction,transform.up),-p_maxSteerAngle,p_maxSteerAngle);
        p_FrontLeftWheel.steerAngle=p_FrontRightWheel.steerAngle = _steerAngle;
        if(Mathf.Abs(_steerAngle)<p_maxSteerAngle-5){return false;}
        else{return true;}

    }
    
    private void MotorWheel(bool a_Reverse)
    {
        p_FrontLeftWheel.brakeTorque=p_FrontRightWheel.brakeTorque = 0;
        m_rigidbody.drag = 0;
        if(a_Reverse)
        {
            if(m_rigidbody.velocity.magnitude*3.6<5)
            {
                p_FrontLeftWheel.motorTorque=p_FrontRightWheel.motorTorque = -p_motorForce*5;
            }
            else
            {
                p_FrontLeftWheel.motorTorque=p_FrontRightWheel.motorTorque = -p_motorForce/10;
            }
        }
            
        else
        {
            if(m_rigidbody.velocity.magnitude*3.6<5)
            {
                p_FrontLeftWheel.motorTorque=p_FrontRightWheel.motorTorque = p_motorForce*5;
                p_RearLeftWheel.motorTorque=p_RearRightWheel.motorTorque = p_motorForce*5;
            }
            else
            {
                p_FrontLeftWheel.motorTorque=p_FrontRightWheel.motorTorque = p_motorForce;
                p_RearLeftWheel.motorTorque=p_RearRightWheel.motorTorque = 0;
            }
        }
    }
    public bool GetDistance(Vector3 a_target, float a_tol)
    {
        bool _reache=(Vector3.Distance(a_target,transform.position)<a_tol)?true:false;
        return _reache;
    }
}
