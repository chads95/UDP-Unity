using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


namespace Udp
{
    public struct ItemWithType
    {
        public RC_MsgDefinesCHARM.RC_DataTypes eType;

        public RC_Data pData;
    }
    public class rCHARM_MsgHandler : MonoBehaviour
    {
        /// Tx and Rx Statistics
        private int m_nRxMsgTotal;      // Total messages processed, good and bad
        private int m_nRxCsumFails;     // Number of msgs which failed the checksum check
        private int m_nRxCompFails;     // Number of msgs containing invalid component operations

        private int m_nRxCompWrites;    // Number of components that have been written to
        private int m_nRxCompReads;     // Number of components that have been read

        private int m_nTxResp;          // Number of response messages transmitted
        private int m_nTxPeri;          // Number of periodic messages transmitted
        private int m_nTxWarn;          // Number of periodic messages transmitted
        [HideInInspector]
        public byte m_cSubSysId;
        [HideInInspector]
        public byte m_cSubSysIndex;
        public struct PERIODIC_DEFN
        {
            public byte[] nMsgDefn;
            public cDI_Byte diMsgFreq;
            public int nMsgSkip;

            public PERIODIC_DEFN(byte nData = 0, byte nIdentifier = 1, int nSkip = 0)
            {
                nMsgDefn = new byte[RC_MsgDefinesCHARM.MAX_MSG_DATA_DEFNS + 1];
                diMsgFreq = new cDI_Byte(nData, nIdentifier);
                nMsgSkip = nSkip;
            }
        }

        public struct PeriodicMessageData
        {
            public byte[] nDataMask;
            public byte nSubSystem;
            public byte nSubIndex;
        }


        public rCHARM_MsgHandler() { }

        public bool handleRxMessage(UdpClient sock, byte[] msg_buf, IPEndPoint client)
        {
            int nMsgLength = msg_buf.Length;

            // Setup the variables for the response
            byte[] cReplyMsg = new byte[256];
            int nReplyLength = 0;
            byte cStatusByte = 0;    // Set to zero to indicate a response message

            byte uCalcChecksum = RC_MsgHelperCHARM.calcRC_Checksum(msg_buf, nMsgLength - 1);

            // Calculate checksum without inclusion of the last byte (i.e. the checksum)
            // Compare against the last byte ... index of length - 1
            if (uCalcChecksum != msg_buf[(nMsgLength - 1)])
            {
                // Checksum fail
                m_nRxCsumFails++;
                /// Set the status byte appropriately
                cStatusByte |= RC_MsgDefinesCHARM.RC_STATUS_CSUMFAIL;

                Debug.Log("Checksum fail, expected 0x{0:X}, received 0x{1:X}" + uCalcChecksum + msg_buf[(nMsgLength - 1)]);
            }
            else
            {
                // Now run through the commands held within the message and
                // pass them off to their respective handlers
                int nCmdIdByte = (int)RC_MsgDefinesCHARM.MsgByte.MSG_BYTE_FIRST_CMD;   // Set the index of the first cmd identifier
                RC_MsgDefns.SuccessType eMsgGood;   // Pick up a success return value
                int nValidBytes = nMsgLength;

                do
                {
                    // Act on commands
                    if (msg_buf[nCmdIdByte] < 0x70)
                    {
                        Debug.Log("MsgCmdByte < 0x70");
                        // Call the action function provided by a derived class
                        // Command byte incremented on by function call
                        eMsgGood = actionCfgWrite(msg_buf, ref nCmdIdByte, nValidBytes);

                        if (RC_MsgDefns.SuccessType.DIS_OK == eMsgGood)
                        {
                            m_nRxCompWrites++;  // Record received commands
                        }
                    }
                    else
                    {
                        // Call the action function provided by a derived class
                        // nMsgCapacity is one byte less than the buffer to allow for the checksum byte
                        eMsgGood = actionCfgRead(msg_buf[nCmdIdByte], ref cReplyMsg, ref nReplyLength, 255);
                        if (RC_MsgDefns.SuccessType.DIS_OK == eMsgGood)
                        {
                            nCmdIdByte += 1;  // Increment by the command specific length
                            m_nRxCompReads++;  // Record received reads
                        }
                    }
                } while ((nCmdIdByte < nValidBytes - 1));  // Don't look at csum byte

                if (RC_MsgDefns.SuccessType.DIS_OK != eMsgGood)
                {
                    m_nRxCompFails++;    // Record the message interpretation failure

                    /// Set the status byte appropriately
                    switch (eMsgGood)
                    {
                        default:
                        case RC_MsgDefns.SuccessType.DIS_UNKNOWN_ITEM:
                            cStatusByte |= RC_MsgDefinesCHARM.RC_STATUS_UNK_ITEM;
                            break;
                    }
                }

                // Complete the formation of the response
                RC_MsgHelperCHARM.completeRC_RespCHARM(cStatusByte, ref cReplyMsg, ref nReplyLength);

                // logTxMsg( cReplyMsg, nReplyLength );

                // Send the response message out
                try
                {
                    sock.Send(cReplyMsg, nReplyLength, client);
                }
                catch (SocketException e)
                {
                    Debug.Log("Socket Exception" + e.ToString());
                }
            }
            return true;
        }

