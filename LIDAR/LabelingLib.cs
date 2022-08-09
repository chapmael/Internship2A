using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;


namespace Enumeration
{
    namespace Labeling
    {   
        public class LabelingFile
        {
            /* Add information */

            public void SaveLabelData(LabelingData a_info,string a_newpath)
            {
                string _dataToSave=a_info.GetLabelingData();
                if(_dataToSave!="")
                {
                    string _header = "# Labeling File\nLidar Coordonates (x y z)(float)   Hit Object Coordonates (x y z)(float)   Object class (string)   Distance(float)\n";
                
                    using (var _Stream = File.Open(a_newpath, FileMode.Create))
                    {
                        using (var writer = new BinaryWriter(_Stream, Encoding.ASCII,false))
                        {
                            writer.Write(_header+_dataToSave);
                        }
                    } 
                }
   
            }
        }


        public class LabelingData
        {   
            private string m_labelingData;

            public LabelingData()
            {
                m_labelingData="";
            }

            public LabelingData(Vector3 a_LidarCoordonates,Vector3 a_HitObjectCoordonates, AssignClass a_ClassObject, float a_distance )
            {
                m_labelingData=a_LidarCoordonates.ToString()+" "+a_HitObjectCoordonates.ToString()+" "+a_ClassObject.GetAssignClass()+" "+a_distance+"\n";
            }

            public LabelingData(Vector3 a_LidarCoordonates, Vector3 a_HitObjectCoordonates, string a_ClassObject, float a_distance)
            {
                m_labelingData = a_LidarCoordonates.ToString() + " " + a_HitObjectCoordonates.ToString() + " " + a_ClassObject+ " " + a_distance + "\n";
            }

            public LabelingData(Vector3 a_LidarCoordonates, Vector3 a_BoundingBoxCenter, Vector3 a_BoundingBoxDim, string a_ClassObject, float a_distance )
            {
                m_labelingData=a_LidarCoordonates.ToString()+" "+ a_BoundingBoxCenter.ToString()+" "+ a_BoundingBoxDim.ToString()+" "+a_ClassObject + " "+a_distance+"\n";
            }

            /*********
            * Setter *
            **********/
            public void AddLabelingData(LabelingData a_labelData)
            {
                m_labelingData+=a_labelData.GetLabelingData();
            }

            public void ClearDataLabel()
            {
                m_labelingData="";
            }


            /**********
            * Guetter *
            ***********/
            public string GetLabelingData()
            {
                return m_labelingData;
            }

            
        }

        public class AssignClass
        {
            private string m_class;
            public AssignClass(string a_gameObjectName)
            {
                m_class=FindClass(a_gameObjectName);
            }

            private string FindClass(string a_Name)
            {   
                if(a_Name.Contains("Road")){return "Road";}
                if(a_Name.Contains("_Tree_")){return "Tree";}
                if(a_Name.Contains("Intersection")){return "Road_Intersection";}
                if(a_Name.Contains("_Stone_")){return "Stone";}
                if(a_Name.Contains("Fense")){return "Fense";}
                if(a_Name.Contains("Wagon")){return "Wagon";}
                if(a_Name.Contains("Forge")){return "House";}
                if(a_Name.Contains("House")){return "House";}
                else{return "Terrain";}
            }

            public string GetAssignClass()=>m_class;
        }
        public class Box
        {
            private string m_info;

            public Box(Vector3 a_BoxCenter, Vector3 a_directionObject, float a_w, float a_h, float a_d)
            {
                m_info=a_BoxCenter.ToString()+" "+a_directionObject.ToString()+" "+a_w+" "+a_h+" "+a_d;
            }

            public Box()
            {
                m_info="";
            }

            /**********
            * Guetter *
            ***********/

            public string GetStr()
            { 
                return m_info;
            }
            /*********
            * Setter *
            **********/

            public void SetBox(Vector3 a_BoxCenter, Vector3 a_directionObject, float a_w, float a_h, float a_d)
            {
                m_info=a_BoxCenter.ToString()+" "+a_directionObject.ToString()+" "+a_w+" "+a_h+" "+a_d;
            }
        }
        
    }
}

