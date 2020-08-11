using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace Udp
{
    class cMsgHandlerSystem : cVMS_MsgHandler
    {
        public cMsgHandlerSystem(byte uSubSysIndex)
        {
            m_cSubSysId = RC_MsgDefnsVMS.RC_CAMERA_SUB_SYS;
            m_cSubSysIndex = uSubSysIndex;
        }
    }
}
