/* Inspired from https://github.com/PointCloudLibrary/pcl */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

namespace PCDlib
{
    public class PCDFile
    {
        private string m_path;

        /*Parameterized Constructor */
        public PCDFile(string a_file)
        {
            m_path = a_file;
            BinaryWriter _saveFile = new BinaryWriter(new FileStream(m_path, FileMode.Create,
                                                                    FileAccess.ReadWrite,
                                                                    FileShare.None), Encoding.UTF8, true);
            _saveFile.Write("# PCD file\n");
            _saveFile.Close();
        }

        public void SetHeader(PCDPointCloud a_pointCloud)
        {
            int _nbPoint = a_pointCloud.GetNbPointStr();
            string _header = "VERSION 0.7\nFIELDS x y z\nSIZE 8 8 8\nTYPE F F F\nCOUNT 1 1 1\nWIDTH " + _nbPoint.ToString() + "\nHEIGHT 1\nVIEWPOINT 0 0 0 1 0 0 0\n" + "POINTS " + _nbPoint.ToString() + "\nDATA ascii\n";
            using (var _Stream = File.Open(m_path, FileMode.Truncate))
            {
                using (var writer = new BinaryWriter(_Stream, Encoding.ASCII,false))
                {
                    writer.Write(_header); 
                }
            }
        }

        public void AddPntCloudParallele(PCDPointCloud a_pointCloud, string a_newpath)
        {
            string _ListPoint = a_pointCloud.GetPointStr();
            int _nbPoint = a_pointCloud.GetNbPointStr();
            using (var _Stream = File.Open(a_newpath, FileMode.Append))
            {
                using (var writer = new BinaryWriter(_Stream, Encoding.ASCII, false))
                {
                    writer.Write(_ListPoint);
                }
            }
        }

        public void AddPointCloudCoordonate(PCDPointCloud a_pointCloud, string a_newpath)
        {
            StringBuilder _ListPoint = new StringBuilder(a_pointCloud.GetPointStr());
            int _nbPoint = a_pointCloud.GetNbPointStr();
            StringBuilder _header = new StringBuilder("VERSION .7\nFIELDS x y z\nSIZE 8 8 8\nTYPE F F F\nCOUNT 1 1 1\nWIDTH " + _nbPoint.ToString() + "\nHEIGHT 1\nVIEWPOINT 0 0 0 1 0 0 0\n" + "POINTS " + _nbPoint.ToString() + "\nDATA ascii\n");
            
            using (var _Stream = File.Open(a_newpath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(_Stream, Encoding.ASCII,false))
                {
                    writer.Write(_header.ToString() + _ListPoint.ToString());
                }
            }   
        }


        public void OverwritePointCloudCoordonate(PCDPointCloud a_pointCloud)
        {            
            string _ListPoint = a_pointCloud.GetPointStr();
            int _nbPoint = a_pointCloud.GetNbPointStr();
            string _header = "VERSION .7\nFIELDS x y z\nSIZE 8 8 8\nTYPE F F F\nCOUNT 1 1 1\nWIDTH " + _nbPoint.ToString() + "\nHEIGHT 1\nVIEWPOINT 0 0 0 1 0 0 0\n" + "POINTS " + _nbPoint.ToString() + "\nDATA ascii\n";
            
            using (var _Stream = File.Open(m_path, FileMode.Truncate))
            {
                using (var writer = new BinaryWriter(_Stream, Encoding.ASCII,false))
                {
                    writer.Write(_header + _ListPoint + "end");
                }
            }   
        }

    }

    public class PCDPointCloud
    {

        /***************
         * Constructor *
         ***************/
        public PCDPointCloud()
        {
            m_pointCloudStr = new PointCloudStr();
        }



        /**********
         * Getter *
         **********/

        public String GetPointStr() => m_pointCloudStr.p_pointstr;

        public int GetNbPointStr()
        {
            return m_pointCloudStr.p_NbPoints;
        }

        public PointCloudStr GetPntCloud() => m_pointCloudStr;
        /**********
         * Setter *
         **********/

        public void AddPointStr(Point a_point)
        {
            m_pointCloudStr.p_NbPoints += 1;
            m_pointCloudStr.p_pointstr += a_point.GetX() + " " + a_point.GetY() + " " + a_point.GetZ()+"\n";

        }

        public void ClearPointCloudStr()
        {
            m_pointCloudStr.p_NbPoints=0;
            m_pointCloudStr.p_pointstr="";
        }
        
        public void SetCopy(PCDPointCloud a_pntcloud)
        {
            m_pointCloudStr.p_NbPoints= a_pntcloud.GetPntCloud().p_NbPoints;
            m_pointCloudStr.p_pointstr = a_pntcloud.GetPntCloud().p_pointstr ;
        }
        /*************
         * Attributs *
         *************/

        private PointCloudStr m_pointCloudStr;

    }

    public class PointCloudStr
    {
        public int p_NbPoints;
        public string p_pointstr;

        public PointCloudStr()
        {
            p_NbPoints=0;
            p_pointstr="";
        }
    }

    public class Point
    {
        private float m_x;
        private float m_y;
        private float m_z;

        public Point(float a_x, float a_y, float a_z)
        {
            this.m_x = a_x;
            this.m_y = a_y;
            this.m_z = a_z;
        }

        public void SetCoordonates(float a_x, float a_y, float a_z)
        {
            this.m_x = a_x;
            this.m_y = a_y;
            this.m_z = a_z;
        }

        public float GetX()
        {
            return this.m_x;
        }

        public float GetY()
        {
            return this.m_y;
        }

        public float GetZ()
        {
            return this.m_z;
        }
    }
}