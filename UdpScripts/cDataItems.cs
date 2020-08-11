using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Udp
{
   public class cDI_Byte : cDataItem
   {
      protected int m_nData;

      public Func<byte, RC_MsgDefns.SuccessType> m_fnWrite;
      public delegate RC_MsgDefns.SuccessType ReadFunc( ref byte nData );
      public ReadFunc m_fnRead;

      public cDI_Byte( byte nData, byte nIndentifier, RC_MsgDefns.AccessType eAccessType = RC_MsgDefns.AccessType.DIA_READ_WRITE )
          : base( nIndentifier, eAccessType )
      {
         m_nData = nData;
      }

      public void update( byte nData )
      {
         m_nData = nData;
      }

      public byte value()
      {
         return (byte)m_nData;
      }

      public override RC_MsgDefns.SuccessType readData( ref byte[] pDataStart, int nMsgCapacity, int nMsgLength, ref int nBytesUsed )
      {
         if ( ( nMsgCapacity - nMsgLength ) < 1 )
            /// Not enough data
            return RC_MsgDefns.SuccessType.DIS_INSUFFICIENT_BUFFER;

         RC_MsgDefns.SuccessType eRetVal = RC_MsgDefns.SuccessType.DIS_OK;


         /// Otherwise write the config data into the message
         pDataStart[nMsgLength] = (byte)m_nData;

         /// Return the number of message bytes used
         nBytesUsed = 1;

         return eRetVal;
      }

      public override RC_MsgDefns.SuccessType writeData( byte[] pDataStart, int nRxBytes, int nByteId, ref int nBytesUsed, bool bIgnore )
      {
            if ((nRxBytes - nByteId) < 1)
            {
                /// Not enough data
                Debug.Log("Not Enough Data");
                return RC_MsgDefns.SuccessType.DIS_INSUFFICIENT_BUFFER;
            }

         /// Return the number of message bytes used
         nBytesUsed = 1;

            if (bIgnore)
            {
                Debug.Log("bIgnore");
                return RC_MsgDefns.SuccessType.DIS_OK;
            }
            else
            {
                // Convert the incoming data
                Debug.Log("Convert the incomming data");
                int nTemp = (int)pDataStart[nByteId];

                RC_MsgDefns.SuccessType eRetVal = RC_MsgDefns.SuccessType.DIS_OK;

                /// Pass on the data
                if (m_fnWrite != null)
                {
                    Debug.Log("m_fnWrite is not null");
                    eRetVal = m_fnWrite((byte)nTemp);

                    if (eRetVal == RC_MsgDefns.SuccessType.DIS_OK)
                    {
                        // Update the config with the supplied data
                        Debug.Log("Update the config with the data");
                        m_nData = nTemp;
                    }
                    else
                    {
                        Debug.Log(eRetVal);
                    }

                    return eRetVal;
                }
                else
                {
                    // Update the config with the supplied data
                    Debug.Log("m_fnWrite is null");
                    m_nData = nTemp;

                    return eRetVal;
                }
            }
      }
   }

   public class cDI_Word : cDataItem
   {
      protected ushort m_nData;

      public Func<ushort, RC_MsgDefns.SuccessType> m_fnWrite;
      public delegate RC_MsgDefns.SuccessType ReadFunc( ref ushort fData );
      public ReadFunc m_fnRead;

      public cDI_Word( ushort nData, byte nIndentifier, RC_MsgDefns.AccessType eAccessType = RC_MsgDefns.AccessType.DIA_READ_WRITE )
          : base( nIndentifier, eAccessType )
      {
         m_nData = nData;
      }

      public override RC_MsgDefns.SuccessType readData( ref byte[] pDataStart, int nMsgCapacity, int nMsgLength, ref int nBytesUsed )
      {
         if ( ( nMsgCapacity - nMsgLength ) < 2 )
            /// Not enough data
            return RC_MsgDefns.SuccessType.DIS_INSUFFICIENT_BUFFER;

         RC_MsgDefns.SuccessType eRetVal = RC_MsgDefns.SuccessType.DIS_OK;

         /// Call any provided read function
         if ( m_fnRead != null )
         {
            eRetVal = m_fnRead( ref m_nData );
         }

         /// Otherwise write the config data into the message
         pDataStart[nMsgLength++] = (byte)( ( m_nData >> 8 ) & 0xFF );
         pDataStart[nMsgLength] = (byte)( m_nData & 0xFF );

         /// Return the number of message bytes used
         nBytesUsed = 2;

         return eRetVal;
      }

      public override RC_MsgDefns.SuccessType writeData( byte[] pDataStart, int nRxBytes, int nByteId, ref int nBytesUsed, bool bIgnore )
      {
         if ( ( nRxBytes - nByteId ) < 2 )
            /// Not enough data
            return RC_MsgDefns.SuccessType.DIS_INSUFFICIENT_BUFFER;

         /// Return the number of message bytes used
         nBytesUsed = 2;

         if ( bIgnore )
            return RC_MsgDefns.SuccessType.DIS_OK;
         else
         {
            /// Otherwise write the config data
            int m_nData = ( pDataStart[nByteId++] ) << 8;
            m_nData |= ( pDataStart[nByteId++] );

            // /// Pass on the data
            if ( m_fnWrite != null )
               return m_fnWrite( (ushort)m_nData );
            else
               return RC_MsgDefns.SuccessType.DIS_OK;
         }
      }
   }

   public class cDI_Int : cDataItem
   {
      protected int m_nData;

      public Func<int, RC_MsgDefns.SuccessType> m_fnWrite;
      public delegate RC_MsgDefns.SuccessType ReadFunc( ref int nData );
      public ReadFunc m_fnRead;

      public cDI_Int( int nData, byte nIndentifier, RC_MsgDefns.AccessType eAccessType = RC_MsgDefns.AccessType.DIA_READ_WRITE )
          : base( nIndentifier, eAccessType )
      {
         m_nData = nData;
      }

      public override RC_MsgDefns.SuccessType readData( ref byte[] pDataStart, int nMsgCapacity, int nMsgLength, ref int nBytesUsed )
      {
         if ( ( nMsgCapacity - nMsgLength ) < 4 )
            /// Not enough data
            return RC_MsgDefns.SuccessType.DIS_INSUFFICIENT_BUFFER;

         RC_MsgDefns.SuccessType eRetVal = RC_MsgDefns.SuccessType.DIS_OK;

         /// Call any provided read function
         if ( m_fnRead != null )
         {
            eRetVal = m_fnRead( ref m_nData );
         }

         /// Otherwise write the config data into the message
         pDataStart[nMsgLength++] = (byte)( ( m_nData >> 24 ) & 0xFF );
         pDataStart[nMsgLength++] = (byte)( ( m_nData >> 16 ) & 0xFF );
         pDataStart[nMsgLength++] = (byte)( ( m_nData >> 16 ) & 0xFF );
         pDataStart[nMsgLength]   = (byte)(   m_nData         & 0xFF );

         /// Return the number of message bytes used
         nBytesUsed = 4;

         return eRetVal;
      }

      public override RC_MsgDefns.SuccessType writeData( byte[] pDataStart, int nRxBytes, int nByteId, ref int nBytesUsed, bool bIgnore )
      {
         if ( ( nRxBytes - nByteId ) < 4 )
            /// Not enough data
            return RC_MsgDefns.SuccessType.DIS_INSUFFICIENT_BUFFER;

         /// Return the number of message bytes used
         nBytesUsed = 4;

         if ( bIgnore )
            return RC_MsgDefns.SuccessType.DIS_OK;
         else
         {
            /// Otherwise write the config data
            int m_nData = ( pDataStart[nByteId++] ) << 24;
            m_nData |= ( pDataStart[nByteId++] ) << 16;
            m_nData |= ( pDataStart[nByteId++] ) << 8;
            m_nData |= ( pDataStart[nByteId++] );

            // /// Pass on the data
            if ( m_fnWrite != null )
               return m_fnWrite( m_nData );
            else
               return RC_MsgDefns.SuccessType.DIS_OK;
         }
      }
   }

   public class cDI_Fixed : cDataItem
   {
      protected float m_fData;

      public Func<float, RC_MsgDefns.SuccessType> m_fnWrite;
      public delegate RC_MsgDefns.SuccessType ReadFunc( ref float fData );
      public ReadFunc m_fnRead;


        public cDI_Fixed( float fData, byte nIndentifier, RC_MsgDefns.AccessType eAccessType = RC_MsgDefns.AccessType.DIA_READ_WRITE )
          : base( nIndentifier, eAccessType )
      {
         m_fData = fData;
      }

      public override RC_MsgDefns.SuccessType readData( ref byte[] pDataStart, int nMsgCapacity, int nMsgLength, ref int nBytesUsed )
      {
         if ( ( nMsgCapacity - nMsgLength ) < 4 )
            /// Not enough data
            return RC_MsgDefns.SuccessType.DIS_INSUFFICIENT_BUFFER;

         RC_MsgDefns.SuccessType eRetVal = RC_MsgDefns.SuccessType.DIS_OK;

         /// Call any provided read function
         if ( m_fnRead != null )
         {
            eRetVal = m_fnRead( ref m_fData );
         }

         /// Otherwise write the config data into the message
         int nVal = (int)( m_fData * 4096.0f );

         pDataStart[nMsgLength++] = (byte)( ( nVal >> 24 ) & 0xFF );
         pDataStart[nMsgLength++] = (byte)( ( nVal >> 16 ) & 0xFF );
         pDataStart[nMsgLength++] = (byte)( ( nVal >> 8 ) & 0xFF );
         pDataStart[nMsgLength] = (byte)( nVal & 0xFF );

         /// Return the number of message bytes used
         nBytesUsed = 4;

         return eRetVal;
      }

      public override RC_MsgDefns.SuccessType writeData( byte[] pDataStart, int nRxBytes, int nByteId, ref int nBytesUsed, bool bIgnore )
      {
         if ( ( nRxBytes - nByteId ) < 4 )
            /// Not enough data
            return RC_MsgDefns.SuccessType.DIS_INSUFFICIENT_BUFFER;

         /// Return the number of message bytes used
         nBytesUsed = 4;

         if ( bIgnore )
            return RC_MsgDefns.SuccessType.DIS_OK;
         else
         {
            /// Otherwise write the config data
            int nVal = ( (int)pDataStart[nByteId++] ) << 24;
            nVal |= ( (int)pDataStart[nByteId++] ) << 16;
            nVal |= ( (int)pDataStart[nByteId++] ) << 8;
            nVal |= ( (int)pDataStart[nByteId++] );

            m_fData = nVal / 4096.0f;

            /// Pass on the data
            if ( m_fnWrite != null )
               return m_fnWrite( m_fData );
            else
               return RC_MsgDefns.SuccessType.DIS_OK;
         }
      }
    }

   public class cDI_Int64 : cDataItem
   {
      public cDI_Int64( byte nIndentifier, RC_MsgDefns.AccessType eAccessType = RC_MsgDefns.AccessType.DIA_READ_WRITE )
          : base( nIndentifier, eAccessType )
      { }

      public override RC_MsgDefns.SuccessType readData( ref byte[] pDataStart, int nMsgCapacity, int nMsgLength, ref int nBytesUsed )
      {
         throw new NotImplementedException();
      }

      public override RC_MsgDefns.SuccessType writeData( byte[] pDataStart, int nRxBytes, int nByteId, ref int nBytesUsed, bool bIgnore )
      {
         throw new NotImplementedException();
      }
    }

   public class cDI_String : cDataItem
   {
      public cDI_String( byte nIndentifier, RC_MsgDefns.AccessType eAccessType = RC_MsgDefns.AccessType.DIA_READ_WRITE )
          : base( nIndentifier, eAccessType )
      { }

      public override RC_MsgDefns.SuccessType readData( ref byte[] pDataStart, int nMsgCapacity, int nMsgLength, ref int nBytesUsed )
      {
         throw new NotImplementedException();
      }

      public override RC_MsgDefns.SuccessType writeData( byte[] pDataStart, int nRxBytes, int nByteId, ref int nBytesUsed, bool bIgnore )
      {
         throw new NotImplementedException();
      }
    }

   public class cDI_Double : cDataItem
   {
        protected double m_nData;

        public Func<double, RC_MsgDefns.SuccessType> m_fnWrite;
        public delegate RC_MsgDefns.SuccessType ReadFunc(ref double nData);
        public ReadFunc m_fnRead;

        public cDI_Double(float nData, byte nIndentifier, RC_MsgDefns.AccessType eAccessType = RC_MsgDefns.AccessType.DIA_READ_WRITE )
          : base( nIndentifier, eAccessType )
        {
            m_nData = nData;
        }

      public override RC_MsgDefns.SuccessType readData( ref byte[] pDataStart, int nMsgCapacity, int nMsgLength, ref int nBytesUsed )
      {
            if ((nMsgCapacity - nMsgLength) < 8)
                /// Not enough data
                return RC_MsgDefns.SuccessType.DIS_INSUFFICIENT_BUFFER;

            RC_MsgDefns.SuccessType eRetVal = RC_MsgDefns.SuccessType.DIS_OK;
            if (m_fnRead != null)
            {
                eRetVal = m_fnRead(ref m_nData);
            }

            /// Otherwise write the config data into the message
            int nVal = (int)(m_nData * 4096.0f);

            /// Return the number of message bytes used
            nBytesUsed = 8;

            return eRetVal;
        }

      public override RC_MsgDefns.SuccessType writeData( byte[] pDataStart, int nRxBytes, int nByteId, ref int nBytesUsed, bool bIgnore )
      {
            if ((nRxBytes - nByteId) < 8)
                /// Not enough data
                return RC_MsgDefns.SuccessType.DIS_INSUFFICIENT_BUFFER;

            RC_MsgDefns.SuccessType eRetVal = RC_MsgDefns.SuccessType.DIS_OK;
            if (m_fnRead != null)
            {
                eRetVal = m_fnRead(ref m_nData);
            }

            /// Otherwise write the config data into the message
            int nVal = (int)(m_nData * 4096.0f);

            /// Return the number of message bytes used
            nBytesUsed = 8;

            return eRetVal;
        }

    }

   public class cDI_Float : cDataItem
   {
      public cDI_Float(byte nIndentifier, RC_MsgDefns.AccessType eAccessType = RC_MsgDefns.AccessType.DIA_READ_WRITE )
          : base( nIndentifier, eAccessType )
      { }

      public override RC_MsgDefns.SuccessType readData( ref byte[] pDataStart, int nMsgCapacity, int nMsgLength, ref int nBytesUsed )
      {
         throw new NotImplementedException();
      }

      public override RC_MsgDefns.SuccessType writeData( byte[] pDataStart, int nRxBytes, int nByteId, ref int nBytesUsed, bool bIgnore )
      {
         throw new NotImplementedException();
      }
    }
}
