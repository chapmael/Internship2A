using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PCDlib;


public class SavePointPCD
{
    private PCDFile m_file;
    private PCDPointCloud m_pointCloud;
    private readonly object LockPointCloud=new object();

    /*Parameterized Constructor */
    public SavePointPCD(string a_file)
    {
        m_file = new PCDFile(a_file);
        lock(LockPointCloud)
        {
            m_pointCloud = new PCDPointCloud();
            
        }
        
    }

    public void SaveNewPoint(Point a_point)
    {
        if(a_point!=null)
        {
            lock(LockPointCloud)
            {
                m_pointCloud.AddPointStr(a_point); 
            }
        }
    }

    public void SaveData(bool a_trainingOrUse, string a_path=null)
    {
        PCDPointCloud _localPCDPointCloud = new PCDPointCloud();
        lock (LockPointCloud)
        {
            _localPCDPointCloud.SetCopy(m_pointCloud);
        }
        m_pointCloud.ClearPointCloudStr();

        if (a_trainingOrUse)
        {
            if(_localPCDPointCloud != null)
            {
                m_file.AddPointCloudCoordonate(_localPCDPointCloud, a_path);
            }
        }

        else
        {
            //Debug.Log(m_pointCloud.GetNbPoint());
            if(_localPCDPointCloud != null){
                m_file.OverwritePointCloudCoordonate(_localPCDPointCloud);
            }
        }
        
    }
}