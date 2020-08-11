using System;
using System.Collections.Generic;
using System.Text;

namespace Udp
{
    public static class RC_MsgDefnsVMS
    {

        // Definitions with connection functionality
        public enum MsgByte
        {
            MSG_BYTE_SUB_SYS = 1,        // Byte position of the sub-system Id
            MSG_BYTE_SUB_SYS_INDEX = 2,  // Byte position of the sub-system Index
            MSG_BYTE_LENGTH_MSB = 3,     // Byte Position of the Status
            MSG_BYTE_LENGTH_LSB = 4,     // Byte Position of the Message Length
            MSG_BYTE_FIRST_CMD = 5,      // Byte Position of the Message
        }

        public const int MAX_MSG_DATA_DEFNS = 128;

        public const int VMS_RC_RX_LEN_BYTE_POS = 4;        // Byte position of LSB of length byte in messages Rx by VMS
        public const int VMS_RC_TX_LEN_BYTE_POS = 5;        // Byte position of LSB of length byte in messages Tx from VMS

        /// Bit mask required to read a datadefn
        public static int RC_READ( int x ) { return ( 0x80 | x ); }

        /// Header character that defines a VMS remote control protocol message
        public const int RC_MSG_HEADER_CHAR = 0x99;

        /// VMS Sub-system IDs
        public const int RC_SYSTEM_SUB_SYS = 0x01;
        public const int RC_PLATFORM_SUB_SYS = 0x02;
        public const int RC_CAMERA_SUB_SYS = 0x03;
        public const int RC_LRF_SUB_SYS = 0x04;
        public const int RC_TRACKER_SUB_SYS = 0x05;
        public const int RC_WAS_SUB_SYS = 0x06;          // NB Slowly replacing WAS with Scan
        public const int RC_SCAN_SUB_SYS = 0x06;
        public const int RC_RECORDER_SUB_SYS = 0x07;     // Lacking documentation
        public const int RC_TRACKER_OVL_SUB_SYS = 0x07;
        public const int RC_TAR_DES_SUB_SYS = 0x08;
        public const int RC_CNTL_SRC_SUB_SYS = 0x09;     // BAE specific platform control source
        public const int RC_GPS_SUB_SYS = 0x0A;
        public const int RC_ENC_STRM_REC_SUB_SYS = 0x10;
        public const int RC_GPIO_SUB_SYS = 0x14;
        public const int RC_VIDEO_CTRL_SUB_SYS = 0x16;
        public const int RC_EXT_DEV_SUB_SYS = 0x20;
        public const int RC_AUDIO_OUT_SUB_SYS = 0x21;
        public const int RC_GUN_SUB_SYS = 0x22;           // AUDS Specific gun (based on an EOD)
        public const int RC_DISRUPTOR_SUB_SYS = 0x23;     // AUDS Specific disruptor
        public const int RC_RAYTHEON_SUB_SYS = 0x24;      // AUDS Raytheon Specific device
        public const int RC_GPS_VRU_SUB_SYS = 0x25;       // AUDS Conex Specific device
        public const int RC_DEBUG_ONLY = 0xF0;            //

        /// Bits of the status byte
        public const int RC_STATUS_BIT_ERR = 0x01;
        public const int RC_STATUS_BIT_WARN = 0x02;

        public const int RC_STATUS_CSUMFAIL = ( 0x01 << 2 );
        public const int RC_STATUS_UNK_ITEM = ( 0x02 << 2 );
        public const int RC_STATUS_NOT_SUPP = ( 0x03 << 2 );
        public const int RC_STATUS_INV_STAT = ( 0x04 << 2 );
        public const int RC_STATUS_INV_ACCE = ( 0x05 << 2 );
        public const int RC_STATUS_NO_BUFF = ( 0x06 << 2 );    // Buffer size does not match datatypes
        public const int RC_STATUS_ERROR = ( 0x07 << 2 );      // Another error, normally to do with command execution
        public const int RC_STATUS_NOT_INIT = ( 0x08 << 2 );   // Device is not initialised
        public const int RC_STATUS_BUSY = ( 0x09 << 2 );       // Device is busy with previous commands or initialisation
        public const int RC_STATUS_NOT_CNTL = ( 0x0A << 2 );   // Device is being controlled by another client