        public static void sendUnknownResp()
        {
            // Setup the variables for the response
            byte[] cReplyMsg = new byte[256];
            int nReplyLength = 0;

            RC_MsgHelperCHARM.formRC_RespHeaderCHARM(0, ref cReplyMsg, ref nReplyLength);

            // Complete the formation of the response
            RC_MsgHelperCHARM.completeRC_RespCHARM(RC_MsgDefinesCHARM.RC_STATUS_UNK_ITEM, ref cReplyMsg, ref nReplyLength);

            // TODO Now send out the reply:
        }

        public void Execute(UdpClient sock, IPEndPoint client, Dictionary<byte, PERIODIC_DEFN> dictPeriodic, int waitTime)
        {
            BackgroundPeriodicMsg periodicMsg = new BackgroundPeriodicMsg(this, sock, client, dictPeriodic, waitTime);
            Thread periodicMsgThread =
                new Thread(new ThreadStart(periodicMsg.RunLoop));
            periodicMsgThread.IsBackground = true;
            periodicMsgThread.Start();
        }

        public class BackgroundPeriodicMsg
        {
            rCHARM_MsgHandler MsgHandler;
            UdpClient sock;
            IPEndPoint client;
            public Dictionary<byte, PERIODIC_DEFN> dictPeriodic;
            public int waitTime;
            private Mutex mutex = new Mutex();

            public BackgroundPeriodicMsg(rCHARM_MsgHandler MsgHandler, UdpClient sock, IPEndPoint client, Dictionary<byte, PERIODIC_DEFN> dictPeriodic, int waitTime)
            {
                this.MsgHandler = MsgHandler;
                this.sock = sock;
                this.client = client;
                this.dictPeriodic = dictPeriodic;
                this.waitTime = waitTime;
            }

            public void RunLoop()
            {
                Debug.Log("periodic Thread Created");
                while (cRC_UDP_IO.ThreadCheck == true)
                {
                    mutex.WaitOne();
                    MsgHandler.sendAllPeriodicMessages(sock, client, dictPeriodic);
                    Thread.Sleep(20);
                    mutex.ReleaseMutex();
                }
            }
        }

        private RC_MsgDefns.SuccessType actionCfgWrite(byte[] pMsg, ref int pByteId, int nRxMsgBytes)
        {
            //// Default to can't find a message match
            //RC_MsgDefns.SuccessType eRetVal = RC_MsgDefns.SuccessType.DIS_UNKNOWN_ITEM;

            //switch (pMsg[pByteId])
            //{
            //    case RC_MsgDefinesCHARM.DID_PeriodicMsgFreq:
            //        {
            //            Debug.Log("Received DID_PeriodicMsgFreq, length {0}" + nRxMsgBytes);
            //            int nUsedBytes = 0;
            //            // Call the item reader
            //            // Bytes available is the capacity - current length - the identifier we have just written
            //            eRetVal = actionMsgFreqWrite(pMsg, nRxMsgBytes, pByteId + 1, ref nUsedBytes);

            //            // Increment the used byte count
            //            pByteId += (nUsedBytes + 1);
            //        }
            //        break;
            //    case RC_MsgDefinesCHARM.DID_PeriodicMsgDefn:
            //        {
            //            Debug.Log("Received DID_PeriodicMsgDefn, length {0}" + nRxMsgBytes);
            //            int nUsedBytes = 0;
            //            /// Call the item reader
            //            /// Bytes available is the capacity - current length - the identifier we have just written
            //            eRetVal = actionMsgDefnWrite(pMsg, nRxMsgBytes, pByteId + 1, ref nUsedBytes);

            //            /// Increment the used byte count
            //            pByteId += (nUsedBytes + 1);
            //        }
            //        break;
            //    default:
            //        {
            //            Debug.Log("Default");

            //            foreach (var iter in vecDataItems)
            //            {
            //                if (pMsg[pByteId] == iter.GetIntentifier())
            //                {

            //                    /// \note that this check is replicated in the generic handler in cDataItem
            //                    if (RC_MsgDefns.AccessType.DIA_READ_ONLY == iter.GetAccess())
            //                    {
            //                        /// No data writer available
            //                        Debug.Log("No Data writer");
            //                        return RC_MsgDefns.SuccessType.DIS_INVALID_ACCESS;
            //                    }

            //                    int nUsedBytes = 0;
            //                    /// Call the item reader
            //                    /// Bytes available is the capacity - current length - the identifier we have just written
            //                    Debug.Log("Bytes avalible");
            //                    eRetVal = iter.write(pMsg, nRxMsgBytes, pByteId + 1, ref nUsedBytes, false);

            //                    if (RC_MsgDefns.SuccessType.DIS_OK == eRetVal)
            //                    {
            //                        /// Increment the used byte count
            //                        Debug.Log("Increment Used byte count");
            //                        pByteId += (nUsedBytes + 1);
            //                    }

            //                    return eRetVal;
            //                }
            //            }
            //        }
            //        break;
            //}  // End of switch

            //return eRetVal;
            throw new NotImplementedException();
        }

