using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Udp
{
    public static class RC_MsgDefinesCHARM
    {
        public enum MsgByte
        {
            MSG_BYTE_SUB_SYS = 1,        // Byte position of the sub-system Id
            MSG_BYTE_LENGTH_MSB = 2,     // Byte position of the Status
            MSG_BYTE_LENGTH_LSB = 3,     // Byte position of the Data length
            MSG_BYTE_FIRST_CMD = 4,      // Byte position of the Data
            MSG_OVERHEAD_BYTES = 5,
        }

        public enum RC_DataTypes
        {
            DT_BYTE,
            DT_U16,
            DT_S16,
            DT_U32,
            DT_S32,
            DT_U64,
            DT_S64,
            DT_U128,
            DT_FIXED,
            DT_STRING,
            DT_FLOAT,
            DT_DOUBLE,
            DT_LIMIT
        }

        public struct MessageStatus
        {
            bool bWarningSet;
            bool bErrorSet;
            bool bPeriodicResponce;
        }

        public const int MAX_MSG_DATA_DEFNS = 128;

        public const int CHARM_RC_RX_LEN_BYTE_POS = 4;        // Byte position of LSB of length byte in messages Rx by CHARM
        public const int CHARM_RC_TX_LEN_BYTE_POS = 5;        // Byte position of LSB of length byte in messages Tx from CHARM

        /// Bit mask required to read a datadefn
        public static int RC_READ(int x) { return (0x70 | x); }

        public const int RC_MSG_HEADER_CHAR = 0x10;

        //CHARM Sub-system ID's
        public const int RC_OBJECT_LOCATION_SUB_SYS = 0x00;
        public const int RC_PAN_TILT_SUB_SYS = 0x01;
        public const int RC_VIDEO_OVERLAY_SUB_SYS = 0x02;
        public const int RC_CAMERA_SUB_SYS = 0x03;
        public const int RC_SYNTH_TARGET_SUB_SYS = 0X04;

        //Bits of the status byte
        //public const int RC_STATUS_BIT_ERR = 0x01;
        //public const int RC_STATUS_BIT_WARN = 0x02;

        //public const int RC_STATUS_CSUMFAIL = (0x01 << 2);
        //public const int RC_STATUS_UNK_ITEM = (0x02 << 2);

        //public const int RC_STATUS_PERIODIC = (0x01 << 6);
        //public const int RC_STATUS_WARNING = (0x02 << 6);

        //public const int RC_PERIODIC_DATA_DEFN_BYTES = 16;

        /// Bits of the status byte
        public const int RC_STATUS_OK = 0;
        public const int RC_STATUS_BIT_ERR = 0x01;
        public const int RC_STATUS_BIT_WARN = 0x02;

        public const int RC_STATUS_CSUMFAIL = (0x01 << 2);
        public const int RC_STATUS_UNK_ITEM = (0x02 << 2);
        public const int RC_STATUS_NOT_SUPP = (0x03 << 2);
        public const int RC_STATUS_INV_STAT = (0x04 << 2);
        public const int RC_STATUS_INV_ACCE = (0x05 << 2);
        public const int RC_STATUS_NO_BUFF = (0x06 << 2);    // Buffer size does not match datatypes
        public const int RC_STATUS_ERROR = (0x07 << 2);      // Another error, normally to do with command execution
        public const int RC_STATUS_NOT_INIT = (0x08 << 2);   // Device is not initialised
        public const int RC_STATUS_BUSY = (0x09 << 2);       // Device is busy with previous commands or initialisation
        public const int RC_STATUS_NOT_CNTL = (0x0A << 2);   // Device is being controlled by another client

        public const int RC_STATUS_PERIODIC = (0x01 << 6);
        public const int RC_STATUS_WARNING = (0x02 << 6);

        public const int RC_PERIODIC_DATA_DEFN_BYTES = 16;

        ///////////////////////////////////////////////////////
        /// Common data definitions
        public const int DID_PeriodicMsgFreq = 0x70;
        public const int DID_PeriodicMsgDefn = 0x71;

        ///////////////////////////////////////////////////////
        /// Object Location (Sub-System 0x00)
        public const int DID_LocObjTrack = 0x00;
        public const int DID_LocAutoTrackSelect = 0x01;
        public const int DID_LocAutoTrackDelay = 0x02;
        public const int DID_LocTrackDuration = 0x03;
        public const int DID_LocDetPrioMetric = 0x04;
        public const int DID_LocDetTargetPrio = 0x05;
        public const int DID_LocTrackAimBias = 0x06;
        public const int DID_LocObjDetectionAlg = 0x0E;
        public const int DID_LocObjTrackAlg = 0x0F;
        public const int DID_LocDeteAreaPosY = 0x10;
        public const int DID_LocDeteAreaPosX = 0x11;
        public const int DID_LocDeteAreaSizeX = 0x12;
        public const int DID_LocDeteAreaSizeY = 0x13;

        ///////////////////////////////////////////////////////
        /// Pan and Tilt Control (Sub-System 0x01)
        /// 
        public const int DID_PedControlSource = 0x02;
        public const int DID_PedDeadbandMode = 0x03;
        public const int DID_PedAcuireBleed = 0x04;
        public const int DID_PedManualMode = 0x05;
        public const int DID_PedTrackModeAzGain = 0x10;
        public const int DID_PedTrackModeAzFilterP0 = 0x11;
        public const int DID_PedTrackModeAzFilterP1 = 0x12;
        public const int DID_PedTrackModeAzFilterI1 = 0x13;
        public const int DID_PedTrackModeELGain = 0x18;
        public const int DID_PedTrackModeElFilterP0 = 0x19;
        public const int DID_PedTrackModeElFilterP1 = 0x1A;
        public const int DID_PedTrackModeElFilterI1 = 0x1B;
        public const int DID_PedRateAz = 0x20;
        public const int DID_PedManualAzGain = 0x22;
        public const int DID_PedAimAzGain = 0x23;
        public const int DID_PedManualElRate = 0x28;
        public const int DID_PedManualElGain = 0x2A;
        public const int DID_PedAimpointElGain = 0x2B;
        public const int DID_PedAzDriftComp = 0x30;
        public const int DID_PedAzDeadbandComp = 0x31;
        public const int DID_PedAzBleedRate = 0x32;
        public const int DID_PedElDriftComp = 0x38;
        public const int DID_PedElDeadbandComp = 0x39;
        public const int DID_PedElBleedRate = 0x3A;
        public const int DID_PedControlMode = 0x40;
        public const int DID_PedAzRateDemand = 0x42;
        public const int DID_PedELRateDemand = 0x43;
        public const int DID_PedFeedbackMode = 0x44;
        public const int DID_PedAzAngleFeedback = 0x45;
        public const int DID_PedElAngleFeedback = 0x46;
        public const int DID_PedPointAngleLat = 0x47;
        public const int DID_PedAzRateFeedback = 0x48;
        public const int DID_PedElRateFeedback = 0x49;
        public const int DID_PedRateFeedbackLat = 0x4A;
        public const int DID_PedAimOffsetEnable = 0x4C;
        public const int DID_PedIncrAzAimpoint = 0x4D;
        public const int DID_PedIncrElAimpoint = 0x4E;
        public const int DID_PedInterfaceOutFormat = 0x50;




    }
}