        public const int RC_STATUS_PERIODIC = ( 0x01 << 6 );
        public const int RC_STATUS_WARNING = ( 0x02 << 6 );

        public const int RC_PERIODIC_DATA_DEFN_BYTES = 16;

        ///////////////////////////////////////////////////////
        /// Common data definitions
        public const int DID_PeriodicMsgFreq = 0x70;
        public const int DID_PeriodicMsgDefn = 0x71;

        ///////////////////////////////////////////////////////
        /// System / GUI data definitions (Sub-System 0x01)
        public const int DID_SysOnOff = 0x00;
        public const int DID_SysAzCoord = 0x01;
        public const int DID_SysMode = 0x03;
        public const int DID_SysSensor = 0x04;
        public const int DID_SysDatalog = 0x05;     // Enable / disable system datalogging
        public const int DID_SysReloadConfig = 0x06;
        public const int DID_SysReacqMode = 0x0A;      // Is automatic re-acquire mode enabled?
        public const int DID_SysReacqPosn = 0x0B;      // Posn that re0acquire mode drives to
        public const int DID_SysLocComms = 0x10;      // State of communications with an external device
        public const int DID_SysLocTime = 0x11;      // Timestamp of the location update
        public const int DID_SysLocDOP = 0x12;      // GPS Dilution of position / confidence
        public const int DID_SysLatitude = 0x13;
        public const int DID_SysLongitude = 0x14;
        public const int DID_SysHeight = 0x15;
        public const int DID_SysLocState = 0x16;      // Validity and quality of Location data
        public const int DID_SysOriComms = 0x18;      // State of communications with an external device
        public const int DID_SysOriTime = 0x19;      // Timestamp of the location update
        public const int DID_SysRoll = 0x1A;
        public const int DID_SysPitch = 0x1B;
        public const int DID_SysYaw = 0x1C;
        public const int DID_SysOriState = 0x1D;      // Validity and quality of orientation data
        public const int DID_SysStartWash = 0x20;
        public const int DID_SysWashAzimuth = 0x21;
        public const int DID_SysWashElevation = 0x22;
        public const int DID_SysWashStatus = 0x23;
        public const int DID_SysParkAzimuth = 0x24;
        public const int DID_SysParkElevation = 0x25;
        public const int DID_SysMatchedFov = 0x26;
        public const int DID_SysRunBIT = 0x2F;     // Run the whole system BIT (Indra)
        public const int DID_SysWarnings = 0x31;
        public const int DID_SysErrors = 0x32;
        public const int DID_SysOpHours = 0x33;
        public const int DID_SysVersion = 0x34;
        public const int DID_SysDiskSpace = 0x36;     // Disk space
        public const int DID_SysCh1VidInput = 0x38;
        public const int DID_SysCh2VidInput = 0x39;
        public const int DID_SysControl = 0x72;
        public const int DID_SysReqControl = 0x73;

        ///////////////////////////////////////////////////////
        /// Remote Control enable / disable data definitions
        public const int DID_RC_Enable = 0x00;

