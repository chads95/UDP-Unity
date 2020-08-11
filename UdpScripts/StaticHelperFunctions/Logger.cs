using System;
using System.IO;
using UnityEngine;
using System.Text;
using System.Linq;

public enum LogTarget
{
    File, Database, DebugLog
}
public enum LogLevel
{
    Debug, COMMS, Warnings
}
public abstract class Logger
{
    public static bool LoggingToggle = true;
    public static bool LoggingDebug = true;
    public static bool LoggingCOMMS = true;
    public static bool LoggingWarning = true;
    protected readonly object lockObj = new object();
    public abstract void Log(string message);
}

public class FileLogger : Logger
{
    public string filePath = Application.streamingAssetsPath + "DebugUdpLog";
    public override void Log(string message)
    {
        if (File.Exists(filePath))
        {
            try
            {

                if (LoggingToggle == true)
                {
                    lock (lockObj)
                    {
                        using (StreamWriter streamWriter = File.AppendText(filePath))
                        {
                            streamWriter.WriteLine(message);
                            streamWriter.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Debug.LogError(e.ToString());
            }
        }
        else
        {
            using (FileStream fs = File.Create(filePath))
            {
                byte[] fileName = new UTF8Encoding(true).GetBytes(message + Environment.NewLine);
                fs.Write(fileName, 0, fileName.Length);
            }
        }
    }
}

public class EventLogger : Logger
{
    public override void Log(string message)
    {
        if (LoggingToggle == true)
        {
            lock (lockObj)
            {
                Debug.Log(message);
            }
        }
    }
}

public class DBLogger : Logger
{
    string connectionString = string.Empty;
    public override void Log(string message)
    {
        if (LoggingToggle == true)
        {
            lock (lockObj)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}

public static class LogHelper
{
    private static Logger logger = null;
    public static void Log(LogTarget target, string message)
    {
        switch (target)
        {
            case LogTarget.File:
                logger = new FileLogger();
                logger.Log(message);
                break;
            case LogTarget.Database:
                logger = new DBLogger();
                logger.Log(message);
                break;
            case LogTarget.DebugLog:
                logger = new EventLogger();
                logger.Log(message);
                break;
            default:
                return;
        }
    }
    public static void Log(LogTarget target, LogLevel level, string message)
    {
        switch (target)
        {
            case LogTarget.File:
                switch (level)
                {
                    case LogLevel.Debug:
                        if (Logger.LoggingDebug == true)
                        {
                            logger = new FileLogger();
                            logger.Log(message);
                        }
                    break;

                    case LogLevel.COMMS:
                        if (Logger.LoggingCOMMS == true)
                        {
                            logger = new FileLogger();
                            logger.Log(message);
                        }
                        break;

                    case LogLevel.Warnings:
                        if (Logger.LoggingWarning)
                        {
                            logger = new FileLogger();
                            logger.Log(message);
                        }
                        break;
                    default:
                        return;
                }
                break;
            case LogTarget.Database:
                switch (level)
                {
                    case LogLevel.Debug:
                        if (Logger.LoggingDebug == true)
                        {
                            logger = new DBLogger();
                            logger.Log(message);
                        }
                        break;

                    case LogLevel.COMMS:
                        if (Logger.LoggingCOMMS == true)
                        {
                            logger = new DBLogger();
                            logger.Log(message);
                        }
                        break;

                    case LogLevel.Warnings:
                        if (Logger.LoggingWarning)
                        {
                            logger = new DBLogger();
                            logger.Log(message);
                        }
                        break;
                    default:
                        return;
                }
                break;
            case LogTarget.DebugLog:
                switch (level)
                {
                    case LogLevel.Debug:
                        if (Logger.LoggingDebug == true)
                        {
                            logger = new EventLogger();
                            logger.Log(message);
                        }
                        break;

                    case LogLevel.COMMS:
                        if (Logger.LoggingCOMMS == true)
                        {
                            logger = new EventLogger();
                            logger.Log(message);
                        }
                        break;

                    case LogLevel.Warnings:
                        if (Logger.LoggingWarning)
                        {
                            logger = new EventLogger();
                            logger.Log(message);
                        }
                        break;
                    default:
                        return;
                }
                break;
            default:
                return;
        }
    }
}

public class LogToText
{
    public static void LogToTextFile(float data)
    {
        string serilizedData = data.ToString();
        string dataPath = Application.streamingAssetsPath + "MsgLog";

        try
        {
            using (FileStream fs = File.Create(dataPath))
            {
                byte[] writer = new UTF8Encoding(true).GetBytes(serilizedData + Environment.NewLine);
                fs.Write(writer, 0, writer.Length);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}

public class HelperFunctions
{
    public static float RadtoDeg(float mRad)
    {
        double Deg = mRad * (180 / Mathf.PI);

        Deg = Deg / 1000;
        return (float)Deg;
    }

    public static float DegtoRad(float deg)
    {
        double Rad = Mathf.PI * deg / 180;

        Rad = Rad * 1000;
        return (float)Rad;
    }

    public static float CovertToRate(ushort rate)
    {
        float desValue = Convert.ToInt32(rate);
        return desValue;
    }

    private static void logTxMsg(byte[] tx_msg, int nMsgLength)
    {
        byte[] log_msg = new byte[nMsgLength];

        Array.Copy(tx_msg, log_msg, nMsgLength);

        //LogHelper.Log(LogTarget.DebugLog, "Tx: 0x" + BitConverter.ToString( log_msg ).Replace( "-", " 0x" ) );
    }

    public static float ZoomRate(float maxRate, float minRate, ushort rate)
    {
        float normalisedfloat = (rate - minRate) / (maxRate - minRate);
        Mathf.Clamp(rate, minRate, maxRate);
        return Mathf.Clamp(normalisedfloat, 0, 1);
    }
}
