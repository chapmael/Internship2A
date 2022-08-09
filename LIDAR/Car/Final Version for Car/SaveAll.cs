using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Enumeration.Labeling;
using PCDlib;

public class SaveAll
{
    private SavePointPCD m_SPPCD;
    private SaveLabelingData m_SLD;
    
    private string m_pathLabel;
    private string m_pathPCD;
    private bool m_trainingOrUse;


    /* Constructor*/
    public SaveAll(string a_FileName)
    {
        m_SLD=new SaveLabelingData();
        m_SPPCD=new SavePointPCD(a_FileName);
    }


    /* Main Function */
    public void Add(LabelingData a_labelingData, Point a_point)
    {
        Parallel.Invoke(()=>
                        {
                            m_SLD.SaveNewData(a_labelingData);
                        },
                        ()=>
                        {
                            m_SPPCD.SaveNewPoint(a_point);
                        });
    }

    public void Save(bool a_trainingOrUse,string a_pathPCD=null,string a_pathLabel=null)
    {
        if(a_trainingOrUse)
        {
            Parallel.Invoke(()=>
                        {
                            m_SLD.SaveLabelingDataInFile(a_pathLabel);
                        },
                        ()=>
                        {
                            m_SPPCD.SaveData(a_trainingOrUse,a_pathPCD);
                        });
        }
        else 
        {
            m_SPPCD.SaveData(a_trainingOrUse);
        }
        
    }
}
