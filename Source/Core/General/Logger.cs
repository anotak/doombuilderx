using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
// anotak

namespace CodeImp.DoomBuilder
{
    public class Logger
    {
        public static string logfile;
        public static Queue<string> toLog;
        public static StringBuilder sb;
        public static bool bLogging = true;
        public static Thread loggerthread;
        public static bool bFinished = false;

        // This outputs log information
        public static void WriteLogLine(string line)
        {
            lock (toLog)
            {
                toLog.Enqueue(line);
            }
        }

        public static void StartLogging(string nlogfile)
        {
            logfile = nlogfile;
            toLog = new Queue<string>();
            sb = new StringBuilder(128);

            loggerthread = new Thread(new ThreadStart(LoggerThread));
            loggerthread.Name = "Logger";
            loggerthread.Priority = ThreadPriority.BelowNormal;
            //loggerthread.IsBackground = true;
            loggerthread.Start();
        }

        public static void StopLogging()
        {
            bLogging = false;

            int i = 0;
            while (!bFinished)
            {
                Thread.Sleep(10);
                i++;
                if (i > 50) // 500 ms
                {
                    return;
                }
            }
        }

        public static void LoggerThread()
        {
            bool bNeedToLog = false;
            string line = "";
            // Remove the previous log file and start logging
            if (File.Exists(logfile)) File.Delete(logfile);
            while (true)
            {
                lock (toLog)
                {
                    if (toLog.Count > 0)
                    {
                        bNeedToLog = true;
                        line = toLog.Dequeue();
                    }
                    else
                    {
                        bNeedToLog = false;
                    }
                }
                if (bNeedToLog)
                {
                    // Output to console
                    Console.WriteLine(line);
                    
                    // Write to log file
                    try
                    {
                        sb.Append(line);
                        sb.Append(Environment.NewLine);
                        File.AppendAllText(logfile, sb.ToString());
                    }
                    catch (Exception)
                    {
                    }
                    sb.Length = 0;
                    
                }
                else
                {
                    Thread.Sleep(100);
                    if (!bLogging)
                    {
                        lock (toLog)
                        {
                            if (toLog.Count == 0)
                            {
                                bFinished = true;
                                break;
                            }
                        }
                    }
                }
            } // while true
            bFinished = true;
        }// loggerthreadd
    } // class
} // namespace
