using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Text;
using System.Linq;
using System.Collections;

public class EODSubSystem : MonoBehaviour
{
    private GameObject m_platform;
    private Quaternion m_platformRot;
    private Vector3 m_currentPlatformRot;
    private Vector3 m_truePlatformRot;
    private Vector3 m_setupRot;
    private float m_platformAzRate;
    private float m_platformElRate;
    private float m_rotationSpeedX;
    private float m_rotationSpeedY;
    private float m_zoomRate;
    private ushort camFocusPons;
    private Vector3 m_rateRotation;
    private Vector3 m_rateRotationMouse;
    private Quaternion m_platformRate;
    private bool m_joyPad;
    private static Mutex mutex = new Mutex();

    //test varibles
    private float updateUpdateCountPerSecond;
    private float updateCount = 0;

    // Start is called before the first frame update
    void Awake()
    {
        m_platform = this.gameObject;
        Logger.LoggingToggle = false;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        m_platformRot = m_platform.transform.rotation;
        StartCoroutine(Loop());
    }

    void Start()
    {
        m_currentPlatformRot = this.transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if(Application.targetFrameRate != 60)
        {
            Application.targetFrameRate = 60;
        }
        m_currentPlatformRot = transform.rotation.eulerAngles;
        mutex.WaitOne();
        if (m_joyPad == false)
            transform.rotation = Quaternion.Lerp(transform.rotation, m_platformRot, Time.deltaTime);
        else if (m_joyPad == true)
            transform.rotation = Quaternion.Lerp(transform.rotation, m_platformRate, Time.deltaTime);

        //Keeps the Camera locked on the Z axis
        Quaternion q = transform.rotation;
        q.eulerAngles = new Vector3(q.eulerAngles.x, q.eulerAngles.y, 0);
        transform.rotation = q;
        Vector3 raterotation = new Vector3(m_rateRotation.x, m_rateRotation.y, 0);
        m_platformRate = Quaternion.Euler(q.eulerAngles + raterotation);

        m_truePlatformRot = transform.rotation.eulerAngles;
        mutex.ReleaseMutex();

        updateCount += 1;
    }

    //Camera Movment Commands
    public void SetPosnAz(float data)
    {
        m_joyPad = false;
        m_rateRotationMouse.y = HelperFunctions.RadtoDeg(data);
        m_platformRot = Quaternion.Euler(m_rateRotationMouse);
    }

    public void SetPosnEl(float data)
    {
        m_joyPad = false;
        m_rateRotationMouse.x = -HelperFunctions.RadtoDeg(data);
        m_platformRot = Quaternion.Euler(m_rateRotationMouse);

        //Debug.Log("SetPosnEl");
    }

    public float GetPosEl()
    {
        //Debug.Log("GetPosnEl");
        return HelperFunctions.DegtoRad(-m_truePlatformRot.x);
    }

    public float GetPosAz()
    {
        return HelperFunctions.DegtoRad(m_truePlatformRot.y);
    }

    public void SetAzRate(float data)
    {
        m_joyPad = true;
        var degreeseRate = HelperFunctions.RadtoDeg(data);
        m_rotationSpeedY = degreeseRate;


        m_rateRotation.y = m_rotationSpeedY;
        m_rateRotation.z = 0;

        m_platformRate = Quaternion.Euler(m_currentPlatformRot += m_rateRotation);
    }
    public void SetElRate(float data)
    {
        m_joyPad = true;
        var degreeseRate = HelperFunctions.RadtoDeg(data);
        m_rotationSpeedX = degreeseRate;


        m_rateRotation.x = -m_rotationSpeedX;
        m_rateRotation.z = 0;

        m_platformRate = Quaternion.Euler(m_currentPlatformRot += m_rateRotation);
    }
    public byte OnGetCommsEOD()
    {
        return 0x02;
    }
    public ushort OnGetWarningsEOD()
    {
        return 0x00;
    }
    public ushort OnGetErrorsEOD()
    {
        return 0x00;
    }

    IEnumerator Loop()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(1);
            updateUpdateCountPerSecond = updateCount;

            updateCount = 0;
        }
    }
    void OnGUI()
    {
        GUIStyle fontSize = new GUIStyle(GUI.skin.GetStyle("label"));
        fontSize.fontSize = 24;
        GUI.Label(new Rect(100, 100, 200, 50), "Update: " + updateUpdateCountPerSecond.ToString(), fontSize);
    }
}