        ///////////////////////////////////////////////////////
        /// Pan and Tilt control data definitions (Sub-System 0x02)
        public const int DID_PedState = 0x00;
        public const int DID_PedDeckPosnAz = 0x01;
        public const int DID_PedDeckPosnEl = 0x02;
        public const int DID_PedDeckRateAz = 0x03;
        public const int DID_PedDeckRateEl = 0x04;
        public const int DID_PedGyroPosnAz = 0x05;
        public const int DID_PedGyroPosnEl = 0x06;
        public const int DID_PedGyroRateAz = 0x07;
        public const int DID_PedGyroRateEl = 0x08;
        public const int DID_PedUpdateTime = 0x09;
        public const int DID_PedControlMode = 0x0A;
        public const int DID_PedGyroNull = 0x0B;
        public const int DID_PedAutoNullTime = 0x0C;
        public const int DID_PedAutoNullState = 0x0D;
        public const int DID_PedAlignAz = 0x20;
        public const int DID_PedAlignEl = 0x21;
        public const int DID_PedTiltMagn = 0x22;
        public const int DID_PedTiltDirn = 0x23;
        public const int DID_PedHeight = 0x24;
        public const int DID_PedLatitude = 0x25;
        public const int DID_PedLongitude = 0x26;
        public const int DID_PedHostRoll = 0x27;
        public const int DID_PedHostPitch = 0x28;
        public const int DID_PedHostYaw = 0x29;
        public const int DID_PedHostRPY_Valid = 0x2A;    // Validity of the previous data
        public const int DID_PedRunBIT = 0x2F;    // Run the pedestal BIT
        public const int DID_PedComms = 0x30;
        public const int DID_PedWarnings = 0x31;
        public const int DID_PedErrors = 0x32;
        public const int DID_PedVersion = 0x34;

        public const int DID_PedCustomStart = 0x40;  // Custom Range
        public const int DID_PedCustomEnd = 0x57;

        public const int DID_PedDeadbandMode = 0x58;
        public const int DID_PedAcqBleedEn = 0x59;
        public const int DID_PedPID_GainAz = 0x5A;
        public const int DID_PedPID_OmegaAz = 0x5B;
        public const int DID_PedPID_GainEl = 0x5C;
        public const int DID_PedPID_OmegaEl = 0x5D;
        public const int DID_PedDriftAz = 0x5E;
        public const int DID_PedDriftEl = 0x5F;
        public const int DID_PedDeadbandAz = 0x60;
        public const int DID_PedDeadbandEl = 0x61;
        public const int DID_PedBleedRateAz = 0x62;
        public const int DID_PedBleedRateEl = 0x63;


        // Custom for Kelvin Hughes
        // Allows access to Goto pre-set position in Pelco-D Protocol
        // Implementation will be as per Silent Sentinel Osiris ICD (SSL DN171701 - v1.3 DRAFT)
        public const int DID_PedGotoPresetCmd = 0x40;  ///< Custom Range

        // Custom for AUDS Stryker
        public const int DID_GunArmed = 0x40;    ///< AUDS Specific whether the trigger is 'hot' or not
        public const int DID_GunTrigger = 0x41;    ///< AUDS Specific whether the trigger is 'pulled' or not
        public const int DID_GunAmmoCount = 0x42;    ///< AUDS Specific ammunition remaining

        ///////////////////////////////////////////////////////
        /// Video Overlay data definitions

        ///////////////////////////////////////////////////////
        /// Camera data definitions (Sub-System 0x03)
        public const int DID_SenOnOff = 0x00;
        public const int DID_SenMode = 0x01;
        public const int DID_SenCalibrate = 0x02;
        public const int DID_SenAutoFocus = 0x03;
        public const int DID_SenFocusRate = 0x04;
        public const int DID_SenFocusPosn = 0x05;
        public const int DID_SenZoomRate = 0x06;
        public const int DID_SenZoomFOV = 0x07;
        public const int DID_SenZoomPosn = 0x08;
        public const int DID_SenZoomMode = 0x09;
        public const int DID_SenAperPosn = 0x0A;
        public const int DID_SenExpMode = 0x0B;
        public const int DID_SenExpPosn = 0x0C;
        public const int DID_SenGainMode = 0x0D;
        public const int DID_SenGainRate = 0x0E;
        public const int DID_SenGainPosn = 0x0F;
        public const int DID_SenOffsetRate = 0x10;
        public const int DID_SenOffsetPosn = 0x11;
        public const int DID_SenIR_Cut = 0x12;
        public const int DID_SenWipe = 0x13;
        public const int DID_SenBS_PosnX = 0x14;
        public const int DID_SenBS_PosnY = 0x15;
        public const int DID_SenZoomStop = 0x16;
        public const int DID_SenFocusStop = 0x17;
        public const int DID_SenPolarity = 0x18;
        public const int DID_SenWipeInter = 0x19;
        public const int DID_SenExtender = 0x1A;     // Optical zoom extender
        public const int DID_SenGainStop = 0x1B;
        public const int DID_SenOffsetStop = 0x1C;
        public const int DID_SenDigiZoom = 0x1D;
        public const int DID_SenEnhanEnable = 0x1E;
        public const int DID_SenEnhanFactor = 0x1F;
        public const int DID_SenEstabEnable = 0x20;
        public const int DID_SenRecEnable = 0x21;
        public const int DID_SenStrmEnable = 0x22;
        public const int DID_SenBS_Symb = 0x23;
        public const int DID_SenIrisStep = 0x25;
        public const int DID_SenShutStep = 0x27;
        public const int DID_SenRunBIT = 0x2F;
        public const int DID_SenComms = 0x30;
        public const int DID_SenWarnings = 0x31;
        public const int DID_SenErrors = 0x32;
        public const int DID_SenOpHours = 0x33;
        public const int DID_SenVersion = 0x34;
        public const int DID_SenTemp1 = 0x35;
        public const int DID_SenTemp2 = 0x36;
        public const int DID_SenInputState = 0x3A;     // Video input status

