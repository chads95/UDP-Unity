using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Udp
{
    public static class RC_MsgDefns
    {
        public enum AccessType
        {
            DIA_READ_ONLY = 0,
            DIA_WRITE_ONLY,
            DIA_READ_WRITE,
        }

        public enum SuccessType
        {
            DIS_OK = 0,
            DIS_UNKNOWN_ITEM,
            DIS_NOT_SUPPORTED,
            DIS_NOT_VALID_STATE,
            DIS_INVALID_ACCESS,
            DIS_INSUFFICIENT_BUFFER,
            DIS_OTHER_ERROR,
            DIS_NOT_INIT,
            DIS_DEVICE_BUSY,
            DIS_NOT_IN_CONTROL,
        }
    }
}
