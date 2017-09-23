/*********说明***********************
 * 简易日志记录类
 * ----------------------------------
 * 本日志类位静态类，所含方法均为静态
 * 方法，在使用时可直接调用。
 * ----------------------------------
 * 最后修改：李飞  2014年3月25日
 ***********************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace LogManagerClass
{
    /// <summary>
    /// 日志管理类
    /// </summary>
    public static class LogManager
    {
        private static string logPath = string.Empty;
        ///   <summary> 
        ///  保存日志的文件夹
        ///  "以\\结尾"
        ///   </summary> 
        public static string LogPath
        {
            get
            {
                if (logPath == string.Empty)
                {
                    logPath = Application.StartupPath + "\\Log\\";
                    if (!System.IO.Directory.Exists(logPath))//如果不存在就创建file文件夹  
                    {
                        System.IO.Directory.CreateDirectory(logPath);
                    }
                }
                return logPath;
            }
            set
            {
                logPath = value;
                if (System.IO.Directory.Exists(logPath) == false)//如果不存在就创建file文件夹  
                {
                    System.IO.Directory.CreateDirectory(logPath);
                }
            }
        }

        private static string logFielPrefix = string.Empty;
        ///   <summary> 
        ///  日志文件前缀
        ///   </summary> 
        public static string LogFielPrefix
        {
            get { return logFielPrefix; }
            set { logFielPrefix = value; }
        }

        ///   <summary> 
        ///  写日志
        ///   </summary> 
        public static void WriteLog(string logFile, string msg)
        {
            try
            {
                StreamWriter sw = File.AppendText(LogPath + LogFielPrefix + logFile + "_" + DateTime.Now.ToString("yyyyMMdd") + ".Log ");
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + msg);
                sw.Close();
            }
            catch
            { }
        }

        /// <summary>
        /// 写日志（默认日志类型为Error）
        /// </summary>
        /// <param name="ex">捕获的错误</param>
        public static void WriteLog(Exception ex)
        {
            WriteLog(LogFile.Error, ex);
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logfile">日志类型</param>
        /// <param name="ex">捕获的错误</param>
        public static void WriteLog(LogFile logfile, Exception ex)
        {
            try
            {
                DateTime logTime = DateTime.Now;
                string logFile = logfile.ToString();
                StreamWriter sw = System.IO.File.AppendText(LogPath + logfile.ToString() + "_" + logTime.ToString("yyyyMMdd") + ".Log ");
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sw.WriteLine("Message:" + Environment.NewLine + ex.Message + Environment.NewLine);
                sw.WriteLine("StackTrace:" + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                sw.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logFile">日志类型</param>
        /// <param name="msg">记录内容</param>
        public static void WriteLog(LogFile logFile, string msg)
        {
            WriteLog(logFile.ToString(), msg);
        }
    }

    ///   <summary> 
    ///  日志类型
    ///   </summary> 
    public enum LogFile
    {
        Success,
        Trace,
        Warning,
        Error,
        SQL,
        Log,
        State
    }

}
