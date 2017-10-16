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
        private static Queue<string> toLog; // MUST ALWAYS LOCK WHEN YOU ACCESS THIS
        public static StringBuilder sb;
        public static volatile bool bLogging = true;
        public static Thread loggerthread;
        public static volatile bool bFinished = false;

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
            if (loggerthread != null)
            {
                throw new Exception("Logger thread already started!");
            }
            logfile = nlogfile;
            toLog = new Queue<string>();
            sb = new StringBuilder(128);
            loggerthread = new Thread(new ThreadStart(LoggerThread));
            loggerthread.Name = "Logger";
            loggerthread.Priority = ThreadPriority.BelowNormal;
            //loggerthread.IsBackground = true;
            loggerthread.Start();
        }

        public static void WaitForBufferToClear()
        {
            while (!bFinished)
            {
                Thread.Sleep(30);
                lock (toLog)
                {
                    if (toLog.Count <= 0)
                    {
                        return;
                    }
                }
            }
        }

        public static void StopLogging()
        {
            bLogging = false;

            int i = 0;
            while (!bFinished)
            {
                Thread.Sleep(16);
                i++;
                if (i > 60) // a little under a second
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
                        Thread.Sleep(100);
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
