using UnityEngine;
using System.Collections;
using Udp;
public class VMS_Engine : MonoBehaviour
{
    public int TxPort = 9846;
    public int RxPort = 9849;

    public void Start()
    {
        cRC_UDP_IO m_UdpIf = new cRC_UDP_IO(TxPort, RxPort);

        // Create message handlers
        cMsgHandlerEOD m_msgHandlerEOD = new cMsgHandlerEOD(0);
        cMsgHandlerLRF    m_msgHandlerLRF    = new cMsgHandlerLRF( 0 );
        cMsgHandlerSensor m_msgHandlerSensor = new cMsgHandlerSensor(0);

        // Add these to our UDP interface
        m_UdpIf.addMsgHandler(m_msgHandlerEOD);
        m_UdpIf.addMsgHandler(m_msgHandlerSensor);
        m_UdpIf.addMsgHandler(m_msgHandlerLRF);

        // Kick off our receiving
        m_UdpIf.initialiseVMS();
    }
}