        private RC_MsgDefns.SuccessType actionCfgRead(byte cReadId, ref byte[] pReplyMsg, ref int pMsgLength, int nMsgCapacity)
        {
            //// Clear the MSbit (read bit)
            //cReadId &= 0x7f;

            //// Default return to Can't find a message match
            //RC_MsgDefns.SuccessType eRetVal = RC_MsgDefns.SuccessType.DIS_UNKNOWN_ITEM;

            //switch (cReadId)
            //{
            //    case RC_MsgDefinesCHARM.DID_PeriodicMsgFreq:
            //        {
            //            // Fill in the data identifier
            //            pReplyMsg[0] = cReadId;

            //            int nUsedBytes = 0;
            //            // Call the item reader
            //            // Bytes available is the capacity - current length - the identifier we have just written
            //            eRetVal = actionMsgFreqRead(pReplyMsg, nMsgCapacity, pMsgLength - 1, ref nUsedBytes);

            //            if (RC_MsgDefns.SuccessType.DIS_OK == eRetVal)
            //                // Increment the used byte count
            //                pMsgLength += (nUsedBytes + 1);
            //        }
            //        break;
            //    case RC_MsgDefinesCHARM.DID_PeriodicMsgDefn:
            //        {
            //            /// Fill in the data identifier
            //            pReplyMsg[0] = cReadId;

            //            int nUsedBytes = 0;
            //            /// Call the item reader
            //            /// Bytes available is the capacity - current length - the identifier we have just written
            //            eRetVal = actionMsgDefnRead(pReplyMsg, (nMsgCapacity - pMsgLength - 1), ref nUsedBytes);

            //            if (RC_MsgDefns.SuccessType.DIS_OK == eRetVal)
            //                /// Increment the used byte count
            //                pMsgLength += (nUsedBytes + 1);
            //        }
            //        break;
            //    default:
            //        {
            //            foreach (var iter in vecDataItems)
            //            {
            //                if (cReadId == iter.GetIntentifier())
            //                {
            //                    /// Fill in the data identifier
            //                    pReplyMsg[pMsgLength++] = cReadId;

            //                    int nUsedBytes = 0;
            //                    /// Call the item reader
            //                    /// Bytes available is the capacity - current length - the identifier we have just written
            //                    eRetVal = iter.read(ref pReplyMsg, nMsgCapacity, pMsgLength, ref nUsedBytes);

            //                    if (RC_MsgDefns.SuccessType.DIS_OK == eRetVal)
            //                        /// Increment the used byte count
            //                        pMsgLength += (nUsedBytes);

            //                    return eRetVal;
            //                }
            //            }
            //            break;
            //        }
            //}
            //return eRetVal;
            throw new NotImplementedException();
        }

