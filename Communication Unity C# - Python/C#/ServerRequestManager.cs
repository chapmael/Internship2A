using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Enumeration;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;

namespace ServerManager
{

    public class ServerRequestManager: MonoBehaviour
    {   
        private string m_message = null;
        private bool m_gotMessage = false;
        private System.Diagnostics.Process m_server;

        public CarController CC;
        public string p_ServerPath ;




        void Start()
        {
            p_ServerPath=PlayerPrefs.GetString("ServerPath");

        }   

        public void StartServer(bool a_withWindow)
        {
            m_server=new System.Diagnostics.Process();
            if(a_withWindow)
            {
                m_server.StartInfo.CreateNoWindow = true;
                m_server.StartInfo.UseShellExecute = false;
                
            }
            m_server.StartInfo.FileName=p_ServerPath;
            m_server.Start();
        }

        public void StopServer()
        {
            m_server.Kill();
        }

        public void SendAndReceiveRequest(string a_sendRequest)
        {
            ForceDotNet.Force();

            using(RequestSocket _client = new RequestSocket())
            {
                _client.Connect("tcp://localhost:5555");
                _client.SendFrame("1");

                while(!m_gotMessage){
                    m_gotMessage=_client.TryReceiveFrameString(out m_message);
                }
                //Debug.Log("Nombre Recu :  " + m_message);
            }
            NetMQConfig.Cleanup();
        }


        void OnDestroy ()
        {
            if(!m_server.HasExited)
            {
                m_server.Kill();
            } 
        }
    }
}