        public const int DID_SenCustomStart = 0x40;  // Custom Range
        public const int DID_SenCustomEnd = 0x57;

        public const int DID_SenVideoStd = 0x58;
        public const int DID_SenFOV_Horiz = 0x59;
        public const int DID_SenFOV_Ratio = 0x5A;     // Horiz / Vertical
        public const int DID_SenProcRgnL = 0x5B;
        public const int DID_SenProcRgnR = 0x5C;
        public const int DID_SenProcRgnT = 0x5D;
        public const int DID_SenProcRgnB = 0x5E;
        public const int DID_SenVidLatency = 0x5F;

        public const int DID_SenEstabReset = 0x60;
        public const int DID_SenEstabZoom = 0x61;
        public const int DID_SenEstabFilterWarp = 0x62;
        public const int DID_SenEstabStabAlpha = 0x63;
        public const int DID_SenEstabSlewAlpha = 0x64;
        public const int DID_SenEstabSlewMag = 0x65;
        public const int DID_SenEstabRelaxAlpha = 0x66;

        ///////////////////////////////////////////////////////
        /// LRF data definitions (Sub-System 0x04)
        public const int DID_LRF_FireOne = 0x00;
        public const int DID_LRF_FireBurst = 0x01;
        public const int DID_LRF_Status = 0x10;
        public const int DID_LRF_FireRes = 0x11;
        public const int DID_LRF_FireRng = 0x12;
        public const int DID_LRF_FireTime = 0x13;
        public const int DID_LRF_OnOff = 0x20;
        public const int DID_LRF_Enable = 0x21;
        public const int DID_LRF_ValidTime = 0x22;        // Time in seconds that the Laser result is held as valid
        public const int DID_LRF_RunBIT = 0x2F;
        public const int DID_LRF_Comms = 0x30;
        public const int DID_LRF_Warnings = 0x31;
        public const int DID_LRF_Errors = 0x32;
        public const int DID_LRF_OpHours = 0x33;
        public const int DID_LRF_Version = 0x34;
        public const int DID_LRF_Shots = 0x35;

