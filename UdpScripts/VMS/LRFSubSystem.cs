using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRFSubSystem : MonoBehaviour
{
    public byte OnGetComms()
    {
        return 0x02;
    }

    public void OnSetFire (byte data)
    {
        LogHelper.Log(LogTarget.DebugLog, "OnSetFire");
    }

    public void OnSetBurst (float data)
    {
        LogHelper.Log(LogTarget.DebugLog, "OnSetBurst");
    }

    public byte OnGetStatus()
    {
        return 0x01;
    }

    public byte OnGetResult()
    {
        return 0x01;
    }

    public ushort OnGetRange()
    {
        return 20;
    }

    public int OnGetTime()
    {
        return 10;
    }

    public byte OnGetPower()
    {
        return 0x01;
    }

    public void OnSetEnable(byte nData)
    {
        LogHelper.Log(LogTarget.DebugLog, "OnSetEnable");
    }

    public byte OnGetEnable()
    {
        return 0x01;
    }

    public float OnGetWarnings()
    {
        return 0x00;
    }

    public float OnGetErrors()
    {
        return 0x00;
    }
}
