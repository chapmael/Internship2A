using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumeration.Labeling;

public class SaveLabelingData
{
    private LabelingFile m_labelingfile;
    private LabelingData m_labelingData;
    private readonly object LockLabelData=new object();

    public SaveLabelingData()
    {
        m_labelingfile=new LabelingFile();
        lock(LockLabelData)
        {
            m_labelingData=new LabelingData();
        }
    }

    public void SaveNewData(LabelingData a_labelingData)
    {
        if(a_labelingData!=null)
        {
            lock(LockLabelData)
            {
                m_labelingData.AddLabelingData(a_labelingData);
            }
        }
    }

    public void SaveLabelingDataInFile(string a_path)
    {
        lock(LockLabelData)
        {
            m_labelingfile.SaveLabelData(m_labelingData,a_path);
            m_labelingData.ClearDataLabel();
        }
    }
}
