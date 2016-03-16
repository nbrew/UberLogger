using System.IO;
using System.Text;

using UnityEngine;

using UberLogger;

/// <summary>
/// A basic file logger backend
/// </summary>
public class UberLoggerFile : UberLogger.ILogger
{
    private FileStream LogFileStream;
    private bool IncludeCallStacks;

    /// <summary>
    /// Constructor. Make sure to add it to UberLogger via Logger.AddLogger();
    /// filename is relative to Application.persistentDataPath
    /// if includeCallStacks is true it will dump out the full callstack for all logs, at the expense of big log files.
    /// </summary>
    public UberLoggerFile(string filename, bool includeCallStacks = true)
    {
        IncludeCallStacks = includeCallStacks;
        var fileLogPath = System.IO.Path.Combine(Application.persistentDataPath, filename);
        Debug.Log("Initialising file logging to " + fileLogPath);
        LogFileStream = new FileStream(fileLogPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
    }

    public void Log(LogInfo logInfo)
    {
        lock (this)
        {
            LogLineToFile(logInfo.Message);
            if (IncludeCallStacks && logInfo.Callstack.Count > 0)
            {
                foreach (var frame in logInfo.Callstack)
                {
                    LogLineToFile(frame.GetFormattedMethodName());
                }
                LogLineToFile("\n");
            }
        }
    }

    private void LogLineToFile(string message)
    {
        string line = message + "\n";
        byte[] lineBytes = Encoding.UTF8.GetBytes(line);
        LogFileStream.Write(lineBytes, 0, lineBytes.Length);
        LogFileStream.Flush();
    }
}