        ///////////////////////////////////////////////////////
        /// Tracker data definitions (Sub-System 0x05)
        public const int DID_Trk_Mode = 0x00;
        public const int DID_Trk_DetAlg = 0x01;
        public const int DID_Trk_AutoAcquire = 0x02;
        public const int DID_Trk_VideoSource = 0x03;
        public const int DID_Trk_CoastDuration = 0x04;
        public const int DID_Trk_DetPriority = 0x05;
        public const int DID_Trk_TrackAlg = 0x06;
        public const int DID_Trk_Polarity = 0x07;
        public const int DID_Trk_Enable = 0x08;
        public const int DID_Trk_DetId = 0x09;
        public const int DID_Trk_DetAreaPosnX = 0x10;
        public const int DID_Trk_DetAreaPosnY = 0x11;
        public const int DID_Trk_DetAreaSizeX = 0x12;
        public const int DID_Trk_DetAreaSizeY = 0x13;
        public const int DID_Trk_AimpointOffsetIncX = 0x14;
        public const int DID_Trk_AimpointOffsetIncY = 0x15;
        public const int DID_Trk_ForceCoast = 0x16;
        public const int DID_Trk_AimpointBias = 0x17;
        public const int DID_Trk_AOI_SizeAdjX = 0x18;
        public const int DID_Trk_AOI_SizeAdjY = 0x19;
        public const int DID_Trk_AOI_PosnAdjX = 0x1A;
        public const int DID_Trk_AOI_PosnAdjY = 0x1B;
        public const int DID_Trk_PredictionEn = 0x1E;
        public const int DID_Trk_PredictionAlpha = 0x1F;
        public const int DID_Trk_CoastThresh = 0x20;
        public const int DID_Trk_ModelUpdateAlpha = 0x21;
        public const int DID_Trk_ListOutputEn = 0x22;
        public const int DID_Trk_ListOutputFormat = 0x23;
        public const int DID_Trk_ListOutputNum = 0x24;
        public const int DID_Trk_ListOutputHyst = 0x25;
        public const int DID_Trk_TrackOnObjID = 0x26;
        public const int DID_Trk_ManDetUsesTrkAlg = 0x27;
        public const int DID_Trk_PrimaryObjStatus = 0x40;
        public const int DID_Trk_PrimaryObjTime = 0x41;
        public const int DID_Trk_PrimaryObjPixPosnX = 0x42;
        public const int DID_Trk_PrimaryObjPixPosnY = 0x43;
        public const int DID_Trk_PrimaryObjSizeX = 0x44;
        public const int DID_Trk_PrimaryObjSizeY = 0x45;
        public const int DID_Trk_PrimaryObjAzPosn = 0x46;
        public const int DID_Trk_PrimaryObjElPosn = 0x47;

        /// Object Detection Parameters
        public const int DID_Trk_ObjMaxReported = 0x48;
        public const int DID_Trk_ObjMinArea = 0x49;
        public const int DID_Trk_ObjMaxArea = 0x4A;
        public const int DID_Trk_ObjMinWidth = 0x4B;
        public const int DID_Trk_ObjMaxWidth = 0x4C;
        public const int DID_Trk_ObjMinHeight = 0x4D;
        public const int DID_Trk_ObjMaxHeight = 0x4E;
        public const int DID_Trk_ObjMinAspect = 0x4F;
        public const int DID_Trk_ObjMaxAspect = 0x50;


        ///////////////////////////////////////////////////////
        /// Wide Area Scan data definitions (Sub-System 0x06)
        public const int DID_Scan_Mode = 0x01;
        public const int DID_Scan_AzStart = 0x02;
        public const int DID_Scan_AzStop = 0x03;
        public const int DID_Scan_Elevation = 0x04;
        public const int DID_Scan_Speed = 0x05;
        public const int DID_Scan_SensorMode = 0x06;
        public const int DID_Scan_SensorFov = 0x07;
        public const int DID_Scan_ElStop = 0x08;
        public const int DID_Stare_Time = 0x09;
        public const int DID_Scan_TrackOutputEnable = 0x28;
        public const int DID_Scan_TrackOutputMaxTracks = 0x29;
        public const int DID_Scan_TrackOutputFormat = 0x2A;

        ///////////////////////////////////////////////////////
        /// Recorder data definitions (Sub-System 0x07)
        public const int DID_RecRecord = 0x00;
        public const int DID_RecStatus = 0x01;

        ///////////////////////////////////////////////////////
        /// Tracker overlay definitions (Sub-System 0x07)
        public const int DID_Ovl_Enable = 0x00;
        public const int DID_Ovl_StatusEnable = 0x01;
        public const int DID_Ovl_StatusPosnX = 0x02;
        public const int DID_Ovl_StatusPosnY = 0x03;
        public const int DID_Ovl_DetAOI_En = 0x06;
        public const int DID_Ovl_AimpointEn = 0x07;
        public const int DID_Ovl_TrkTgtSizeEn = 0x08;
        public const int DID_Ovl_CamBS_En = 0x09;     // \todo Resolve this compared to the Camera Boresight enable
        public const int DID_Ovl_SecTargetsEn = 0x0A;
        public const int DID_Ovl_PreprocVidEn = 0x0B;
        public const int DID_Ovl_NumTagsEn = 0x0C;

