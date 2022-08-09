using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CreateFilePath
        {
            private string m_pathLabeling;
            private string m_pathPCD;
            private string m_finalPathLabeling;
            private string m_finalPathPCD;
            public CreateFilePath(string a_pathLabeling,string a_pathPCD)
            {
                m_pathLabeling=a_pathLabeling;
                m_pathPCD=a_pathPCD;
                m_finalPathLabeling=null;
                m_finalPathPCD=null;
            }

            public void CreateName()
            {
                string _filename=(DateTime.Now).ToString("yyyyMMddHHmmssffff");

                m_finalPathLabeling=System.IO.Path.Combine(m_pathLabeling,_filename+".txt");
                m_finalPathPCD=System.IO.Path.Combine(m_pathPCD,_filename+".pcd");
            }

            public string GetPathLabeling()=>m_finalPathLabeling;
            public string GetPathPCD()=>m_finalPathPCD;
        }