        private RC_MsgDefns.SuccessType actionMsgDefnWrite(byte[] pDataStart, int nRxMsgBytes, int nByteId, ref int nBytesUsed)
        {
            //if ((nRxMsgBytes - nByteId) < 16)
            //{
            //    /// Not enough data
            //    return RC_MsgDefns.SuccessType.DIS_UNKNOWN_ITEM;
            //}

            //PERIODIC_DEFN pPeriodic = new PERIODIC_DEFN();

            //if (!getPeriodicDefn(0x00, ref pPeriodic))
            //    return RC_MsgDefns.SuccessType.DIS_UNKNOWN_ITEM;

            //byte[] pDataOut = pPeriodic.nMsgDefn;

            ///// Zero the output data defintion count
            //pDataOut[0] = 0;

            ///// TODO: Run through all the configuration handlers
            //int nIndex = 0;

            //foreach (var iter in vecDataItems)
            //{
            //    if (pDataOut[0] < (RC_MsgDefinesCHARM.MAX_MSG_DATA_DEFNS - 1))
            //    {

            //        if (RC_MsgDefns.AccessType.DIA_WRITE_ONLY != iter.GetAccess())
            //        {
            //            int nByte = iter.GetIntentifier() / 8;
            //            int nBit = iter.GetIntentifier() % 8;
            //            /// See if the appropriate bit is set
            //            var pData = pDataStart[nByteId + nByte] & (0x01 << nBit);


            //            if (pData >= 1)
            //            {
            //                /// Increment the output count
            //                pDataOut[0] += 1;
            //                /// Set the index of the data defn in the message definition list
            //                pDataOut[(pDataOut[0])] = (byte)nIndex;
            //            }
            //        }
            //        nIndex++;   ///< Increment the cfgHandler pointer
            //    }
            //}

            //IPEndPoint client = cRC_UDP_IO.client;
            //UdpClient sock = cRC_UDP_IO.m_udpClient;
            ///// TODO Force out a periodic message
            //if (0 != pDataOut[0])
            //{
            //    Execute(sock, client, m_dictPeriodic, 20);
            //}

            ///// Update the number of message bytes used
            //nBytesUsed = 16;

            //return RC_MsgDefns.SuccessType.DIS_OK;
            throw new NotImplementedException();
        }

        private RC_MsgDefns.SuccessType actionMsgDefnRead(byte[] pDataStart, int nBytesAvail, ref int nBytesUsed)
        {
            //if (nBytesAvail < 16)
            //{
            //    /// Not enough data
            //    return RC_MsgDefns.SuccessType.DIS_UNKNOWN_ITEM;
            //}

            //PERIODIC_DEFN pPeriodic = new PERIODIC_DEFN();

            //if (!getPeriodicDefn(0x00, ref pPeriodic))
            //    return RC_MsgDefns.SuccessType.DIS_UNKNOWN_ITEM;

            ///// Clear the bytes down
            //for (int i = 0; i < 16; i++)
            //{
            //    pDataStart[i] = 0;
            //}

            //for (int i = 0; i < pPeriodic.nMsgDefn[0]; i++)
            //{
            //    cDataItem pItem = vecDataItems.ElementAt(pPeriodic.nMsgDefn[i + 1]);

            //    /// Set the appropriate bit in the block
            //    RC_MsgHelpers.setMesgDefnBit(ref pDataStart, pItem.GetIntentifier());
            //}

            ///// Return the number of message bytes used
            //return RC_MsgDefns.SuccessType.DIS_OK;
            throw new NotImplementedException();
        }

        private RC_MsgDefns.SuccessType actionMsgFreqWrite(byte[] pDataStart, int nRxBytes, int nByteId, ref int nBytesUsed)
        {
            //PERIODIC_DEFN pPeriodic = new PERIODIC_DEFN();

            //if (!getPeriodicDefn(0x00, ref pPeriodic))
            //    return RC_MsgDefns.SuccessType.DIS_NOT_INIT;

            //return pPeriodic.diMsgFreq.write(pDataStart, nRxBytes, nByteId, ref nBytesUsed, false);
            throw new NotImplementedException();
        }

        private RC_MsgDefns.SuccessType actionMsgFreqRead(byte[] pDataStart, int nMsgCapacity, int nMsgLength, ref int nBytesUsed)
        {
            //PERIODIC_DEFN pPeriodic = new PERIODIC_DEFN();

            //if (!getPeriodicDefn(0x00, ref pPeriodic))
            //    return RC_MsgDefns.SuccessType.DIS_NOT_INIT;

            //return pPeriodic.diMsgFreq.read(ref pDataStart, nMsgCapacity, nMsgLength, ref nBytesUsed);
            throw new NotImplementedException();
        }

        private void sendAllPeriodicMessages(UdpClient sock, IPEndPoint client, Dictionary<byte, PERIODIC_DEFN> dictPeriodic)
        {
            //foreach (KeyValuePair<byte, PERIODIC_DEFN> entry in m_dictPeriodic)
            //{
            //    if (sendPeriodicMessage(sock, client))
            //    {
            //        // We have transmitted a periodic message
            //        m_nTxPeri++;
            //    }
            //    //else
            //    // Have failed to send a periodic message, probably buffer full 
            //    // could this connection be broken?
            //}
            throw new NotImplementedException();
        }