        ///////////////////////////////////////////////////////
        /// GPS data definitions (Sub-System 0x0A)
        public const int DID_GPS_Comms = 0x30;
        public const int DID_GPS_Warnings = 0x31;
        public const int DID_GPS_Errors = 0x32;

        ///////////////////////////////////////////////////////
        /// Encoding, Streaming & Recording data definitions (Sub-System 0x10)
        public const int DID_ESR_EncoderH264Level = 0x01;
        public const int DID_ESR_EncoderH264Profile = 0x02;
        public const int DID_ESR_EncoderSkipFrames = 0x03;
        public const int DID_ESR_EncoderResolution = 0x04;
        public const int DID_ESR_EncoderHorizontalResolution = 0x05;
        public const int DID_ESR_EncoderVerticalResolution = 0x06;
        public const int DID_ESR_EncoderBitRate = 0x07;
        public const int DID_ESR_EncoderQuality = 0x08;
        public const int DID_ESR_StreamEnable = 0x10;
        public const int DID_ESR_StreamTargetIp = 0x11;
        public const int DID_ESR_StreamTargetPort = 0x12;
        public const int DID_ESR_RecordEnable = 0x20;
        public const int DID_ESR_RecordChannelName = 0x21;
        public const int DID_ESR_RecordBufferSize = 0x22;
        public const int DID_ESR_RecordDiskSpace = 0x23;
        public const int DID_ESR_RecordSubDir = 0x24;
        public const int DID_ESR_SaveConfig = 0x30;

        ///////////////////////////////////////////////////////
        /// GPIO data definitions (Sub-System 0x14)
        public const int DID_GPIO_Outputs = 0x00;
        public const int DID_GPIO_Inputs = 0x01;
        public const int DID_GPIO_Comms = 0x30;

        ///////////////////////////////////////////////////////
        /// Audio output data definitions (Sub-System 0x15)
        public const int DID_AO_Enable = 0x00;
        public const int DID_AO_Range = 0x01;
        public const int DID_AO_Volume = 0x02;
        public const int DID_AO_Source = 0x08;
        public const int DID_AO_Alarm = 0x09;
        public const int DID_AO_FileNum = 0x0A;
        public const int DID_AO_Output = 0x10;
        public const int DID_AO_RangeChange = 0x11;
        public const int DID_AO_File0 = 0x20;
        public const int DID_AO_File1 = 0x21;
        public const int DID_AO_File2 = 0x22;
        public const int DID_AO_File3 = 0x23;
        public const int DID_AO_File4 = 0x24;
        public const int DID_AO_File5 = 0x25;
        public const int DID_AO_File6 = 0x26;
        public const int DID_AO_File7 = 0x27;
        public const int DID_AO_File8 = 0x28;
        public const int DID_AO_File9 = 0x29;
        public const int DID_AO_FileA = 0x2A;
        public const int DID_AO_FileB = 0x2B;
        public const int DID_AO_FileC = 0x2C;
        public const int DID_AO_FileD = 0x2D;
        public const int DID_AO_FileE = 0x2E;
        public const int DID_AO_FileF = 0x2F;
        public const int DID_AO_Comms = 0x30;
        public const int DID_AO_Warnings = 0x31;
        public const int DID_AO_Errors = 0x32;

