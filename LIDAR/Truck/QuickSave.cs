using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using Enumeration.Labeling;
using System.Threading.Tasks;
using System.IO;

public class QuickSave 
{
    private StringBuilder m_pcd;
    private StringBuilder m_label;
    private int m_indexSave;

    private string m_PCDFile;
    private string m_LabelFile;




    public QuickSave(string a_PCDPPath, string a_LabelPath, Vector3 a_pos, Vector3 a_view, int a_indexSave = 100)
    {
        m_pcd = new StringBuilder();
        m_label = new StringBuilder();
        m_indexSave = a_indexSave;

        m_PCDFile = a_PCDPPath;
        m_LabelFile = a_LabelPath;

        Parallel.Invoke(() =>
                            {
                                string _pos = a_pos.ToString();
                                string _view = a_view.ToString();
                                string _header = "VERSION 0.7\nFIELDS x y z\nSIZE 8 8 8\nTYPE F F F\nCOUNT 1 1 1\nWIDTH 16000" + "\nHEIGHT 1\nVIEWPOINT "+ _pos.Substring(1, _pos.Length - 2)+" 1 "+_view.Substring(1, _view.Length - 2)+ "\n" + "POINTS 16000" + "\nDATA ascii\n";
                                using (var _Stream = File.Open(m_PCDFile, FileMode.Create))
                                {
                                    using (var writer = new BinaryWriter(_Stream, Encoding.ASCII, false))
                                    {
                                        writer.Write(_header);
                                    }
                                }
                            },
                        () =>
                            {
                                if (m_label != null)
                                {
                                    string _header = "# Labeling File\nLidar Coordonates (x y z)(float)   BoundingBox Center (x y z)(float)   BoundingBox size (x y z)(float)   Object class (string)   Distance(float)\n";
                                    using (var _Stream = File.Open(m_LabelFile, FileMode.Create))
                                    {
                                        using (var writer = new BinaryWriter(_Stream, Encoding.ASCII, false))
                                        {
                                            writer.Write(_header);
                                        }
                                    }
                                }

                            });
    }

    public void SavePartPoint(bool a_WithLabel, string a_PCDPPath, string a_LabelPath = null)
    {
        Parallel.Invoke(() =>
                                {
                                    using (var _Stream = File.Open(m_PCDFile, FileMode.Append))
                                    {
                                        using (var writer = new BinaryWriter(_Stream, Encoding.ASCII, false))
                                        {
                                            writer.Write(m_pcd.ToString());
                                        }
                                    }
                                    m_pcd.Clear();
                                },
                            () =>
                                {
                                    if (a_WithLabel)
                                    {
                                        using (var _Stream = File.Open(m_LabelFile, FileMode.Append))
                                        {
                                            using (var writer = new BinaryWriter(_Stream, Encoding.ASCII, false))
                                            {
                                                writer.Write(m_label.ToString());
                                            }
                                        }
                                        
                                    }
                                    m_label.Clear();
                                });
    }

    public void AddPoint(bool a_WithLabel, Vector3 a_point, LabelingData a_labelingData=null)
    {
        if (!a_WithLabel)
        {
            string _point = a_point.ToString();
            m_pcd.Append(_point.Substring(1, _point.Length - 2)+"\n");
        }
        else
        {
            string _point = a_point.ToString();
            m_pcd.Append(_point.Substring(1, _point.Length - 2) + "\n");
            m_label.Append(a_labelingData.GetLabelingData());
        }
    }


    public void SaveOverWrite(Vector3 a_pos, Vector3 a_view, string a_PCDPPath)
    {
        SavePartPoint(false, a_PCDPPath);
        string _pos = a_pos.ToString();
        string _view = a_view.ToString();
        string _header = "VERSION 0.7\nFIELDS x y z\nSIZE 8 8 8\nTYPE F F F\nCOUNT 1 1 1\nWIDTH 16000" + "\nHEIGHT 1\nVIEWPOINT " + _pos.Substring(1, _pos.Length - 2) + " 1 " + _view.Substring(1, _view.Length - 2) + "\n" + "POINTS 16000" + "\nDATA ascii\n";
        using (var _Stream = File.Open(a_PCDPPath, FileMode.Truncate))
        {
            using (var writer = new BinaryWriter(_Stream, Encoding.ASCII, false))
            {
                writer.Write(_header);
            }
        }
    }

    public void Save(bool a_WithLabel, Vector3 a_pos, Vector3 a_view, string a_PCDPPath, string a_LabelPath=null)
    {
        SavePartPoint(a_WithLabel, a_PCDPPath, a_LabelPath);

        m_PCDFile = a_PCDPPath;
        m_LabelFile = a_LabelPath;

        Parallel.Invoke(() =>
                            {
                                string _pos = a_pos.ToString();
                                string _view = a_view.ToString();
                                string _header = "VERSION 0.7\nFIELDS x y z\nSIZE 8 8 8\nTYPE F F F\nCOUNT 1 1 1\nWIDTH 16000" + "\nHEIGHT 1\nVIEWPOINT " + _pos.Substring(1, _pos.Length - 2) + " 1 " + _view.Substring(1, _view.Length - 2) + "\n" + "POINTS 16000" + "\nDATA ascii\n";
                                using (var _Stream = File.Open(m_PCDFile, FileMode.Create))
                                {
                                    using (var writer = new BinaryWriter(_Stream, Encoding.ASCII, false))
                                    {
                                        writer.Write(_header);
                                    }
                                }
                            },
                        () =>
                            {
                                if (a_WithLabel)
                                {
                                    string _header = "# Labeling File\nLidar Coordonates (x y z)(float)   BoundingBox Center (x y z)(float)   BoundingBox size (x y z)(float)   Object class (string)   Distance(float)\n";
                                    using (var _Stream = File.Open(m_LabelFile, FileMode.Create))
                                    {
                                        using (var writer = new BinaryWriter(_Stream, Encoding.ASCII, false))
                                        {
                                            writer.Write(_header);
                                        }
                                    }
                                }
                            });
    }
}





