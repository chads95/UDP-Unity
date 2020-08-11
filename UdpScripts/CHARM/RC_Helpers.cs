using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Udp
{
    public interface RC_Helpers
    {
        ref byte pMsgBuf();
        int nCurrentByte();
    }

    public class RC_Data
    {
        public RC_Data() { bChanged = false; }
        ~RC_Data() {;}

        public bool bChanged;
    }

    class RC_Byte : RC_Data
    {
        public RC_Byte() { nData = 0; }
        ~RC_Byte() {;}

        public byte nData;
    }

    class RC_U16 : RC_Data
    {
        public RC_U16() { nData = 0; }
        ~RC_U16() {;}

        public ushort nData;
    }

    class RC_S16 : RC_Data
    {
        public RC_S16() { nData = 0; }
        ~RC_S16() {; }

        public short nData;
    }

    class RC_U32 : RC_Data
    {
        public RC_U32() { nData = 0; }
        ~RC_U32() {; }

        public uint nData;
    }

    class RC_S32 : RC_Data
    {
        public RC_S32() { nData = 0; }
        ~RC_S32() {; }

        public int nData;
    }

    class RC_U64 : RC_Data
    {
        public RC_U64() { nData = 0; }
        ~RC_U64() {; }

        public ulong nData;
    }

    class RC_S64 : RC_Data
    {
        public RC_S64() { nData = 0; }
        ~RC_S64(){; }

        public long nData;
    }

    class RC_U128 : RC_Data
    {
        public RC_U128() { nDataHigh = nDataLow = 0; }
        ~RC_U128() {; }

        public ulong nDataHigh;
        public ulong nDataLow;
    }

    class RC_Fixed : RC_Data
    {
        public RC_Fixed() { nData = 0; fData = 0; }
        ~RC_Fixed() {;}

        public int nData;
        public float fData;
    }

    class RC_String : RC_Data
    {
        public RC_String() { sData = ""; }
        ~RC_String() {; }

        public string sData;
    }

    class RC_Float : RC_Data
    {
        public RC_Float() { fData = 0.0f; }
        ~RC_Float() {; }

        public float fData;
    }

    class RC_Double : RC_Data
    {
        public RC_Double() { dData = 0.0; }
        ~RC_Double() {; }

        public double dData;
    }

    public struct RC_Item
    {
        public byte nSubsystem;
        public byte bSubIndex;
        public byte nItem;
        public RC_MsgDefinesCHARM.RC_DataTypes nType;
        public RC_MsgDefns.AccessType nAccess;
        public RC_Data stData;
        public string sLabe;
    }
}
