using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class SavePoint
{   
/*Constructors*/
    /*Default Constructor*/
    public SavePoint()
    {
        m_path=@"C:\Users\chapm\Documents\unity\save_point\Test.txt";
        BinaryWriter _saveFile=new BinaryWriter(new FileStream(m_path , FileMode.Create),Encoding.UTF8,true);
        _saveFile.Write("x y z frame R G B \n");
        _saveFile.Close();
        m_txt=new string("");
    }

    /*Parameterized Constructor 2*/
    public SavePoint(string a_file)
    {
        m_path=a_file;
        BinaryWriter _saveFile=new BinaryWriter(new FileStream(m_path , FileMode.OpenOrCreate,
                                                                FileAccess.ReadWrite, 
                                                                FileShare.None),Encoding.UTF8,true);
        _saveFile.Write("x y z frame R G B\n");
        _saveFile.Close();
        m_txt=new string("");

    }

    /*Getter*/
    public bool IsAString()
    {
        bool _var=(string.IsNullOrEmpty(m_txt))? true:false;
        return _var;
    }
/*Setter*/


    public void SaveNewLine(float a_x,float a_y, float a_z,float a_frame, Color a_color)
    {
        string _info =a_x+" "+a_y+" "+a_z+" "+a_frame+" "+(int)(a_color.r*256)+" "+(int)(a_color.g*256)+" "+(int)(a_color.b*256)+"\n";
        m_txt+=_info;
    }
/*Save Fonction */

    public void SaveEleQueue(string a_txt){
        File.AppendAllText(m_path,a_txt);  
    }

    public void FinalSave(){

        File.AppendAllText(m_path,m_txt+Environment.NewLine);
        m_txt="";

    }

    private string m_path;

    private string m_txt;
}