        ///////////////////////////////////////////////////////
        /// Video Control data definitions (Sub-System 0x16)
        public const int DID_VidRecordCh1 = 0x00;
        public const int DID_VidStreamCh1 = 0x01;
        public const int DID_VidRecordCh2 = 0x02;
        public const int DID_VidStreamCh2 = 0x03;
        public const int DID_VidMJPG_LoopCh1 = 0x04;
        public const int DID_VidMJPG_LoopCh2 = 0x05;
        public const int DID_VidEnablePlayback = 0x10;
        public const int DID_VidPlaybackControl = 0x11;
        public const int DID_VidPlaybackSeek = 0x12;
        public const int DID_VidNavigateFolder = 0x13;
        public const int DID_VidArchiveLoop1 = 0x20;
        public const int DID_VidArchiveLoop2 = 0x21;
        public const int DID_VidSnapshot1 = 0x22;
        public const int DID_VidSnapshot2 = 0x23;
        public const int DID_VidSysTimeOffset = 0x28;
        public const int DID_VidEraseRecDrive = 0x30;
        public const int DID_VidLicenseEnable = 0x38;
        public const int DID_VidInputStatus1 = 0x40;
        public const int DID_VidInputStatus2 = 0x41;

        ///////////////////////////////////////////////////////
        /// External Device data definitions (Sub-System 0x20)
        public const int DID_ExtDevEnable = 0x00;
        public const int DID_ExtDevBulbLifetime = 0x10;  // Selex Petronas Spotlight
        public const int DID_ExtDevBulbLifeLeft = 0x11;  // Selex Petronas Spotlight
        public const int DID_ExtDevBulbReset = 0x12;  // Selex Petronas Spotlight
        public const int DID_ExtDevComms = 0x30;
        public const int DID_ExtDevWarnings = 0x31;
        public const int DID_ExtDevErrors = 0x32;

        ///////////////////////////////////////////////////////
        /// Target Designation Control data definitions

        public const int DID_TD_CTRL_PrimaryDesTgtId = 0x00;
        public const int DID_TD_CTRL_PrimaryDesTgtStatus = 0x01;
        public const int DID_TD_CTRL_AimOffEnable = 0x05;
        public const int DID_TD_CTRL_AimOffAzimuth = 0x06;
        public const int DID_TD_CTRL_AimOffElevation = 0x07;
        public const int DID_TD_CTRL_OutputMsgFrequency = 0x50;
        public const int DID_TD_CTRL_OutputTgtDataFormat = 0x51;
        public const int DID_TD_CTRL_OutputTgtCount = 0x52;
        public const int DID_TD_CTRL_Comms = 0x60;

        ///////////////////////////////////////////////////////
        /// Target Designation data definitions
        public const int DID_TD_PosnAz = 0x00;
        public const int DID_TD_PosnEl = 0x01;
        public const int DID_TD_PosnValid = 0x02;
        public const int DID_TD_Range = 0x03;
        public const int DID_TD_RangeValid = 0x04;
        public const int DID_TD_TargetID = 0x05;
        public const int DID_TD_Time = 0x06;
        public const int DID_TD_Comms = 0x07;

        ///////////////////////////////////////////////////////
        /// Cntl Source data definitions
        public const int DID_CS_Source = 0x00;     ///< Chess BAE Current Control Source
        public const int DID_CS_OffsetX = 0x01;
        public const int DID_CS_OffsetY = 0x02;
        public const int DID_CS_OffsetReset = 0x03;     ///< Chess BAE request to reset Control Source offset

        ///////////////////////////////////////////////////////
        /// Cntl Source data definitions
        public const int DID_PT_OnOff = 0x00;
        public const int DID_PT_TxMessage = 0x01;
        public const int DID_PT_RxMessage = 0x02;

