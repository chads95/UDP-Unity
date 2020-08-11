using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace Udp
{
    class cRC_UDP_IO : MonoBehaviour
    {
        public static UdpClient m_udpClient;
        public static byte[] rx_buf;

        public static IPEndPoint server;
        public static IPEndPoint client;
        public bool recived;

        private static Mutex mutex = new Mutex();
        [HideInInspector]
        public static bool ThreadCheck = true;
        private bool initalize;

        private List<cVMS_MsgHandler> lMsgHandlers = new List<cVMS_MsgHandler>();

        public List<rCHARM_MsgHandler> rMsgHandlers = new List<rCHARM_MsgHandler>();

        public XMLReader xMLReader;


        public List<cDataItem> vecDataItems = new List<cDataItem>();
        public static List<List<RC_Item>> m_pageList = new List<List<RC_Item>>();
        List<byte> startMsg = new List<byte>();
        int startMsgSize;
        bool sendMsg = false;


        public Dictionary<int, rCHARM_MsgHandler.PeriodicMessageData> m_dicPeriodicRequests = new Dictionary<int, rCHARM_MsgHandler.PeriodicMessageData>();

        public Dictionary<byte, rCHARM_MsgHandler.PERIODIC_DEFN> m_dictPeriodic = new Dictionary<byte, rCHARM_MsgHandler.PERIODIC_DEFN>();

        public Dictionary<int, List<ItemWithType>> m_mapItemList = new Dictionary<int, List<ItemWithType>>();
        void Awake()
        {
            ThreadCheck = true;
            initalize = false;
            recived = false;
        }

        public cRC_UDP_IO( int nTxPort, int nRxPort )
        {
            // Setup a byte array for the rx msg
            rx_buf = new byte[255];

            // Create a new IP endpoint with the specified port number
            server = new IPEndPoint(IPAddress.Any, nRxPort);

            // Create a UDP client and bind to specified endpoint
            m_udpClient = new UdpClient(server);

            // Specify the sender endpoint
            client = new IPEndPoint(IPAddress.Parse("127.0.0.1"), nTxPort);
        }

        public void initialiseVMS()
        {
            try
            {
                if (m_udpClient.Client.Poll(50000, SelectMode.SelectRead))
                {
                    rx_buf = m_udpClient.Receive(ref client);
                    Debug.Log("Rx: 0X" + ByteArrayToString(rx_buf));
                }
                else
                {
                    m_udpClient.Client.Close();
                    throw new ApplicationException("Failed to connect to CHARM");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }

            {
                rxMsgHandler(rx_buf, lMsgHandlers);
                BackgroundListener longTest = new BackgroundListener(m_udpClient, rx_buf, client, lMsgHandlers);
                Thread backgroundThread =
                    new Thread(new ThreadStart(longTest.RunLoop));
                backgroundThread.IsBackground = true;
                backgroundThread.Start();
                Debug.Log("VMS thread");
            }
        }

        public void initialiseCHARM()
        {
            XMLReader xmlReader = new XMLReader(rMsgHandlers);

            //Read in our XML file
            xmlReader.ReadXML(Application.dataPath + "/dataitems.xml", this);

            try
            {
                if (m_udpClient.Client.Poll(50000, SelectMode.SelectRead))
                {
                    rx_buf = m_udpClient.Receive(ref client);
                    recived = true;
                    Debug.Log("Rx: 0X" + ByteArrayToString(rx_buf));
                }
                else
                {
                    m_udpClient.Close();
                    throw new ApplicationException("Failed to connect to CHARM");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }


            {
                int nLength = rx_buf.Length;
                BackgroundListener longTest = new BackgroundListener(m_udpClient, rx_buf, client, this);
                Thread backgroundThread =
                    new Thread(new ThreadStart(longTest.RunLoop));
                backgroundThread.IsBackground = true;
                backgroundThread.Start();
                Debug.Log("CHARM thread");
            }
        }

        public class BackgroundListener
        {
            UdpClient newsock;
            byte[] rx_buf;
            IPEndPoint client;
            cRC_UDP_IO uDP_IO;


            public BackgroundListener(UdpClient newsock, byte[] rx_buf, IPEndPoint client, List<cVMS_MsgHandler> lMsgHandlers)
            {
                this.newsock = newsock;
                this.rx_buf = rx_buf;
                this.client = client;
            }

            public BackgroundListener(UdpClient newsock, byte[] rx_buf, IPEndPoint client, cRC_UDP_IO uDP_IO)
            {
                this.newsock = newsock;
                this.rx_buf = rx_buf;
                this.client = client;
                this.uDP_IO = uDP_IO;
            }

            public void RunLoop()
            {
                Debug.Log("background thread Created");
                try
                {
                    while (ThreadCheck == true)
                    {
                        mutex.WaitOne();
                        rx_buf = newsock.Receive(ref client);
                        int nLength = (int)rx_buf.Length;
                        //if (lMsgHandlers.Count > 0)
                        //    rxMsgHandler(rx_buf, lMsgHandlers);
                        //else if (rMsgHandlers.Count > 0)
                        //    rxMsgHandler(rx_buf, rMsgHandlers);
                        //    rCHARM_Msg.handleCharmMessage(rx_buf, nLength);
                        uDP_IO.handleCharmMessage(rx_buf, nLength);
                        
                        LogHelper.Log(LogTarget.File, "Rx: 0X" + ByteArrayToString(rx_buf));
                        LogHelper.Log(LogTarget.DebugLog,"Rx: 0X" + ByteArrayToString(rx_buf));
                        //Thread.Sleep(5);
                        mutex.ReleaseMutex();
                    }
                }
                catch(Exception e)
                {
                    LogHelper.Log(LogTarget.DebugLog, e.ToString());
                }
            }
        }

        public void addMsgHandler( cVMS_MsgHandler cMsgHandler )
        {
            // Store this msg handler in our list
            lMsgHandlers.Add( cMsgHandler );
        }

        public void addMsgHandler(rCHARM_MsgHandler cMsgHandler)
        {
            // Store this msg handler in our list
            rMsgHandlers.Add(cMsgHandler);
        }

        public static void rxMsgHandler( byte[] msg_buf, List<cVMS_MsgHandler> lMsgHandlers)
        {

            bool MsgHandled = false;
            mutex.WaitOne();
            // Pass on this msg to the msg handlers
            foreach ( var msg_handler in lMsgHandlers )
            {
                //TODO: Need to check against the msg handlers ID!
                if (msg_buf[1] == msg_handler.m_cSubSysId
                    && msg_buf[2] == msg_handler.m_cSubSysIndex) //Debug.Log("Msg_Buf[1], Msg_Buf[2]");
                {
                    msg_handler.handleRxMessage(m_udpClient, msg_buf, client);
                    MsgHandled = true;
                }
            }
            mutex.ReleaseMutex();

            if (!MsgHandled && lMsgHandlers.Count > 0)
            {
                //    //Assume we have a connection byte!
                cVMS_MsgHandler.sendUnknownResp();
                Debug.Log("unknownsRespVMS"); 
            }
        }

        public static void rxMsgHandler(byte[] msg_buf, List<rCHARM_MsgHandler> lMsgHandlers)
        {
            bool MsgHandled = false;
            mutex.WaitOne();
            // Pass on this msg to the msg handlers
            foreach (var msg_handler in lMsgHandlers)
            {
                if (msg_buf[1] == msg_handler.m_cSubSysId
                    && msg_buf[2] == msg_handler.m_cSubSysIndex) Debug.Log(msg_buf[1] + msg_buf[2]);
                {
                    msg_handler.handleRxMessage(m_udpClient, msg_buf, client);
                    MsgHandled = true;
                }
            }
            mutex.ReleaseMutex();

            if (!MsgHandled && lMsgHandlers.Count > 0)
            {
                //Assume we have a connection byte!
                cVMS_MsgHandler.sendUnknownResp();
                Debug.Log("unknownsRespCHARM");
            }
        }


        // Converts a byte array to a string
        public static string ByteArrayToString( byte[] ba )
        {
            return BitConverter.ToString( ba ).Replace( "-", " 0x" );
        }

        void OnDestroy()
        {
            ThreadCheck = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ///RC Helpers
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        ///


        public static RC_Byte ExtractByte(ref byte[] pMsgBuf, int nCurrentByte)
        {
            RC_Byte retVal = default;
            retVal.bChanged = true;

            retVal.nData = (byte)pMsgBuf[nCurrentByte++];

            return retVal;
        }

        public static RC_U16 ExtractU16(ref byte[] pMsgBuf, int nCurrentByte)
        {
            RC_U16 retVal = default;
            retVal.bChanged = true;

            retVal.nData  = (ushort)pMsgBuf[nCurrentByte++];
            retVal.nData += (ushort)pMsgBuf[nCurrentByte++];

            return retVal;
        }

        public static RC_S16 ExtractS16(ref byte[] pMsgBuf, int nCurrentByte)
        {
            RC_S16 retVal = default;
            retVal.bChanged = true;

            retVal.nData  = (short)(pMsgBuf[nCurrentByte++] << 8);
            retVal.nData += (short)pMsgBuf[nCurrentByte++];

            return retVal;
        }

        public static RC_U32 ExtractU32(ref byte[] pMsgBuf, int nCurrentByte)
        {
            RC_U32 retVal = default;
            retVal.bChanged = true;

            retVal.nData  = (uint)(pMsgBuf[nCurrentByte++] << 24);
            retVal.nData += (uint)(pMsgBuf[nCurrentByte++] << 16);
            retVal.nData += (uint)(pMsgBuf[nCurrentByte++] << 8);
            retVal.nData += (uint)(pMsgBuf[nCurrentByte++]);

            return retVal;
        }

        public static RC_S32 ExtractS32(ref byte[] pMsgBuf, int nCurrentByte)
        {
            RC_S32 retVal = default;
            retVal.bChanged = true;

            retVal.nData  = (int)(pMsgBuf[nCurrentByte++] << 24);
            retVal.nData += (int)(pMsgBuf[nCurrentByte++] << 16);
            retVal.nData += (int)(pMsgBuf[nCurrentByte++] << 8);
            retVal.nData += (int)(pMsgBuf[nCurrentByte++]);

            return retVal;
        }

        public static RC_U64 ExtractU64(ref byte[] pMsgBuf, int nCurrentByte)
        {
            RC_U64 retVal = default;
            retVal.bChanged = true;

            retVal.nData  = (ulong)(pMsgBuf[nCurrentByte++] << 56);
            retVal.nData += (ulong)(pMsgBuf[nCurrentByte++] << 48);
            retVal.nData += (ulong)(pMsgBuf[nCurrentByte++] << 40);
            retVal.nData += (ulong)(pMsgBuf[nCurrentByte++] << 32);
            retVal.nData += (ulong)(pMsgBuf[nCurrentByte++] << 24);
            retVal.nData += (ulong)(pMsgBuf[nCurrentByte++] << 16);
            retVal.nData += (ulong)(pMsgBuf[nCurrentByte++] << 8);
            retVal.nData += (ulong)(pMsgBuf[nCurrentByte++]);

            return retVal;
        }

        public static RC_S64 ExtractS64(ref byte[] pMsgBuf, int nCurrentByte)
        {
            RC_S64 retVal = default;
            retVal.bChanged = true;

            retVal.nData  = (long)(pMsgBuf[nCurrentByte++] << 56);
            retVal.nData += (long)(pMsgBuf[nCurrentByte++] << 48);
            retVal.nData += (long)(pMsgBuf[nCurrentByte++] << 40);
            retVal.nData += (long)(pMsgBuf[nCurrentByte++] << 32);
            retVal.nData += (long)(pMsgBuf[nCurrentByte++] << 24);
            retVal.nData += (long)(pMsgBuf[nCurrentByte++] << 16);
            retVal.nData += (long)(pMsgBuf[nCurrentByte++] << 8);
            retVal.nData += (long)(pMsgBuf[nCurrentByte++]);

            return retVal;
        }

        public static RC_U128 ExtractU128(ref byte[] pMsgBuf, int nCurrentByte)
        {
            RC_U128 retVal = default;
            retVal.bChanged = true;

            retVal.nDataHigh  = (ulong)(pMsgBuf[nCurrentByte++] << 56);
            retVal.nDataHigh += (ulong)(pMsgBuf[nCurrentByte++] << 48);
            retVal.nDataHigh += (ulong)(pMsgBuf[nCurrentByte++] << 40);
            retVal.nDataHigh += (ulong)(pMsgBuf[nCurrentByte++] << 32);
            retVal.nDataHigh += (ulong)(pMsgBuf[nCurrentByte++] << 24);
            retVal.nDataHigh += (ulong)(pMsgBuf[nCurrentByte++] << 16);
            retVal.nDataHigh += (ulong)(pMsgBuf[nCurrentByte++] << 8);
            retVal.nDataHigh += (ulong)(pMsgBuf[nCurrentByte++]);

            retVal.nDataLow  = (ulong)(pMsgBuf[nCurrentByte++] << 56);
            retVal.nDataLow += (ulong)(pMsgBuf[nCurrentByte++] << 48);
            retVal.nDataLow += (ulong)(pMsgBuf[nCurrentByte++] << 40);
            retVal.nDataLow += (ulong)(pMsgBuf[nCurrentByte++] << 32);
            retVal.nDataLow += (ulong)(pMsgBuf[nCurrentByte++] << 24);
            retVal.nDataLow += (ulong)(pMsgBuf[nCurrentByte++] << 16);
            retVal.nDataLow += (ulong)(pMsgBuf[nCurrentByte++] << 8);
            retVal.nDataLow += (ulong)(pMsgBuf[nCurrentByte++]);

            return retVal;
        }

        public static RC_Fixed ExtractFixed(ref byte[] pMsgBuf, int nCurrentByte)
        {
            RC_Fixed retVal = default;
            retVal.bChanged = true;

            int nTemp;

            nTemp  = (int)(pMsgBuf[nCurrentByte++] << 24);
            nTemp += (int)(pMsgBuf[nCurrentByte++] << 16);
            nTemp += (int)(pMsgBuf[nCurrentByte++] << 8);
            nTemp += (int)(pMsgBuf[nCurrentByte++]);

            retVal.nData = nTemp;
            retVal.fData = (nTemp) / 4096.0f;

            return retVal;
        }

        public static RC_S32 ExtractFixedAsS32(ref byte[] pMsgBuf, int nCurrentByte)
        {
            RC_S32 retVal = default;
            retVal.bChanged = true;

            int nTemp;

            nTemp  = (int)(pMsgBuf[nCurrentByte++] << 24);
            nTemp += (int)(pMsgBuf[nCurrentByte++] << 16);
            nTemp += (int)(pMsgBuf[nCurrentByte++] << 8);
            nTemp += (int)(pMsgBuf[nCurrentByte++]);

            retVal.nData = (int)((nTemp) / 4096.0f);

            return retVal;
        }

        public static RC_Float ExtractFloat(ref byte[] pMsgBuf, int nCurrentByte)
        {
            RC_Float retVal = default;
            retVal.bChanged = true;

            int nTemp;

            nTemp  = (int)(pMsgBuf[nCurrentByte++]);
            nTemp += (int)(pMsgBuf[nCurrentByte++] << 8);
            nTemp += (int)(pMsgBuf[nCurrentByte++] << 16);
            nTemp += (int)(pMsgBuf[nCurrentByte++] << 24);

            float fData = (float)(nTemp);

            retVal.fData = fData;

            return retVal;
        }
        
        public static RC_Double ExtractDouble(ref byte[] pMsgBuf, int nCurrentByte)
        {
            RC_Double retVal = default;
            retVal.bChanged = true;

            _DLB_TO_BYTES data = default;

            for(int i=7; i >= 0; i--)
            {
                data.uData[i] = (char)pMsgBuf[nCurrentByte++]; 
            }
            return retVal;
        }

        public static void ExtractString(ref byte[] pMsgBuf, int nCurrentByte, RC_String output, int nBytesToIncrement) 
        {
            output.bChanged = true;

            byte nStringLength = pMsgBuf[nCurrentByte++];

            byte[] pTemp = default;

            for (int i = 0; i < nStringLength; i++)
            {
                pTemp[i] = pMsgBuf[nCurrentByte++];
            }

            output.sData = (pTemp, nStringLength).ToString();

            nBytesToIncrement = nStringLength + 1; // Add one for null termination
        }

        ////////////////////////////////
        ///
        ////////////////////////////////

        public bool sendStartMsg(UdpClient sock, IPEndPoint client)
        {

            LogHelper.Log(LogTarget.DebugLog, "Tx: 0X" + cRC_UDP_IO.ByteArrayToString(startMsg.ToArray()));
            // Send the response message out
            try
            {
                sock.Send(startMsg.ToArray(), startMsgSize, client);
            }
            catch (SocketException e)
            {
                Debug.Log("Socket Exception" + e.ToString());
            }

            return true;


        }

        public void DisableAllPeriodicRequests()
        {
            foreach (var msg in m_dicPeriodicRequests)
            {
                DisablePeriodicRequest(msg.Value.nSubSystem, msg.Value.nSubIndex);
            }
        }

        public void EnableAllPeriodicRequests()
        {
            foreach (var msg in m_dicPeriodicRequests)
            {
                EnablePeriodicRequest(msg.Value.nSubSystem, msg.Value.nSubIndex, msg.Value.nDataMask);
            }
        }

        public void EnablePeriodicRequest(byte nSubsystem, byte nIndex, byte[] nDataMask)
        {
            List<byte> vecData = new List<byte>();

            vecData.Add(0x70);
            vecData.Add(0x01); // Frequency
            vecData.Add(0x71);

            for (int i = 0; i < 16; i++)
            {
                vecData.Add(nDataMask[i]);
            }

            SendData(nSubsystem, nIndex, vecData);
        }

        public void DisablePeriodicRequest(byte nSubsystem, byte nIndex)
        {
            List<byte> vecData = new List<byte>();

            vecData.Add(0x70);
            vecData.Add(0x00); // Frequency - Disabled
            vecData.Add(0x71);

            for (int i = 0; i < 16; i++)
            {
                vecData.Add(0x00); // Clear registrations
            }

            SendData(nSubsystem, nIndex, vecData);
        }

        public bool SendData(byte nSubsystem, byte nIdex, List<byte> vecData)
        {
            bool bRetValue;
            bRetValue = SendCharmData(nSubsystem, vecData);
            return bRetValue;
        }

        public bool SendCharmData(byte nSubSystem, List<byte> vecData)
        {
            List<byte> vecMsg = new List<byte>();
            bool recived = false;

            vecMsg.Add(0x10);
            vecMsg.Add(nSubSystem);
            vecMsg.Add(0x00);
            vecMsg.Add((byte)vecData.Count());

            var tempMsg = vecMsg.Concat(vecData);

            startMsg = tempMsg.ToList();

            startMsg.Add(CalcChecksum(startMsg.ToArray(), startMsg.Count()));

            startMsgSize = startMsg.Count();

            try
            {
                do
                {
                    if (!sendStartMsg(cRC_UDP_IO.m_udpClient, cRC_UDP_IO.client))
                    {
                        Debug.LogError("Could not initialise");
                    }
                    else if (sendStartMsg(cRC_UDP_IO.m_udpClient, cRC_UDP_IO.client))
                    {
                        recived = true;
                        Debug.Log("Charm initialised");
                    }
                } while (!recived);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }

            return true;
        }

        public byte CalcChecksum(byte[] pMsgBuf, int nMsgLength)
        {
            byte nCalculateChecksum = 0x00;

            for (var i = 0; i < nMsgLength; i++)
            {
                nCalculateChecksum += pMsgBuf[i];
            }

            return nCalculateChecksum;
        }

        // Root handler for messages
        public void handleCharmMessage(byte[] pMsgBuf, int nMsgLength)
        {
            //// The expected format is:
            ////
            //// Byte  0 - Header - 0x10
            //// Byte  1 - Sub-system ID
            //// Byte  2 - Status
            //// Byte  3 - Length
            //// Byte  4 - Message
            //// Byte  N - Checksum

            byte nSubSystem = pMsgBuf[(byte)RC_MsgDefinesCHARM.MsgByte.MSG_BYTE_SUB_SYS];
            byte nStatus = pMsgBuf[(byte)RC_MsgDefinesCHARM.MsgByte.MSG_BYTE_LENGTH_MSB];


            RC_MsgDefinesCHARM.MessageStatus stMessageStatus = default;


            if (false == ProcessStatusByte(nStatus, stMessageStatus))
                return;


            ProcessStatus(ref pMsgBuf, nMsgLength - (byte)RC_MsgDefinesCHARM.MsgByte.MSG_OVERHEAD_BYTES, nSubSystem, 0);

            // Comms are OK.
            //m_nCommsMissingCount = 0;
            //m_bCommsConnected = true;


        }

        public bool ProcessStatusByte(int nStatus, RC_MsgDefinesCHARM.MessageStatus stMessageStatus)
        {
            byte nMessageStatus = (byte)(nStatus & 0x3C);

            switch (nMessageStatus)
            {
                case RC_MsgDefinesCHARM.RC_STATUS_OK:
                    Debug.Log("Ok Status");
                    break;
                case RC_MsgDefinesCHARM.RC_STATUS_CSUMFAIL:
                    Debug.LogWarning("Bad Status - Csum Fail");
                    return false;
                case RC_MsgDefinesCHARM.RC_STATUS_UNK_ITEM:
                    Debug.LogWarning("Bad Status - Unknown Item");
                    return false;
                case RC_MsgDefinesCHARM.RC_STATUS_NOT_SUPP:
                    Debug.LogWarning("Bad Status - Data not supported");
                    return false;
                case RC_MsgDefinesCHARM.RC_STATUS_INV_STAT:
                    Debug.LogWarning("Bad Status - Data invalid");
                    return false;
                case RC_MsgDefinesCHARM.RC_STATUS_INV_ACCE:
                    Debug.LogWarning("Bad Status - Invalid data access");
                    return false;
                case RC_MsgDefinesCHARM.RC_STATUS_NO_BUFF:
                    Debug.LogWarning("Bad Status - Data type error");
                    return false;
                case RC_MsgDefinesCHARM.RC_STATUS_ERROR:
                    Debug.LogWarning("Bad Status - Unknown error");
                    return false;
                case RC_MsgDefinesCHARM.RC_STATUS_NOT_INIT:
                    Debug.LogWarning("Bad Status - Device not initialised ");
                    return false;
                case RC_MsgDefinesCHARM.RC_STATUS_BUSY:
                    Debug.LogWarning("Device busy");
                    return false;
                default:
                    Debug.LogWarning("Bad Status - Unknown error");
                    return false;
            }

            return true;
            ////Debug returns(I think)
            {
                //    int nTrigger = (nStatus & 0xC0);
                //    switch (nTrigger)
                //    {

                //        case RC_MsgDefinesCHARM.MessageStatus
                //             stMessageStatus = false;
                //            break;
                //        case RC_STATUS_PERIODIC:
                //            stMessageStatus.bPeriodicResponse = true;
                //            break;
                //        default:
                //            LG_WRN_FN(MM("Unknown Transmission Trigger - %d", nMessageStatus));
                //            return false;
                //    }


                //    stMessageStatus.bWarningSet = (nStatus & RC_STATUS_BIT_WARN) > 0;
                //    stMessageStatus.bErrorSet = (nStatus & RC_STATUS_BIT_ERR) > 0;


                //    return true;
            }
        }

        public void ProcessStatus(ref byte[] pMsgBuf, int nMsgLength, byte nSubsystem, byte nSubIndex)
        {
            int nCurrentByte = 0;

            Debug.Log("ProcessingTheStatus");

            int nBaseAddress = (int)((nSubsystem) << 16) + (int)((nSubIndex) << 8);

            while (nCurrentByte < (nMsgLength - 1))
            {
                int nAddress = nBaseAddress + pMsgBuf[nCurrentByte++];

                if (m_mapItemList.ContainsKey(nAddress))
                {
                    int nItems = m_mapItemList[nAddress].Count();
                    int nBytesToIncrement = 0;

                    for (int i = 0; i < nItems; i++)
                    {
                        ItemWithType stItem = m_mapItemList[nAddress].ElementAt(i);
                        //ItemWithType sItems = m_mapItemList[nAddress].Count();

                        switch (stItem.eType)
                        {
                            case RC_MsgDefinesCHARM.RC_DataTypes.DT_BYTE:
                                {
                                    RC_Byte ThisItem = (RC_Byte)(stItem.pData);
                                    ThisItem = cRC_UDP_IO.ExtractByte(ref pMsgBuf, nCurrentByte);
                                    nBytesToIncrement = 1;
                                    Debug.Log("Byte");
                                    break;
                                }
                            case RC_MsgDefinesCHARM.RC_DataTypes.DT_U16:
                                {
                                    RC_U16 ThisItem = (RC_U16)(stItem.pData);
                                    ThisItem = cRC_UDP_IO.ExtractU16(ref pMsgBuf, nCurrentByte);
                                    nBytesToIncrement = 2;
                                    Debug.Log("U16");
                                    break;
                                }
                            case RC_MsgDefinesCHARM.RC_DataTypes.DT_S16:
                                {
                                    RC_S16 ThisItem = (RC_S16)(stItem.pData);
                                    ThisItem = cRC_UDP_IO.ExtractS16(ref pMsgBuf, nCurrentByte);
                                    nBytesToIncrement = 2;
                                    Debug.Log("S16");
                                    break;
                                }
                            case RC_MsgDefinesCHARM.RC_DataTypes.DT_U32:
                                {
                                    RC_U32 ThisItem = (RC_U32)(stItem.pData);
                                    ThisItem = cRC_UDP_IO.ExtractU32(ref pMsgBuf, nCurrentByte);
                                    nBytesToIncrement = 4;
                                    Debug.Log("U32");
                                    break;
                                }
                            case RC_MsgDefinesCHARM.RC_DataTypes.DT_S32:
                                {
                                    RC_S32 ThisItem = (RC_S32)(stItem.pData);
                                    ThisItem = cRC_UDP_IO.ExtractS32(ref pMsgBuf, nCurrentByte);
                                    nBytesToIncrement = 4;
                                    Debug.Log("S32");
                                    break;
                                }
                            case RC_MsgDefinesCHARM.RC_DataTypes.DT_U64:
                                {
                                    RC_U64 ThisItem = (RC_U64)(stItem.pData);
                                    ThisItem = cRC_UDP_IO.ExtractU64(ref pMsgBuf, nCurrentByte);
                                    nBytesToIncrement = 8;
                                    Debug.Log("U64");
                                    break;
                                }
                            case RC_MsgDefinesCHARM.RC_DataTypes.DT_S64:
                                {
                                    RC_S64 ThisItem = (RC_S64)(stItem.pData);
                                    ThisItem = cRC_UDP_IO.ExtractS64(ref pMsgBuf, nCurrentByte);
                                    nBytesToIncrement = 8;
                                    Debug.Log("S64");
                                    break;
                                }
                            case RC_MsgDefinesCHARM.RC_DataTypes.DT_U128:
                                {
                                    RC_U128 ThisItem = (RC_U128)(stItem.pData);
                                    ThisItem = cRC_UDP_IO.ExtractU128(ref pMsgBuf, nCurrentByte);
                                    nBytesToIncrement = 16;
                                    Debug.Log("U128");
                                    break;
                                }
                            case RC_MsgDefinesCHARM.RC_DataTypes.DT_FIXED:
                                {
                                    RC_Fixed ThisItem = (RC_Fixed)(stItem.pData);
                                    ThisItem = cRC_UDP_IO.ExtractFixed(ref pMsgBuf, nCurrentByte);
                                    nBytesToIncrement = 4;
                                    Debug.Log("FIXED");
                                    break;
                                }
                            case RC_MsgDefinesCHARM.RC_DataTypes.DT_STRING:
                                {
                                    RC_String ThisItem = (RC_String)(stItem.pData);
                                    cRC_UDP_IO.ExtractString(ref pMsgBuf, nCurrentByte, (ThisItem), nBytesToIncrement);
                                    Debug.Log("String");
                                    break;
                                }
                            case RC_MsgDefinesCHARM.RC_DataTypes.DT_FLOAT:
                                {
                                    RC_Float ThisItem = (RC_Float)(stItem.pData);
                                    ThisItem = cRC_UDP_IO.ExtractFloat(ref pMsgBuf, nCurrentByte);
                                    nBytesToIncrement = 4;
                                    Debug.Log("Float");
                                    break;
                                }
                            case RC_MsgDefinesCHARM.RC_DataTypes.DT_DOUBLE:
                                {
                                    RC_Double ThisItem = (RC_Double)(stItem.pData);
                                    ThisItem = cRC_UDP_IO.ExtractDouble(ref pMsgBuf, nCurrentByte);
                                    nBytesToIncrement = 8;
                                    Debug.Log("Double");
                                    break;
                                }
                            default:
                                break;
                        }
                    }

                    nCurrentByte += nBytesToIncrement;
                }
                else
                {
                    Debug.LogError("Canot Find Key in Item Dictonary");
                }
            }

            //sigMsgReceived(nSubsystem);
        }

        public void CreateMapList()
        {
            if (m_pageList == null)
            {
                Debug.LogError("PageList Empty");
                return;
            }


            foreach (var page in m_pageList)
            {
                foreach (var item in page)
                {
                    if (item.nItem > 127)
                        continue;

                    int nAddress = (item.nSubsystem << 16)
                                 + (item.nSubsystem << 8)
                                 + (item.nItem);

                    if (!m_mapItemList.ContainsKey(nAddress))
                    {
                        // create new list of itemwithtype
                        List<ItemWithType> newList = new List<ItemWithType>();
                        m_mapItemList.Add(nAddress, newList);
                        Debug.Log("Item Map Created");
                    }

                    ItemWithType stThisItem;
                    stThisItem.eType = (RC_MsgDefinesCHARM.RC_DataTypes)(item.nType);
                    stThisItem.pData = (RC_Data)(item.stData);

                    m_mapItemList[nAddress].Add(stThisItem);

                    // check if needs to be added to periodic requests
                    if (item.nAccess != RC_MsgDefns.AccessType.DIA_WRITE_ONLY)
                    {
                        // Add the item to the periodic message request
                        int nAdress2 = (item.nSubsystem << 8) + item.nSubsystem;

                        // check if periodic request for this subsystem
                        if (!m_dicPeriodicRequests.ContainsKey(nAdress2))
                        {
                            rCHARM_MsgHandler.PeriodicMessageData newPeriodicMsg = new rCHARM_MsgHandler.PeriodicMessageData();

                            byte[] temp = new byte[16];

                            for (int i = 0; i < 16; i++)
                            {
                                // clear out 128 bits
                                temp[i] = 0;
                                //newPeriodicMsg.nDataMask[i] = 0;
                            }

                            newPeriodicMsg.nDataMask = temp;
                            newPeriodicMsg.nSubSystem = item.nSubsystem;
                            newPeriodicMsg.nSubIndex = item.bSubIndex;

                            m_dicPeriodicRequests.Add(nAdress2, newPeriodicMsg);
                        }

                        // derive byte offset and bit position in 128 bit array
                        int nByte = item.nItem / 8;
                        int nBit = item.nItem % 8;

                        // now OR bit into appropriate slot in 128 bit array
                        m_dicPeriodicRequests[nAdress2].nDataMask[nByte] |= (byte)((1 << nBit) & 0xff);
                    }
                }
            }

            // OK - now transmit Periodic Message Request to target
            EnableAllPeriodicRequests();
        }


    }

    public struct _DLB_TO_BYTES
    {
        public double dData;
        public char[] uData;
    }

}