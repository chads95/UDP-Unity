using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Udp
{
    class cMsgHandlerEOD : cVMS_MsgHandler
    {
        EODSubSystem EODSub = GameObject.Find("Main Camera").GetComponent<EODSubSystem>();
        public cMsgHandlerEOD( byte uSubSysIndex )
        {
            m_cSubSysId = RC_MsgDefnsVMS.RC_PLATFORM_SUB_SYS;     // Platform subsystem is index 0x02
            m_cSubSysIndex = uSubSysIndex;

            cDI_Byte Mode = new cDI_Byte(0, RC_MsgDefnsVMS.DID_PedControlMode, RC_MsgDefns.AccessType.DIA_READ_ONLY);
            cDI_Byte State = new cDI_Byte( 1, RC_MsgDefnsVMS.DID_PedState, RC_MsgDefns.AccessType.DIA_READ_ONLY );
            cDI_Fixed DeckPosnAz = new cDI_Fixed( 0.0f, RC_MsgDefnsVMS.DID_PedDeckPosnAz, RC_MsgDefns.AccessType.DIA_READ_WRITE );
            cDI_Fixed DeckPosnEl = new cDI_Fixed( 0.0f, RC_MsgDefnsVMS.DID_PedDeckPosnEl, RC_MsgDefns.AccessType.DIA_READ_WRITE );
            cDI_Fixed DeckRateAz = new cDI_Fixed( 0.0f, RC_MsgDefnsVMS.DID_PedDeckRateAz, RC_MsgDefns.AccessType.DIA_WRITE_ONLY );
            cDI_Fixed DeckRateEl = new cDI_Fixed( 0.0f, RC_MsgDefnsVMS.DID_PedDeckRateEl, RC_MsgDefns.AccessType.DIA_WRITE_ONLY );
            cDI_Fixed GyroPosnAz = new cDI_Fixed( 0.0f, RC_MsgDefnsVMS.DID_PedGyroPosnAz, RC_MsgDefns.AccessType.DIA_READ_WRITE );
            cDI_Fixed GyroPosnEl = new cDI_Fixed( 0.0f, RC_MsgDefnsVMS.DID_PedGyroPosnEl, RC_MsgDefns.AccessType.DIA_READ_WRITE );
            cDI_Fixed GyroRateAz = new cDI_Fixed( 0.0f, RC_MsgDefnsVMS.DID_PedGyroRateAz, RC_MsgDefns.AccessType.DIA_WRITE_ONLY );
            cDI_Fixed GyroRateEl = new cDI_Fixed( 0.0f, RC_MsgDefnsVMS.DID_PedGyroRateEl, RC_MsgDefns.AccessType.DIA_WRITE_ONLY );
            cDI_Double Longitude = new cDI_Double(0.0f, RC_MsgDefnsVMS.DID_PedLongitude, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Double Latitude = new cDI_Double(0.0f, RC_MsgDefnsVMS.DID_PedLatitude, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Fixed Height = new cDI_Fixed(0.0f, RC_MsgDefnsVMS.DID_PedHeight, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Byte GyroNull = new cDI_Byte(0, RC_MsgDefnsVMS.DID_PedGyroNull, RC_MsgDefns.AccessType.DIA_READ_WRITE);
            cDI_Fixed AlignAz = new cDI_Fixed( 0.0f, RC_MsgDefnsVMS.DID_PedAlignAz, RC_MsgDefns.AccessType.DIA_READ_WRITE );
            cDI_Fixed AlignEl = new cDI_Fixed( 0.0f, RC_MsgDefnsVMS.DID_PedAlignEl, RC_MsgDefns.AccessType.DIA_READ_WRITE );
            cDI_Fixed TiltMagn = new cDI_Fixed( 0.0f, RC_MsgDefnsVMS.DID_PedTiltMagn, RC_MsgDefns.AccessType.DIA_READ_WRITE );
            cDI_Fixed TiltDirn = new cDI_Fixed( 0.0f, RC_MsgDefnsVMS.DID_PedTiltDirn, RC_MsgDefns.AccessType.DIA_READ_WRITE );
            cDI_Byte Comms = new cDI_Byte( 2, RC_MsgDefnsVMS.DID_PedComms, RC_MsgDefns.AccessType.DIA_READ_ONLY );
            cDI_Word Warnings = new cDI_Word( 0, RC_MsgDefnsVMS.DID_PedWarnings, RC_MsgDefns.AccessType.DIA_READ_ONLY );
            cDI_Word Errors = new cDI_Word( 0, RC_MsgDefnsVMS.DID_PedErrors, RC_MsgDefns.AccessType.DIA_READ_ONLY );

            State.m_fnRead = OnGetState;
            Mode.m_fnRead = OnGetMode;
            //DeckPosn
            DeckPosnAz.m_fnWrite = OnSetPosnAz;
            DeckPosnEl.m_fnWrite = OnSetPosnEl;
            DeckPosnAz.m_fnRead = OnGetPosnAz;
            DeckPosnEl.m_fnRead = OnGetPosnEl;
            //DeckRate
            DeckRateAz.m_fnWrite = OnSetRateAz;
            DeckRateEl.m_fnWrite = OnSetRateEl;
            //GyroPosn
            GyroPosnAz.m_fnWrite = OnSetGyroPosnAz;
            GyroPosnEl.m_fnWrite = OnSetGyroPosnEl;
            GyroPosnAz.m_fnRead = OnGetGyroPosnAz;
            GyroPosnEl.m_fnRead = OnGetGyroPosnEl;
            //GyroRate
            GyroRateAz.m_fnWrite = OnSetGyroRateAz;
            GyroRateEl.m_fnWrite = OnSetGyroRateEl;
            GyroNull.m_fnWrite = OnSetGyroNull;
            GyroNull.m_fnRead = OnGetGyroNull;
            //Align
            AlignAz.m_fnWrite = OnSetAlignAz;
            AlignAz.m_fnWrite = OnSetAlignEl;
            AlignAz.m_fnRead = OnGetAlignAz;
            AlignAz.m_fnRead = OnGetAlignEl;
            //Tilt
            TiltDirn.m_fnWrite = OnSetTiltDirn;
            TiltMagn.m_fnWrite = OnSetTiltMagn;
            TiltDirn.m_fnRead = OnGetTiltDirn;
            TiltMagn.m_fnRead = OnGetTiltMagn;
            //Height
            Height.m_fnWrite = OnSetHeight;
            Height.m_fnRead = OnGetHeight;
            //Global Position
            Latitude.m_fnWrite = OnSetLatitude;
            Latitude.m_fnRead = OnGetLatitude;
            Longitude.m_fnWrite = OnSetLongitude;
            Longitude.m_fnRead = OnGetLongitude;
            //Warnings/Errors/Comms
            Comms.m_fnRead = OnGetComms;
            Warnings.m_fnRead = OnGetWarnings;
            Errors.m_fnRead = OnGetErrors;

            vecDataItems.Add(Mode);
            vecDataItems.Add( State );
            vecDataItems.Add( DeckPosnAz );
            vecDataItems.Add( DeckPosnEl );
            vecDataItems.Add( DeckRateAz );
            vecDataItems.Add( DeckRateEl );
            vecDataItems.Add( GyroPosnAz );
            vecDataItems.Add( GyroPosnEl );
            vecDataItems.Add( GyroRateAz );
            vecDataItems.Add( GyroRateEl );
            vecDataItems.Add(GyroNull);
            vecDataItems.Add( AlignAz );
            vecDataItems.Add( AlignEl );
            vecDataItems.Add( TiltMagn );
            vecDataItems.Add( TiltDirn );
            vecDataItems.Add( Height );
            vecDataItems.Add( Comms );
            vecDataItems.Add( Warnings );
            vecDataItems.Add( Errors );
        }

        //Deck
        public RC_MsgDefns.SuccessType OnGetMode(ref byte fData)
        {
            fData = 0x02;
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnGetState(ref byte fData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
        public RC_MsgDefns.SuccessType OnSetPosnAz( float fData )
        {
            EODSub.SetPosnAz(fData);
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnSetPosnEl( float fData )
        {
            EODSub.SetPosnEl(fData);
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
        public RC_MsgDefns.SuccessType OnGetPosnAz(ref float fData)
        {
            fData = EODSub.GetPosAz();
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
        public RC_MsgDefns.SuccessType OnGetPosnEl(ref float fData )
        {
            fData = EODSub.GetPosEl();
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
        public RC_MsgDefns.SuccessType OnSetRateAz( float fData )
        {
            EODSub.SetAzRate(fData);
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnSetRateEl( float fData)
        {
            EODSub.SetElRate(fData);
            return RC_MsgDefns.SuccessType.DIS_OK;
        }


        //Gyro
        public RC_MsgDefns.SuccessType OnSetGyroPosnAz(float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnSetGyroPosnEl( float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnGetGyroPosnEl( ref float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
        public RC_MsgDefns.SuccessType OnGetGyroPosnAz( ref float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnSetGyroRateAz( float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnSetGyroRateEl( float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnSetGyroNull(byte fData)
        {
            LogHelper.Log(LogTarget.DebugLog, "Set Gyro Null");
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        public RC_MsgDefns.SuccessType OnGetGyroNull(ref byte fData)
        {
            fData = 0x00;
            LogHelper.Log(LogTarget.DebugLog, "Get Gyro Null");
            return RC_MsgDefns.SuccessType.DIS_OK;
        }


        //Align
        private RC_MsgDefns.SuccessType OnSetAlignAz( float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        private RC_MsgDefns.SuccessType OnSetAlignEl( float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        private RC_MsgDefns.SuccessType OnGetAlignEl( ref float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        private RC_MsgDefns.SuccessType OnGetAlignAz( ref float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        //Tilt
        private RC_MsgDefns.SuccessType OnSetTiltMagn( float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        private RC_MsgDefns.SuccessType OnSetTiltDirn( float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        private RC_MsgDefns.SuccessType OnGetTiltMagn( ref float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        private RC_MsgDefns.SuccessType OnGetTiltDirn( ref float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
        
        //Height
        private RC_MsgDefns.SuccessType OnSetHeight( float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        private RC_MsgDefns.SuccessType OnGetHeight( ref float fData )
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
        //Ped Global position
        private RC_MsgDefns.SuccessType OnSetLatitude (double fData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
        private RC_MsgDefns.SuccessType OnGetLatitude(ref double fData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
        private RC_MsgDefns.SuccessType OnSetLongitude(double fData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
        private RC_MsgDefns.SuccessType OnGetLongitude(ref double fData)
        {
            return RC_MsgDefns.SuccessType.DIS_OK;
        }

        //Warnings/Errors
        private RC_MsgDefns.SuccessType OnGetComms (ref byte fData)
        {
            fData = EODSub.OnGetCommsEOD();
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
        private RC_MsgDefns.SuccessType OnGetWarnings( ref ushort fData)
        {
            fData = EODSub.OnGetWarningsEOD();
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
        private RC_MsgDefns.SuccessType OnGetErrors( ref ushort fData )
        {
            fData = EODSub.OnGetErrorsEOD();
            return RC_MsgDefns.SuccessType.DIS_OK;
        }
    }
}