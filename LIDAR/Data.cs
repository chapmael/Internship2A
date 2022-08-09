using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PCDlib;
using Enumeration.Labeling;

public class Data
{
    private Vector3 m_point;
    private Point m_pointP;
    private LabelingData m_labelingdata;
    private Vector3 m_pos;
    private Vector3 m_view;

    public Data(LabelingData a_labelingData, Vector3 a_point, Vector3 a_pos, Vector3 a_view)
    {
        m_point=a_point;
        m_labelingdata=a_labelingData;
        m_pos = a_pos;
        m_view = a_view;
    }

    public Data(LabelingData a_labelingData, Point a_point)
    {
        m_pointP = a_point;
        m_labelingdata = a_labelingData;
    }

    public Point GetPoint() => m_pointP;
    public Vector3 GetPointV()=>m_point;
    public LabelingData GetLabel()=>m_labelingdata;
    public Vector3 GetPos() => m_pos;
    public Vector3 GetView() => m_view;
}
