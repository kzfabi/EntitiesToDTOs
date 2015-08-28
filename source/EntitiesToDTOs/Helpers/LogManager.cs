/* EntitiesToDTOs. Copyright (c) 2011. Fabian Fernandez.
 * http://entitiestodtos.codeplex.com
 * Licensed by Common Development and Distribution License (CDDL).
 * http://entitiestodtos.codeplex.com/license
 * Fabian Fernandez. 
 * http://www.linkedin.com/in/fabianfernandezb/en
 * */
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using EntitiesToDTOs.Properties;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Manages the Log operations.
    /// </summary>
    internal class LogManager
    {
        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        private static object Lock = new object();

        /// <summary>
        /// Log file path.
        /// </summary>
        public static string LogFilePath
        {
            get
            {
                if (LogManager.logFilePath == null)
                {
                    LogManager.logFilePath = Path.GetTempPath()
                        + Resources.EntitiesToDTOsTempFolderName
                        + Resources.LogFileName;
                }

                return LogManager.logFilePath;
            }
        }
        private static string logFilePath = null;

        /// <summary>
        /// Indicates if it is the first log in this Visual Studio session.
        /// </summary>
        public static bool isFirstLog = true;



        /// <summary>
        /// Logs an Exception.
        /// </summary>
        /// <param name="ex">Exception to log.</param>
        public static void LogError(Exception ex)
        {
            LogManager.Log(Resources.LogError + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
        }

        /// <summary>
        /// Logs an Info message
        /// </summary>
        /// <param name="logMessage"></param>
        public static void LogInfo(string logMessage)
        {
            LogManager.Log(Resources.LogInfo + Environment.NewLine + logMessage);
        }

        /// <summary>
        /// Logs method start.
        /// </summary>
        public static void LogMethodStart()
        {
            // Get call stack
            var stackTrace = new StackTrace();

            // Get calling method name
            string methodName = stackTrace.GetFrame(1).GetMethod().Name;

            LogManager.LogInfo(string.Format(Resources.Info_MethodStart, methodName));
        }

        /// <summary>
        /// Logs method end.
        /// </summary>
        public static void LogMethodEnd()
        {
            // Get call stack
            StackTrace stackTrace = new StackTrace();

            // Get calling method name
            string methodName = stackTrace.GetFrame(1).GetMethod().Name;

            LogManager.LogInfo(string.Format(Resources.Info_MethodEnd, methodName));
        }

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="logMessage"></param>
        private static void Log(string logMessage)
        {
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = false;

            worker.DoWork += new DoWorkEventHandler(LogManager.Log_DoWork);

            worker.RunWorkerAsync(logMessage);
        }

        /// <summary>
        /// Executed when the BackgroundWorker decides to execute the provided Logic (i.e., the Log process).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Log_DoWork(object sender, DoWorkEventArgs args)
        {
            try
            {
                // Synchronize threads
                lock (LogManager.Lock)
                {
                    // First log in this Visual Studio session?
                    if (LogManager.isFirstLog == true)
                    {
                        LogManager.isFirstLog = false;

                        // Delete log file, we are going to create a new log file 
                        // for this Visual Studio session
                        File.Delete(LogManager.LogFilePath);
                    }

                    // Compose log entry
                    string logMessage = LogManager.GetLogEntryTimeStamp() + (string)args.Argument 
                        + Environment.NewLine + Resources.LogSeparator + Environment.NewLine;

                    // Append log entry to log file
                    File.AppendAllText(LogManager.LogFilePath, logMessage);
                }
            }
            catch (Exception ex)
            {
                // Create a Warning if Log is not possible
                string message = (args.Argument != null ? args.Argument.ToString() : string.Empty);

                message = string.Format(Resources.Warning_CouldNotLogMessage, message) + Environment.NewLine + 
                    string.Format(Resources.Warning_ExceptionWhenLog, ex.Message);

                // Add warning to ErrorList pane
                VisualStudioHelper.AddToErrorList(TaskErrorCategory.Warning, message, null, null, null, null);
            }
        }

        /// <summary>
        /// Gets a timestamp to use in a log entry.
        /// </summary>
        /// <returns>Timestamp in the following format: yyyy/MM/dd HH:mm:ss</returns>
        private static string GetLogEntryTimeStamp()
        {
            return DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss (UTC) ");
        }
    }
}