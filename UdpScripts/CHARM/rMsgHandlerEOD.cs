using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Udp
{ 
    class rMsgHandlerEOD : rCHARM_MsgHandler
    {
        public rMsgHandlerEOD(byte uSubSysIndex)
        {
            m_cSubSysId = RC_MsgDefinesCHARM.RC_OBJECT_LOCATION_SUB_SYS;  //Sub-System ID 0x00
            m_cSubSysIndex = uSubSysIndex;
        }
    }
}
