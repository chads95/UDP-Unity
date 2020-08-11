using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace Udp
{
    class cMsgHandlerSensor : cVMS_MsgHandler
    {
        SenSubSystem SenSub = GameObject.Find("Main Camera").GetComponent<SenSubSystem>();
        public cMsgHandlerSensor( byte uSubSysIndex )
        {
            m_cSubSysId = RC_MsgDefnsVMS.RC_CAMERA_SUB_SYS;     // Camera subsystem is index 0x03
            m_cSubSysIndex = uSubSysIndex;

            cDI_Byte Mode = new cDI_Byte(1, RC_MsgDefnsVMS.DID_SenMode, RC_MsgDefns.AccessType.DIA_READ_ONLY);
            cDI_Byte Calibrate = new cDI_Byte(0, RC_MsgDefnsVMS.DID_SenCalibrate, RC_MsgDefns.AccessType.DIA_WRITE_ONLY);
            cDI_Byte AutoFocus = new cDI_Byte(0, RC_MsgDefnsVMS.DID_SenAutoFocus, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Word FocusRate = new cDI_Word(0, RC_MsgDefnsVMS.DID_SenFocusRate, RC_MsgDefns.AccessType.DIA_WRITE_ONLY);
            cDI_Word FocusPosn = new cDI_Word(0, RC_MsgDefnsVMS.DID_SenFocusPosn, RC_MsgDefns.AccessType.DIA_READ_ONLY);
            cDI_Byte FocusStop = new cDI_Byte(0, RC_MsgDefnsVMS.DID_SenFocusStop, RC_MsgDefns.AccessType.DIA_WRITE_ONLY);
            cDI_Byte AperPosn = new cDI_Byte(0, RC_MsgDefnsVMS.DID_SenAperPosn, RC_MsgDefns.AccessType.DIA_WRITE_ONLY);
            cDI_Word ZoomRate = new cDI_Word(0, RC_MsgDefnsVMS.DID_SenZoomRate, RC_MsgDefns.AccessType.DIA_WRITE_ONLY);
            cDI_Word ZoomFOV = new cDI_Word(0, RC_MsgDefnsVMS.DID_SenZoomFOV, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Word ZoomPosn = new cDI_Word(0, RC_MsgDefnsVMS.DID_SenZoomPosn, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Byte ZoomMode = new cDI_Byte(0, RC_MsgDefnsVMS.DID_SenZoomMode, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Byte ZoomStop = new cDI_Byte(0, RC_MsgDefnsVMS.DID_SenZoomStop, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Byte ExpMode = new cDI_Byte(0, RC_MsgDefnsVMS.DID_SenExpMode, RC_MsgDefns.AccessType.DIA_READ_WRITE); 
            cDI_Byte ExpPosn = new cDI_Byte(0, RC_MsgDefnsVMS.DID_SenExpPosn, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Byte GainMode = new cDI_Byte(0, RC_MsgDefnsVMS.DID_SenGainMode, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Word GainRate = new cDI_Word(0, RC_MsgDefnsVMS.DID_SenGainRate, RC_MsgDefns.AccessType.DIA_WRITE_ONLY);
            cDI_Word GainPosn = new cDI_Word(0, RC_MsgDefnsVMS.DID_SenGainPosn, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Word GainStop = new cDI_Word(0, RC_MsgDefnsVMS.DID_SenGainStop, RC_MsgDefns.AccessType.DIA_WRITE_ONLY);
            cDI_Word OffsetRate = new cDI_Word(0, RC_MsgDefnsVMS.DID_SenOffsetRate, RC_MsgDefns.AccessType.DIA_WRITE_ONLY);
            cDI_Word OffsetPosn = new cDI_Word(0, RC_MsgDefnsVMS.DID_SenOffsetPosn, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Byte OffsetStop = new cDI_Byte(0, RC_MsgDefnsVMS.DID_SenOffsetStop, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Byte Comms = new cDI_Byte(2, RC_MsgDefnsVMS.DID_SenComms, RC_MsgDefns.AccessType.DIA_READ_ONLY);
            cDI_Word Warnings = new cDI_Word(0, RC_MsgDefnsVMS.DID_SenWarnings, RC_MsgDefns.AccessType.DIA_READ_ONLY);
            cDI_Word Errors = new cDI_Word(0, RC_MsgDefnsVMS.DID_SenErrors, RC_MsgDefns.AccessType.DIA_READ_ONLY);



            Mode.m_fnRead = OnGetMode;
            FocusRate.m_fnWrite = OnSetFocusRate;
            FocusPosn.m_fnRead = OnGetFocusPosn;
            FocusStop.m_fnWrite = OnSetFocusStop;
            AperPosn.m_fnWrite = OnSetAperPosn;
            ZoomRate.m_fnWrite = OnSetZoomRate;
            ZoomFOV.m_fnWrite = OnSetZoomFOV;
            ZoomFOV.m_fnRead = OnGetZoomFOV;
            ZoomPosn.m_fnWrite = OnSetZoomPosn;
            ZoomPosn.m_fnRead = OnGetZoomPosn;
            ZoomMode.m_fnWrite = OnSetZoomMode;
            ZoomMode.m_fnRead = OnGetZoomMode;
            ZoomStop.m_fnWrite = OnSetZoomStop;
            ZoomStop.m_fnRead = OnGetZoomStop;
            ExpMode.m_fnWrite = OnSetExpMode;
            ExpMode.m_fnRead = OnGetExpMode;
            ExpPosn.m_fnWrite = OnSetExpPosn;
            ExpPosn.m_fnRead = OnGetExpPosn;
            GainMode.m_fnWrite = OnSetGainMode;
            GainMode.m_fnRead = OnGetGainMode;
            GainRate.m_fnWrite = OnSetGainRate;
            GainStop.m_fnWrite = OnSetGainStop;
            OffsetRate.m_fnWrite = OnSetOffsetRate;
            Comms.m_fnRead = OnGetComms;
            Warnings.m_fnRead = OnGetWarnings;
            Errors.m_fnRead = OnGetErrors;

            vecDataItems.Add(Mode);
            vecDataItems.Add(FocusRate);
            vecDataItems.Add(FocusPosn);
            vecDataItems.Add(FocusStop);
            vecDataItems.Add(AperPosn);
            vecDataItems.Add(ZoomRate);
            vecDataItems.Add(ZoomFOV);
            vecDataItems.Add(ZoomPosn);
            vecDataItems.Add(ZoomMode);
            vecDataItems.Add(ZoomStop);
            vecDataItems.Add(ExpMode);
            vecDataItems.Add(ExpPosn);
            vecDataItems.Add(GainMode);
            vecDataItems.Add(GainRate);
            vecDataItems.Add(GainPosn);
            vecDataItems.Add(GainStop);
            vecDataItems.Add(OffsetRate);
            vecDataItems.Add(OffsetPosn);
            vecDataItems.Add(OffsetStop);
            vecDataItems.Add(Comms);
            vecDataItems.Add(Warnings);
            vecDataItems.Add(Errors);
        }

      public RC_MsgDefns.SuccessType OnGetMode( ref byte nData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnSetFocusRate( ushort nData )
        {
            SenSub.OnSetFocusRate(nData);
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnGetFocusPosn( ref ushort nData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnSetFocusStop( byte nData )
        {
            SenSub.OnSetFocusStop(nData);
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnSetAperPosn( byte nData )
        {
            SenSub.OnSetAperPons(nData);
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnSetZoomRate( ushort nData )
        {
            SenSub.OnSetZoomRate(nData);
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnSetZoomFOV( ushort nData )
        {
            SenSub.OnSetZoomFOV(nData);
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
        public RC_MsgDefns.SuccessType OnGetZoomFOV(ref ushort nData)
        {
            nData = SenSub.OnGetZoomFOV();
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnSetZoomPosn( ushort nData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnGetZoomPosn( ref ushort nData )
        {
            //fData = udp.OnGetZoomPons();
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnSetZoomMode( byte nData )
        {
            SenSub.OnSetZoomMode(nData);
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnGetZoomMode( ref byte nData )
        {
            nData = SenSub.OnGetZoomMode();
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnSetZoomStop( byte nData )
        {
            Debug.Log("Set Zoom Stop");
            SenSub.OnSetZoomStop(nData);
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnGetZoomStop( ref byte nData )
        {
            //nData = udp.OnGetZoomStop();
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

      public RC_MsgDefns.SuccessType OnSetExpMode( byte nData )
        {
            SenSub.OnSetExpMode(nData);
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

      public RC_MsgDefns.SuccessType OnGetExpMode( ref byte nData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnSetExpPosn( byte nData )
        {
            Debug.Log("SetExpPosn");
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnGetExpPosn( ref byte nData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnSetGainMode( ref byte nData )
        {
            Debug.Log("SetGainMode");
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnSetGainMode( byte nData )
        {
            Debug.Log("SetGainMode");
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnGetGainMode( ref byte nData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnSetGainRate( ushort nData )
        {
            Debug.Log("SetGainRate");
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnSetGainStop( ushort nData )
        {
            Debug.Log("SetGainStop");
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnSetOffsetRate( ushort nData )
        {
            Debug.Log("SetOffSetRate");
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnGetComms( ref byte nData )
        {
            nData = SenSub.OnGetCommsSens();
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnGetWarnings( ref ushort nData )
        {
            nData = SenSub.OnGetWarningsSens();
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
      public RC_MsgDefns.SuccessType OnGetErrors( ref ushort nData )
        {
            nData = SenSub.OnGetErrorsSens();
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

    }
}