        private const int MAX_PERIODIC_LENGTH = 1024;
        private const int CHECKSUM_LENGTH = 1;

        private bool sendPeriodicMessage(UdpClient sock, IPEndPoint client)
        {
            ///// Setup a local buffer for the message
            //byte[] cStatusMsg = new byte[MAX_PERIODIC_LENGTH];
            //byte cStatusByte;
            //int nMsgLength = 0;

            ///// Form the message header
            //RC_MsgHelperCHARM.formRC_RespHeaderCHARM(m_cSubSysId, ref cStatusMsg, ref nMsgLength);

            ///// Call the sub-system specific function to fill in the data
            //if (!fillPeriodicMessage(cStatusMsg, (MAX_PERIODIC_LENGTH - nMsgLength - CHECKSUM_LENGTH), ref nMsgLength))
            //{
            //    Debug.Log("Execute formPeriodicMessage returned false");
            //    return false;
            //}

            //cStatusByte = RC_MsgDefinesCHARM.RC_STATUS_PERIODIC;

            //// Complete the message
            //RC_MsgHelperCHARM.completeRC_RespCHARM(cStatusByte, ref cStatusMsg, ref nMsgLength);

            //LogHelper.Log(LogTarget.File, "Tx: 0X" + cRC_UDP_IO.ByteArrayToString(cStatusMsg));
            //LogHelper.Log(LogTarget.DebugLog, "Tx: 0X" + cRC_UDP_IO.ByteArrayToString(cStatusMsg));

            //// Transmit the msg
            //sock.Send(cStatusMsg, nMsgLength, client);

            //return true;
            throw new NotImplementedException();

        }

        private bool fillPeriodicMessage(byte[] pStatusMsg, int nBytesAvail, ref int nBytesUsed)
        {
            //PERIODIC_DEFN pPeriodic = new PERIODIC_DEFN();

            //if (!getPeriodicDefn(0x00, ref pPeriodic))
            //    return false;

            ///// If there is no periodic message contents, then return false
            //if (0 == pPeriodic.nMsgDefn[0])
            //    return false;

            //int nFreq = pPeriodic.diMsgFreq.value();
            //if ((0 == nFreq) || (++pPeriodic.nMsgSkip < nFreq))
            //    /// Frequency request not met
            //    return false;
            //else
            //    /// Reset the skipped frame counter
            //    pPeriodic.nMsgSkip = 0;

            //int nDefnBytes = 0;
            //RC_MsgDefns.SuccessType eRetVal = RC_MsgDefns.SuccessType.DIS_OK;

            //for (int i = 0; i < pPeriodic.nMsgDefn[0]; i++)
            //{
            //    /// Pick up the required DataDefn handler
            //    cDataItem pItem = vecDataItems.ElementAt(pPeriodic.nMsgDefn[i + 1]);

            //    /// Fill in the data definition id
            //    pStatusMsg[nBytesUsed] = pItem.GetIntentifier();

            //    /// Fill in the associated data
            //    eRetVal = pItem.read(ref pStatusMsg, nBytesAvail, nBytesUsed + 1, ref nDefnBytes);

            //    if (RC_MsgDefns.SuccessType.DIS_OK == eRetVal)
            //    {
            //        /// Move the byte index on
            //        nBytesUsed += (nDefnBytes + 1);
            //    }
            //    else if (RC_MsgDefns.SuccessType.DIS_UNKNOWN_ITEM == eRetVal)
            //    {
            //        /// A real problem
            //        Debug.Log("Very big Oh Dear!");
            //        return false;
            //    }
            //}

            //return true;
            throw new NotImplementedException();
        }

        private bool getPeriodicDefn(byte nConnection, ref PERIODIC_DEFN pOut)
        {
            //foreach (KeyValuePair<byte, PERIODIC_DEFN> entry in m_dictPeriodic)
            //{
            //    if (entry.Key == nConnection)
            //    {
            //        // Found it
            //        pOut = entry.Value;
            //        return true;
            //    }
            //}

            //// Not found - add it
            //return addPeriodicDefn(nConnection, ref pOut);

            throw new NotImplementedException();
        }

        /// Translate from endPoint to connection
        private bool addPeriodicDefn(byte nConnection, ref PERIODIC_DEFN pOut)
        {
            //// Add message definition
            //pOut = new PERIODIC_DEFN(0, 1);

            //m_dictPeriodic.Add(nConnection, pOut);

            //return true;
            throw new NotImplementedException();
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        // Starter Functions
        /////////////////////////////////////////////////////////////////////////////////////////


    }
}
