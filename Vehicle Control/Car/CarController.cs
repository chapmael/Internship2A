using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    void Start(){
        m_rigidbody=GetComponent<Rigidbody>();
        m_centerOfMass=GameObject.Find("mass");
        m_rigidbody.centerOfMass=m_centerOfMass.transform.localPosition;
    }
    /*Get the input*/
    private void GetInput()
    {
        m_HorizontalInput=Input.GetAxis("Horizontal")*p_SensibilityH;
        m_VerticalInput=Input.GetAxis("Vertical")*p_SensibilityV;
        //m_BreakInput=Input.GetKey("s");
    }



    /*Wheel Steer*/
    private void WheelSteer()
    {
        //acerman steering formula
		//steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
        //One wheel turn more than the other one 
        if (m_HorizontalInput > 0 ) {
				//rear tracks size is set to 1.5f       wheel base has been set to 2.55f
            p_FrontDriverW.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * m_HorizontalInput;
            p_FrontPassengerW.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * m_HorizontalInput;
        } else if (m_HorizontalInput < 0 ) {                                                          
            p_FrontDriverW.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * m_HorizontalInput;
            p_FrontPassengerW.steerAngle     = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * m_HorizontalInput;

        } else {
            p_FrontDriverW.steerAngle = 0;
            p_FrontPassengerW.steerAngle  = 0;
        }
        /*
        m_SteeringAngle=p_MaxSteerAngle*m_HorizontalInput;
        p_FrontDriverW.steerAngle=m_SteeringAngle;
        p_FrontPassengerW.steerAngle=m_SteeringAngle;*/
    }


    /*Add down force*/
    private void addDownForce()
    {
         
        m_rigidbody.AddForce(-transform.up * m_DownForceValue * m_rigidbody.velocity.magnitude*10);
        
    }


    /*Set Acceleration*/
    private void Accelerate()
    {
        p_FrontDriverW.motorTorque = m_TotalPower/2;
		p_FrontPassengerW.motorTorque = m_TotalPower/2;
        p_RearDriverW.brakeTorque=p_RearPassengerW.brakeTorque=p_FrontDriverW.brakeTorque=p_FrontPassengerW.brakeTorque= m_MotorBreak;
        p_KPH=m_rigidbody.velocity.magnitude*3.6f;
        
        if(Input.GetKey("s")){
            p_RearDriverW.brakeTorque=p_RearPassengerW.brakeTorque=p_FrontDriverW.brakeTorque=p_FrontPassengerW.brakeTorque=p_BrakePower;
        }
        else{
            p_RearDriverW.brakeTorque=p_RearPassengerW.brakeTorque=p_FrontDriverW.brakeTorque=p_FrontPassengerW.brakeTorque=0;
        }
        if(Input.GetKey("space")){
            p_RearDriverW.brakeTorque=p_RearPassengerW.brakeTorque=p_HandBrakePower;
        }
        else{
            p_RearDriverW.brakeTorque=p_RearPassengerW.brakeTorque=0;
        }
    }



    /*Set MotorForce*/
    private void calculateEnginePower()
    {
        WheelRPM();
        p_motorRPM=Mathf.Min(Mathf.Max(m_minRPM,m_minRPM+(m_WheelRPM*p_FinalDriveRatio*p_GearRatio.Evaluate(p_GearIndex))),m_maxRPM);
        if(m_VerticalInput<=0){

            m_TotalPower=-p_EngineTorque.Evaluate(p_motorRPM)*p_GearRatio.Evaluate(p_GearIndex)*p_FinalDriveRatio*m_rigidbody.velocity.magnitude/10;

            m_MotorBreak=p_EngineTorque.Evaluate(p_motorRPM)*p_GearRatio.Evaluate(p_GearIndex)*p_FinalDriveRatio*1000;
        }
        else{
            m_TotalPower=p_EngineTorque.Evaluate(p_motorRPM)*p_GearRatio.Evaluate(p_GearIndex)*p_FinalDriveRatio*m_VerticalInput;
            m_MotorBreak=0;
        }
        
    }



    /*Wheel RPM calculation*/
    private void WheelRPM()
    {
        m_WheelRPM=(p_RearDriverW.rpm+p_RearPassengerW.rpm)/2;
    }



    /*Change Gear*/
    private void updateGear()
    {
        if (Input.GetKey(KeyCode.KeypadPlus))
        {   
            if(p_GearIndex<5){
                p_GearIndex ++;
                m_ChangeGear=true;
            }
            else {
                m_ChangeGear=false;
            }

        }
        if(Input.GetKey(KeyCode.KeypadMinus))
        {
            if(p_GearIndex>-1){
                p_GearIndex --;
                m_ChangeGear=true;
            }
            else{
                m_ChangeGear=false;
            }

        }
    }



    /*Update for all the wheels*/
    private void UpdateWheelPoses()
	{
		UpdateWheelPose(p_FrontDriverW, p_FrontDriverT);
		UpdateWheelPose(p_FrontPassengerW, p_FrontPassengerT);
		UpdateWheelPose(p_RearDriverW, p_RearDriverT);
		UpdateWheelPose(p_RearPassengerW, p_RearPassengerT);
	}



	private void UpdateWheelPose(WheelCollider i_collider, Transform i_transform)
	{
		Vector3 _pos = i_transform.position;
		Quaternion _quat = i_transform.rotation;

		i_collider.GetWorldPose(out _pos, out _quat);

		i_transform.position = _pos;
		i_transform.rotation = _quat;
	}



    private void TimeChangeGear(){
        if(m_counterChangeGear<10){
            m_counterChangeGear ++;
        }
        else{
            m_ChangeGear=false;
            m_counterChangeGear=0;
        }
    }

    private void updateAutomaticGear(){
        if(p_motorRPM>4800){
            p_GearIndex=(p_GearIndex<5)?p_GearIndex+1:p_GearIndex;
        }
        if(p_motorRPM<2300){
            p_GearIndex=(p_GearIndex>1)?p_GearIndex-1:p_GearIndex;
        }
        if(Input.GetKey("n")){
            p_GearIndex=0;
        }
        if(Input.GetKey("r")){
            p_GearIndex=-1;
        }
        if(Input.GetKey("a")){
            p_GearIndex=1;
        }
    }
    private void GearManagement()
    {
        if(p_automaticGear){
            updateAutomaticGear();
        }
        else{
            if (!m_ChangeGear){
                updateGear();
            }
            else {
                TimeChangeGear();
            }
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetInput();
        WheelSteer();
        addDownForce();
        GearManagement();
        calculateEnginePower();
        Accelerate();
        UpdateWheelPoses();

    }

    /*Private attribut*/
    private float m_HorizontalInput,m_VerticalInput;
    private float m_BreakInput;

    private float m_SteeringAngle;
    private float m_Speed;
    private float m_WheelRPM;
    public float m_TotalPower;
    public float m_MotorBreak;
    private float m_minRPM=700f; 
    private float m_maxRPM=10000f;
    private bool m_ChangeGear=false;
    private int m_counterChangeGear=0;
    private float m_DownForceValue =50f;
    private GameObject m_centerOfMass;
    private Rigidbody m_rigidbody;
    
    /*Hidden public attributs*/
    [HideInInspector]public int p_GearIndex=0;
    [HideInInspector]public float p_KPH;
    [HideInInspector]public float p_motorRPM;
    [HideInInspector]public float p_SensibilityH=1;
    [HideInInspector]public float p_SensibilityV=1;
    [HideInInspector]public bool p_automaticGear=true;
    /*Visible Public Attributs*/
    [Tooltip("WheelCollider from front wheel")]
    public WheelCollider p_FrontDriverW, p_FrontPassengerW;
    [Tooltip("WheelCollider from rear wheel")]
    public WheelCollider p_RearDriverW, p_RearPassengerW;
    [Tooltip("Wheel Transform from rear wheel")]
    public Transform p_FrontDriverT, p_FrontPassengerT;
    [Tooltip("Wheel Transform from rear wheel")]
    public Transform p_RearDriverT, p_RearPassengerT;
    [Tooltip("Wheel max steer angle")]
    public float p_MaxSteerAngle = 38f;
    [Tooltip("EnginePower Curve")]
    public AnimationCurve p_EngineTorque;
    [Tooltip("GearRatio Curve")]
    public AnimationCurve p_GearRatio;
    [Tooltip("Final Drive Ratio")]
    public float p_FinalDriveRatio=5f;
    [Tooltip("HandBrake Power")]
    public float p_HandBrakePower=1000000000f;
    [Tooltip("Brake Power")]
    public float p_BrakePower=10000f;
    public float radius = 6;



}
