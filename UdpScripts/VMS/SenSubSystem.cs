using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenSubSystem : MonoBehaviour
{
    public float fov;
    private float setFoV;
    private float currentFoV;
    private float rateMod;
    byte[] bytes;
    byte[] bytes2;
    // Start is called before the first frame update
    void Start()
    {
        fov = 0;
        setFoV = 0;
        rateMod = 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.GetComponent<Camera>().fieldOfView > 10 && gameObject.GetComponent<Camera>().fieldOfView < 60)
        {
            GetComponent<Camera>().fieldOfView += fov * Time.deltaTime;
        }
        if (gameObject.GetComponent<Camera>().fieldOfView < 10)
        {
            GetComponent<Camera>().fieldOfView = 10.1f;
        }
        if (gameObject.GetComponent<Camera>().fieldOfView > 60)
        {
            GetComponent<Camera>().fieldOfView = 59.9f;
        }

        if(setFoV != 0)
        {
            if (GetComponent<Camera>().fieldOfView < setFoV)
            {
                GetComponent<Camera>().fieldOfView += 1;
            }
            else if (GetComponent<Camera>().fieldOfView < setFoV)
            {
                GetComponent<Camera>().fieldOfView -= 1;
            }
            setFoV = 0;
        }

        currentFoV = GetComponent<Camera>().fieldOfView;
    }

    //Set Camera Focus Commands
    public byte OnGetMode()
    {
        return 0x01;
    }
    public void OnSetFocusRate(ushort data)
    {
    }
    public ushort OnGetFocusPons()
    {
        return 0x0001;
    }
    public void OnSetFocusStop(byte data)
    {
    }
    public void OnSetAperPons(byte data)
    {
    }
    public void OnSetZoomRate(ushort data)
    {
        if (data > 32767)
        {
            Debug.Log(HelperFunctions.ZoomRate(32767, 1, data) - rateMod);
            fov = HelperFunctions.ZoomRate(32767,1,data) * rateMod;
        }
        else if (data < 32767)
        {
            Debug.Log(HelperFunctions.ZoomRate(32767, 1, data) - rateMod);
            fov = -HelperFunctions.ZoomRate(32767, 1, data) * rateMod;
        }
    }
    public void OnSetZoomFOV(ushort data)
    {

    }
    public ushort OnGetZoomFOV()
    {
        double temp = Convert.ToDouble(HelperFunctions.DegtoRad(currentFoV));
        return Convert.ToUInt16(temp);
    }
    public void OnSetZoomPons(ushort data)
    {

    }
    public ushort OnGetZoomPons()
    {
        return 0x0001;
    }
    public void OnSetZoomMode(byte data)
    {

    }
    public byte OnGetZoomMode()
    {
        return 0x00;
    }
    public void OnSetZoomStop(byte data)
    {
        fov = 0f;
    }
    public byte OnGetZoomStop()
    {
        return 0x01;
    }
    public void OnSetExpMode(byte data)
    {
        Debug.Log("SetExpMode");
    }
    public byte OnGetExpMode()
    {
        return 0x00;
    }
    public void OnSetExpPosn()
    {
        Debug.Log("SetExpPosn");
    }
    public byte OnGetExpPosn()
    {
        return 0x00;
    }
    public void OnSetGainMode()
    {
        Debug.Log("SetGainMode");
    }
    public byte OnGetGainMode()
    {
        return 0x02;
    }
    public void OnSetGainRate()
    {
        Debug.Log("OnSetGainRate");
    }
    public void OnSetGainStop()
    {
        Debug.Log("OnSetGainRate");
    }
    public void OnSetOffsetRate()
    {
        Debug.Log("OnSetOffSetRate");
    }
    public byte OnGetCommsSens()
    {
        return 0x02;
    }
    public ushort OnGetWarningsSens()
    {
        return 0x00;
    }
    public ushort OnGetErrorsSens()
    {
        return 0x00;
    }
}
