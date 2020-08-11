using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Udp
{
    public abstract class cDataItem
    {
        private byte m_nIdentifier;
        private RC_MsgDefns.AccessType m_eAccessType;

        public cDataItem( byte nIndentifier, RC_MsgDefns.AccessType eAccessType = RC_MsgDefns.AccessType.DIA_READ_WRITE )
        {
            m_nIdentifier = nIndentifier;
            m_eAccessType = eAccessType;

        }

        public byte GetIntentifier()
        {
            return m_nIdentifier;
        }

        public void SetIdentifier( byte nIdentifier )
        {
            m_nIdentifier = nIdentifier;
        }

        public RC_MsgDefns.AccessType GetAccess()
        {
            return m_eAccessType;
        }

        public void SetAccess( RC_MsgDefns.AccessType eAccessType )
        {
            m_eAccessType = eAccessType;
        }


        public RC_MsgDefns.SuccessType write( byte[] pDataStart, int nRxBytes, int nByteId, ref int nBytesUsed, bool bIgnore )
        {
            if ( m_eAccessType == RC_MsgDefns.AccessType.DIA_READ_ONLY )
            {
                Debug.Log("Invalid Access");
                return RC_MsgDefns.SuccessType.DIS_INVALID_ACCESS;
            }
            else
            {
                Debug.Log("write data");
                return writeData( pDataStart, nRxBytes, nByteId, ref nBytesUsed, bIgnore );
            }
        }

        public RC_MsgDefns.SuccessType read( ref byte[] pDataStart, int nMsgCapacity, int nMsgLength, ref int nBytesUsed )
        {
            if ( m_eAccessType == RC_MsgDefns.AccessType.DIA_WRITE_ONLY )
            {
                return RC_MsgDefns.SuccessType.DIS_INVALID_ACCESS;
            }
            else
            {
                return readData( ref pDataStart, nMsgCapacity, nMsgLength, ref nBytesUsed );
            }
        }

        abstract public RC_MsgDefns.SuccessType writeData( byte[] pDataStart, int nRxBytes, int nByteId, ref int nBytesUsed, bool bIgnore );

        abstract public RC_MsgDefns.SuccessType readData( ref byte[] pDataStart, int nMsgCapacity, int nMsgLength, ref int nBytesUsed );
    }
}