        ///////////////////////////////////////////////////////
        /// Disruptor data definitions (Sub-System 0x24)
        public const int DID_DisrBands = 0x00;     // Number of bands supported by the dispruptor
        public const int DID_DisrArm = 0x01;
        public const int DID_DisrOutputs = 0x02;
        public const int DID_DisrSources = 0x03;
        public const int DID_DisrSetBands = 0x04;
        public const int DID_DisrJammingTime = 0x05;
        public const int DID_DisrTime = 0x07;
        public const int DID_DisrCLAW_OK = 0x08;
        public const int DID_DisrCLAW_Intlock = 0x09;
        public const int DID_DisrCLAW_Armed = 0x0A;
        public const int DID_DisrAppliOK = 0x0B;
        public const int DID_DisrAppliIntlock = 0x0C;
        public const int DID_DisrAppliArmed = 0x0D;
        public const int DID_DisrBand0State = 0x10;
        public const int DID_DisrBand0NumSrcs = 0x11;
        public const int DID_DisrBand0SrcState = 0x12;     ///< Bitfield for the number of sources
        public const int DID_DisrBand0Power = 0x14;
        public const int DID_DisrBand0Atten = 0x15;
        public const int DID_DisrBand1State = 0x18;
        public const int DID_DisrBand1NumSrcs = 0x19;
        public const int DID_DisrBand1SrcState = 0x1A;     ///< Bitfield for the number of sources
        public const int DID_DisrBand1Power = 0x1C;
        public const int DID_DisrBand1Atten = 0x1D;
        public const int DID_DisrBand2State = 0x20;
        public const int DID_DisrBand2NumSrcs = 0x21;
        public const int DID_DisrBand2SrcState = 0x22;     ///< Bitfield for the number of sources
        public const int DID_DisrBand2Power = 0x24;
        public const int DID_DisrBand2Atten = 0x25;
        public const int DID_DisrBand3State = 0x28;
        public const int DID_DisrBand3NumSrcs = 0x29;
        public const int DID_DisrBand3SrcState = 0x2A;     ///< Bitfield for the number of sources
        public const int DID_DisrBand3Power = 0x2C;
        public const int DID_DisrBand3Atten = 0x2D;
        public const int DID_DisrBand4State = 0x30;
        public const int DID_DisrBand4NumSrcs = 0x31;
        public const int DID_DisrBand4SrcState = 0x32;     ///< Bitfield for the number of sources
        public const int DID_DisrBand4Power = 0x34;
        public const int DID_DisrBand4Atten = 0x35;
        public const int DID_DisrBand5State = 0x38;
        public const int DID_DisrBand5NumSrcs = 0x39;
        public const int DID_DisrBand5SrcState = 0x3A;     ///< Bitfield for the number of sources
        public const int DID_DisrBand5Power = 0x3C;
        public const int DID_DisrBand5Atten = 0x3D;
        public const int DID_DisrComms = 0x50;
        public const int DID_DisrWarnings = 0x51;
        public const int DID_DisrErrors = 0x52;

        ///////////////////////////////////////////////////////
        /// Other messages
        public const int VMS_DATA_MSG_HEADER_CHAR = 0x55;        ///< Header character for VMS TX or RX data messages
        public const int VMS_VRU_MSG_ID = 0x01;        // ID character for VMS VRU data message
        public const int VMS_TD_MULTI_MSG_ID = 0x03;        // ID character for VMS Multiple Target data message
        public const int VMS_TD_SINGLE_MSG_ID = 0x04;        // ID character for VMS Single Target data message
        public const int VMS_TD_DELETE_MSG_ID = 0x05;        // ID character for VMS Single Target delete data message
        public const int VMS_GPS_MSG_ID = 0x06;        // ID character for VMS GPS data input message
        public const int VMS_EO_TRACK_MSG_ID = 0x07;        // ID character for VMS EO track data output message

        public const int VMS_EO_TRACK_DATA_HEADER_CHAR = 0x77;   // Header character for EO Track List output

        public const int VMS_FORMAT_SHIP_REL = 0;
        public const int VMS_FORMAT_SHIP_REL_WITH_RANGE = 1;
        public const int VMS_FORMAT_NORTH_REL = 8;
        public const int VMS_FORMAT_NORTH_REL_WITH_RANGE = 9;
        public const int VMS_FORMAT_PIXELS = 30; // Pixels - relative to screen origin
        public const int VMS_FORMAT_PIXELS_BORESIGHT = 31; // Pixels - relative to boresight origin

        /// Bit states indicating whether a remote user can control the system
        public const int NO_CONTROLLER = 0x00;
        public const int CURRENT_CONTROLLER = 0x01;
        public const int OTHER_CONTROLLER = 0x02;
        public const int CONTROL_BLOCKED = 0x04;


    }
}
