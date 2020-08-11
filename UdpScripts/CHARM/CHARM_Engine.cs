using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Xml;
using Udp;

public class CHARM_Engine : MonoBehaviour
{

    public int TxPort = 9876;
    public int RxPort = 9879;

    // Start is called before the first frame update
    public void Start()
    {
        cRC_UDP_IO m_UdpIf = new cRC_UDP_IO(TxPort, RxPort);


        rMsgHandlerEOD m_MsgHandlerEOD = new rMsgHandlerEOD(0);
        //Set up our subsystems
        m_UdpIf.addMsgHandler(m_MsgHandlerEOD);



        //Kickof Sending and reciving
        m_UdpIf.initialiseCHARM();
    }
     
    private IEnumerator Connect(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            print("WaitAndPrint" + Time.time);
        }
    }
}
