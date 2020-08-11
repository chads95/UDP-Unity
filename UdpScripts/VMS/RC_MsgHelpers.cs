using System;
using System.Collections.Generic;
using System.Text;

namespace Udp
{
    public static class RC_MsgHelpers
    {
        public static byte calcRC_Checksum( byte[] pMsg, int nLength )
        {
            byte cCsum = 0;

            for ( int i = 0; i < nLength; i++ )
            {
                cCsum += pMsg[i];
            }

            return cCsum;
        }

        // Helper function to form the initial parts of the command message
        public static void formRC_CmdHeader( byte cSubSysId, byte nIndex, ref byte[] pMsg, ref int pLength )
        {
            // Add in the header
            pMsg[pLength++] = RC_MsgDefnsVMS.RC_MSG_HEADER_CHAR;
            // Add in the subsytem ID
            pMsg[pLength++] = cSubSysId;
            // Add in the subsytem index
            pMsg[pLength++] = nIndex;
            // Zero the length bytes for the moment
            pMsg[pLength++] = 0x00;
            pMsg[pLength++] = 0x00;
        }

        // Helper function to form the initial parts of the command message
        public static void formRC_RespHeader( byte cSubSysId, byte nIndex, ref byte[] pMsg, ref int pLength )
        {
            // Add in the header
            pMsg[pLength++] = RC_MsgDefnsVMS.RC_MSG_HEADER_CHAR;
            // Add in the subsytem ID
            pMsg[pLength++] = cSubSysId;
            // Add in the subsytem index
            pMsg[pLength++] = nIndex;
            // Zero the status byte for the moment
            pMsg[pLength++] = 0x00;
            // Zero the length bytes for the moment
            pMsg[pLength++] = 0x00;
            pMsg[pLength++] = 0x00;
        }

        // Helper function to form the initial parts of the command message
        public static void formRC_RespHeader( byte cConnectionId, byte cSubSysId, byte nIndex, ref byte[] pMsg, ref int pLength )
        {
            // Add in the header
            pMsg[pLength++] = cConnectionId;
            // Add in the header
            pMsg[pLength++] = RC_MsgDefnsVMS.RC_MSG_HEADER_CHAR;
            // Add in the subsytem ID
            pMsg[pLength++] = cSubSysId;
            // Add in the subsytem index
            pMsg[pLength++] = nIndex;
            // Zero the status byte for the moment
            pMsg[pLength++] = 0x00;
            // Zero the length bytes for the moment
            pMsg[pLength++] = 0x00;
            pMsg[pLength++] = 0x00;
        }

        // Helper function to finalise the command message
        public static void completeRC_Cmd( byte[] pMsg, ref int pLength )
        {
            // Set the length byte (note we are yet to add the checksum)
            int nLength = pLength + 1;
            pMsg[3] = (byte)( 0xFF & ( nLength >> 8 ) );
            pMsg[4] = (byte)( 0xFF & nLength );

            // Calculate the checksum
            pMsg[pLength] = calcRC_Checksum( pMsg, pLength );
            pLength += 1;
        }

        // Helper function to finalise the command message
        public static void completeRC_Resp( byte cStatus, ref byte[] pMsg, ref int pLength )
        {
            // Set the status byte
            pMsg[3] = cStatus;

            // Set the length byte (note we are yet to add the checksum)
            int nLength = pLength + 1;
            pMsg[4] = (byte)( 0xFF & ( nLength >> 8 ) );
            pMsg[5] = (byte)( 0xFF & nLength );

            // Calculate the checksum
            pMsg[pLength] = calcRC_Checksum( pMsg, nLength );
            pLength += 1;
        }

        // Helper function to finalise the command message (those with connections)
        public static void completeRC_Resp( byte cConnectionId, byte cStatus, ref byte[] pMsg, ref int pLength )
        {
            // Set the status byte
            pMsg[4] = cStatus;

            // Set the length byte (note we are yet to add the checksum)
            int nLength = pLength + 1;
            pMsg[5] = (byte)( 0xFF & ( nLength >> 8 ) );
            pMsg[6] = (byte)( 0xFF & nLength );

            // Calculate the checksum
            nLength = pLength - 1;
            pMsg[pLength] = calcRC_Checksum( pMsg, nLength );
            pLength += 1;
        }

        // Helper function to finalise the command message (those with connections)
        public static void setMesgDefnBit( ref byte[] pMsgDefn, byte cDataDefn )
        {
            if ( cDataDefn > 127 )
                return;

            int nByte = cDataDefn / 8;
            int nBit = cDataDefn % 8;

            pMsgDefn[nByte] |= (byte)( ( 1 << nBit ) & 0xff );
        }
    }
}
