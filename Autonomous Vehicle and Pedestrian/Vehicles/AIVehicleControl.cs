using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVehicleControl : MonoBehaviour
{
    public WaypointNavigatorVehicle WNV;
    public TruckController TC;
    public Vector3 p_target;
    public float p_tol;

    private bool m_ReverseGear;
    public bool m_Stop;
    public int m_counterStop;
    public bool m_Brake;
    // Start is called before the first frame update
    void Start()
    {
        //agent = GetComponent<UnityEngine.AI.NavMeshObstacle>();
        //WNV = GetComponent<WaypointNavigatorVehicle>();
        //TC = GetComponent<TruckController>();
        WNV.UpdatePosition();
        m_counterStop=Random.Range(60,2000);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(p_target != null)
        {
            if(TC.GetDistance(p_target,p_tol))
            {
                WNV.UpdatePosition();
                TC.Move(p_target,m_ReverseGear,m_Stop,m_Brake);

            }
            else
            {   
                if(m_Stop)
                {
                    if(m_counterStop<=0)
                    {
                        m_Stop=false;
                        m_counterStop=Random.Range(60,2000);
                    }
                    m_counterStop--;
                }
                TC.Move(p_target,m_ReverseGear,m_Stop,m_Brake);
            }
        }
        else
        {
            TC.StopTruck();
        }
    }

    public void SetTarget(in Vector3 a_target, bool a_Stop, bool a_Reverse, bool a_Brake)
    {
        p_target = a_target;
        m_ReverseGear = a_Reverse;
        m_Stop = a_Stop;
        m_Brake=a_Brake;
    }
}
