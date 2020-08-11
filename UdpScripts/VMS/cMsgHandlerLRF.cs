using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Udp
{
    class cMsgHandlerLRF : cVMS_MsgHandler
    {
        public cMsgHandlerLRF( byte uSubSysIndex )
        {
            m_cSubSysId = RC_MsgDefnsVMS.RC_LRF_SUB_SYS;     // LRF subsystem is index 0x04
            m_cSubSysIndex = uSubSysIndex;


            cDI_Byte comms = new cDI_Byte(1, RC_MsgDefnsVMS.DID_LRF_Comms, RC_MsgDefns.AccessType.DIA_READ_ONLY);
            cDI_Byte Fire = new cDI_Byte(1, RC_MsgDefnsVMS.DID_LRF_FireOne, RC_MsgDefns.AccessType.DIA_WRITE_ONLY);
            cDI_Fixed Burst = new cDI_Fixed(0, RC_MsgDefnsVMS.DID_LRF_FireBurst, RC_MsgDefns.AccessType.DIA_WRITE_ONLY);
            cDI_Byte Status = new cDI_Byte(1, RC_MsgDefnsVMS.DID_LRF_Status, RC_MsgDefns.AccessType.DIA_READ_ONLY);
            cDI_Byte Result = new cDI_Byte(1, RC_MsgDefnsVMS.DID_LRF_FireRes, RC_MsgDefns.AccessType.DIA_READ_ONLY);
            cDI_Word Range = new cDI_Word(1, RC_MsgDefnsVMS.DID_LRF_FireRng, RC_MsgDefns.AccessType.DIA_READ_ONLY);
            cDI_Int Time = new cDI_Int(1, RC_MsgDefnsVMS.DID_LRF_FireTime, RC_MsgDefns.AccessType.DIA_READ_ONLY);
            cDI_Byte Power = new cDI_Byte(1, RC_MsgDefnsVMS.DID_LRF_OnOff, RC_MsgDefns.AccessType.DIA_READ_ONLY);
            cDI_Byte Enable = new cDI_Byte(1, RC_MsgDefnsVMS.DID_LRF_Enable, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Fixed Warnings = new cDI_Fixed(1, RC_MsgDefnsVMS.DID_LRF_Warnings, RC_MsgDefns.AccessType.DIA_READ_ONLY);
            cDI_Fixed Errors = new cDI_Fixed(1, RC_MsgDefnsVMS.DID_LRF_Errors, RC_MsgDefns.AccessType.DIA_READ_ONLY);

            comms.m_fnRead = OnGetComms;
            Fire.m_fnWrite = OnSetFire;
            Burst.m_fnWrite = OnSetBurst;
            Status.m_fnRead = OnGetStatus;
            Result.m_fnRead = OnGetResult;
            Range.m_fnRead = OnGetRange;
            Time.m_fnRead = OnGetTime;
            Power.m_fnRead = OnGetPower;
            Enable.m_fnWrite = OnSetEnable;
            Enable.m_fnRead = OnGetEnable;
            Warnings.m_fnRead = OnGetWarnings;
            Errors.m_fnRead = OnGetErrors;



            vecDataItems.Add(comms);
            vecDataItems.Add(Fire);
            vecDataItems.Add(Burst);
            vecDataItems.Add(Status);
            vecDataItems.Add(Result);
            vecDataItems.Add(Time);
            vecDataItems.Add(Power);
            vecDataItems.Add(Enable);
            vecDataItems.Add(Warnings);
            vecDataItems.Add(Errors);
        }

        public RC_MsgDefns.SuccessType OnGetComms(ref byte nData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnSetFire (byte nData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnSetBurst(float nData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnGetStatus(ref byte nData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnGetResult(ref byte nData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnGetRange(ref ushort nData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnGetTime(ref int nData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnGetPower(ref byte nData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnSetEnable(byte nData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnGetEnable(ref byte nData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnGetWarnings(ref float nData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnGetErrors(ref float nData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
    }
}
